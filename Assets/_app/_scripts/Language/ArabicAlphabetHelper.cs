using Antura.Database;
using ArabicSupport;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Antura.Helpers
{
    // TODO refactor: We should create an intermediate layer for accessing language-specific helpers, so that they can be removed easily.
    // TODO refactor: this class needs a large refactoring as it is used for several different purposes
    public static class ArabicAlphabetHelper
    {
        public struct ArabicStringPart
        {
            public LetterData letter;
            public int fromCharacterIndex;
            public int toCharacterIndex;
            public LetterForm letterForm;

            public ArabicStringPart(LetterData letter, int fromCharacterIndex, int toCharacterIndex, LetterForm letterForm)
            {
                this.letter = letter;
                this.fromCharacterIndex = fromCharacterIndex;
                this.toCharacterIndex = toCharacterIndex;
                this.letterForm = letterForm;
            }
        }

        struct UnicodeLookUpEntry
        {
            public LetterData data;
            public LetterForm form;

            public UnicodeLookUpEntry(Database.LetterData data, Database.LetterForm form)
            {
                this.data = data;
                this.form = form;
            }
        }

        struct DiacriticComboLookUpEntry
        {
            public string symbolID;
            public string LetterID;

            public DiacriticComboLookUpEntry(string symbolID, string LetterID)
            {
                this.symbolID = symbolID;
                this.LetterID = LetterID;
            }
        }

        static List<LetterData> allLetterData;
        static Dictionary<string, UnicodeLookUpEntry> unicodeLookUpCache = new Dictionary<string, UnicodeLookUpEntry>();

        static Dictionary<DiacriticComboLookUpEntry, LetterData> diacriticComboLookUpCache =
            new Dictionary<DiacriticComboLookUpEntry, LetterData>();

        /// <summary>
        /// Collapses diacritics and letters, collapses multiple words variations (e.g. lam + alef), selects correct forms unicodes, and reverses the string.
        /// </summary>
        /// <returns>The string, ready for display or further processing.</returns>
        public static string ProcessArabicString(string str)
        {
            return GenericHelper.ReverseText(ArabicFixer.Fix(str, true, true));
        }

        /// <summary>
        /// Return single letter string start from unicode hex code.
        /// </summary>
        /// <param name="hexCode">string Hexadecimal number</param>
        /// <returns>string char</returns>
        public static string GetLetterFromUnicode(string hexCode)
        {
            if (hexCode == "") {
                Debug.LogError(
                    "Letter requested with an empty hexacode (data is probably missing from the DataBase). Returning - for now.");
                hexCode = "002D";
            }

            int unicode = int.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);
            var character = (char)unicode;
            return character.ToString();
        }

        /// <summary>
        /// Get char hexa code.
        /// </summary>
        /// <param name="_char"></param>
        /// <param name="unicodePrefix"></param>
        /// <returns></returns>
        public static string GetHexUnicodeFromChar(char _char, bool unicodePrefix = false)
        {
            return string.Format("{1}{0:X4}", Convert.ToUInt16(_char), unicodePrefix ? "/U" : string.Empty);
        }

        /// <summary>
        /// Return a string of a word without a character. Warning: the word is already reversed and fixed for rendering.
        /// This is mandatory since PrepareArabicStringForDisplay should be called before adding removedLetterChar.
        /// </summary>
        public static string GetWordWithMissingLetterText(WordData arabicWord, ArabicStringPart partToRemove,
            string removedLetterChar = "_")
        {
            string text = ProcessArabicString(arabicWord.Arabic);

            int toCharacterIndex = partToRemove.toCharacterIndex + 1;
            text = text.Substring(0, partToRemove.fromCharacterIndex) + removedLetterChar +
                   (toCharacterIndex >= text.Length ? "" : text.Substring(toCharacterIndex));

            return text;
        }

        /// <summary>
        /// Find all the occurrences of "letterToFind" in "arabicWord"
        /// </summary>
        /// <returns>the list of occurrences</returns>
        public static List<ArabicStringPart> FindLetter(DatabaseManager database, WordData arabicWord, LetterData letterToFind)
        {
            var result = new List<ArabicStringPart>();

            var parts = AnalyzeData(database, arabicWord, false, letterToFind.Kind != LetterDataKind.LetterVariation);

            for (int i = 0, count = parts.Count; i < count; ++i) {
                if (parts[i].letter.Id == letterToFind.Id) {
                    result.Add(parts[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the list of letters found in a word string
        /// </summary>
        public static List<ArabicStringPart> AnalyzeData(DatabaseManager database, WordData arabicWord, bool separateDiacritics = false,
            bool separateVariations = true)
        {
            // Use ArabicFixer to deal only with combined unicodes
            return AnalyzeArabicString(database.StaticDatabase, ProcessArabicString(arabicWord.Arabic), separateDiacritics,
                separateVariations);
        }

        public static List<ArabicStringPart> AnalyzeData(DatabaseObject staticDatabase, WordData arabicWord,
            bool separateDiacritics = false, bool separateVariations = true)
        {
            // Use ArabicFixer to deal only with combined unicodes
            return AnalyzeArabicString(staticDatabase, ProcessArabicString(arabicWord.Arabic), separateDiacritics, separateVariations);
        }

        /// <summary>
        /// Returns the list of letters found in a word string
        /// </summary>
        public static List<ArabicStringPart> AnalyzeData(DatabaseManager database, PhraseData phrase, bool separateDiacritics = false,
            bool separateVariations = true)
        {
            // Use ArabicFixer to deal only with combined unicodes
            return AnalyzeArabicString(database.StaticDatabase, ProcessArabicString(phrase.Arabic), separateDiacritics, separateVariations);
        }

        public static List<ArabicStringPart> AnalyzeData(DatabaseObject staticDatabase, PhraseData phrase, bool separateDiacritics = false,
            bool separateVariations = true)
        {
            // Use ArabicFixer to deal only with combined unicodes
            return AnalyzeArabicString(staticDatabase, ProcessArabicString(phrase.Arabic), separateDiacritics, separateVariations);
        }

        static List<ArabicStringPart> AnalyzeArabicString(DatabaseObject staticDatabase, string processedArabicString,
            bool separateDiacritics = false, bool separateVariations = true)
        {
            if (allLetterData == null) {
                allLetterData = new List<LetterData>(staticDatabase.GetLetterTable().GetValuesTyped());

                for (int l = 0; l < allLetterData.Count; ++l) {
                    var data = allLetterData[l];

                    foreach (var form in data.GetAvailableForms()) {
                        if (data.Kind == LetterDataKind.Letter) {
                            // Overwrite
                            unicodeLookUpCache[data.GetUnicode(form)] = new UnicodeLookUpEntry(data, form);
                        } else {
                            var unicode = data.GetUnicode(form);

                            if (!unicodeLookUpCache.ContainsKey(unicode)) {
                                unicodeLookUpCache.Add(unicode, new UnicodeLookUpEntry(data, form));
                            }
                        }
                    }

                    if (data.Kind == LetterDataKind.DiacriticCombo) {
                        diacriticComboLookUpCache.Add(new DiacriticComboLookUpEntry(data.Symbol, data.BaseLetter), data);
                    }
                }
            }

            var result = new List<ArabicStringPart>();

            // If we used ArabicFixer, this char array will contain only combined unicodes
            char[] chars = processedArabicString.ToCharArray();

            int stringIndex = 0;
            for (int i = 0; i < chars.Length; i++) {
                char character = chars[i];

                // Skip spaces and arabic "?"
                if (character == ' ' || character == '؟') {
                    ++stringIndex;
                    continue;
                }

                string unicodeString = GetHexUnicodeFromChar(character);

                if (unicodeString == "0640") {
                    // it's an arabic tatweel
                    // just extends previous character
                    for (int t = result.Count - 1; t >= 0; --t) {
                        var previous = result[t];

                        if (previous.toCharacterIndex == stringIndex - 1) {
                            ++previous.toCharacterIndex;
                            result[t] = previous;
                        } else {
                            break;
                        }
                    }

                    ++stringIndex;
                    continue;
                }

                // Find the letter, and check its form
                LetterForm letterForm = LetterForm.None;
                LetterData letterData = null;

                UnicodeLookUpEntry entry;
                if (unicodeLookUpCache.TryGetValue(unicodeString, out entry)) {
                    letterForm = entry.form;
                    letterData = entry.data;
                }

                if (letterData != null) {
                    if (letterData.Kind == LetterDataKind.DiacriticCombo && separateDiacritics) {
                        // It's a diacritic combo
                        // Separate Letter and Diacritic
                        result.Add(new ArabicStringPart(staticDatabase.GetById(staticDatabase.GetLetterTable(), letterData.BaseLetter),
                            stringIndex, stringIndex, letterForm));
                        result.Add(new ArabicStringPart(staticDatabase.GetById(staticDatabase.GetLetterTable(), letterData.Symbol),
                            stringIndex, stringIndex, letterForm));
                    } else if (letterData.Kind == LetterDataKind.Symbol &&
                               letterData.Type == LetterDataType.DiacriticSymbol && !separateDiacritics) {
                        // It's a diacritic
                        // Merge Letter and Diacritic

                        var symbolId = letterData.Id;
                        var lastLetterData = result[result.Count - 1];
                        var baseLetterId = lastLetterData.letter.Id;

                        LetterData diacriticLetterData = null;

                        diacriticComboLookUpCache.TryGetValue(new DiacriticComboLookUpEntry(symbolId, baseLetterId), out diacriticLetterData);

                        if (diacriticLetterData == null) {
                            Debug.LogError("Cannot find a single character for " + baseLetterId + " + " + symbolId +
                                           ". Diacritic removed in (" + processedArabicString + ").");
                        } else {
                            var previous = result[result.Count - 1];
                            previous.letter = diacriticLetterData;
                            ++previous.toCharacterIndex;
                            result[result.Count - 1] = previous;
                        }
                    } else if (letterData.Kind == LetterDataKind.LetterVariation && separateVariations &&
                               letterData.BaseLetter == "lam") {
                        // it's a lam-alef combo
                        // Separate Lam and Alef
                        result.Add(new ArabicStringPart(staticDatabase.GetById(staticDatabase.GetLetterTable(), letterData.BaseLetter),
                            stringIndex, stringIndex, letterForm));

                        var secondPart = staticDatabase.GetById(staticDatabase.GetLetterTable(), letterData.Symbol);

                        if (secondPart.Kind == LetterDataKind.DiacriticCombo && separateDiacritics) {
                            // It's a diacritic combo
                            // Separate Letter and Diacritic
                            result.Add(new ArabicStringPart(staticDatabase.GetById(staticDatabase.GetLetterTable(), secondPart.BaseLetter),
                                stringIndex, stringIndex, letterForm));
                            result.Add(new ArabicStringPart(staticDatabase.GetById(staticDatabase.GetLetterTable(), secondPart.Symbol),
                                stringIndex, stringIndex, letterForm));
                        } else {
                            result.Add(new ArabicStringPart(secondPart, stringIndex, stringIndex, letterForm));
                        }
                    } else {
                        result.Add(new ArabicStringPart(letterData, stringIndex, stringIndex, letterForm));
                    }
                } else {
                    Debug.Log("Cannot parse letter " + character + " (" + unicodeString + ") in " + processedArabicString);
                }

                ++stringIndex;
            }

            return result;
        }

        #region Diacritic Fix

        private struct DiacriticComboEntry
        {
            public string Unicode1;
            public string Unicode2;

            public DiacriticComboEntry(string _unicode1, string _unicode2)
            {
                Unicode1 = _unicode1;
                Unicode2 = _unicode2;
            }
        }

        private static Dictionary<DiacriticComboEntry, Vector2> DiacriticCombos2Fix = null;

        /// <summary>
        /// these are manually configured positions of diacritic symbols relative to the main letter
        /// since TextMesh Pro can't manage these automatically and some letters are too tall, with the symbol overlapping 
        /// </summary>
        private static void BuildDiacriticCombos2Fix()
        {
            DiacriticCombos2Fix = new Dictionary<DiacriticComboEntry, Vector2>();

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0627", "064B"), new Vector2(0, 70));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE8E", "064B"), new Vector2(-20, 80));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0623", "064E"), new Vector2(0, 200));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE84", "064E"), new Vector2(-20, 200));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0639", "0650"), new Vector2(20, -300));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FECA", "0650"), new Vector2(20, -300));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0628", "0650"), new Vector2(130, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE91", "0650"), new Vector2(0, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE92", "0650"), new Vector2(0, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE90", "0650"), new Vector2(130, -120));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0630", "064E"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEAC", "064E"), new Vector2(0, 80));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED3", "064E"), new Vector2(200, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED4", "064E"), new Vector2(200, 50));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED3", "0652"), new Vector2(200, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED4", "0652"), new Vector2(200, 60));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEEC", "0650"), new Vector2(0, -120));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062D", "0650"), new Vector2(0, -350));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEA2", "0650"), new Vector2(0, -350));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDB", "064F"), new Vector2(0, 70));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDC", "064F"), new Vector2(0, 70));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEFB", "064B"), new Vector2(0, 70));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEFC", "064B"), new Vector2(0, 70));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDF", "064C"), new Vector2(40, 120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE0", "064C"), new Vector2(40, 120));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDF", "064E"), new Vector2(40, 120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE0", "064E"), new Vector2(40, 120));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0644", "0650"), new Vector2(0, -100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDE", "0650"), new Vector2(0, -100));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0644", "064D"), new Vector2(30, -90));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDE", "064D"), new Vector2(30, -90));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDF", "0652"), new Vector2(60, 140));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE0", "0652"), new Vector2(60, 140));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0645", "0650"), new Vector2(60, -100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE2", "0650"), new Vector2(60, -100));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0646", "064D"), new Vector2(40, -130));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE6", "064D"), new Vector2(40, -130));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE7", "0652"), new Vector2(30, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE8", "0652"), new Vector2(30, 50));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0642", "064E"), new Vector2(50, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED7", "064E"), new Vector2(0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED8", "064E"), new Vector2(0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED6", "064E"), new Vector2(50, 0));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0631", "0650"), new Vector2(50, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEAE", "0650"), new Vector2(50, -200));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0637", "064C"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC3", "064C"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC4", "064C"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC2", "064C"), new Vector2(0, 100));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0637", "064E"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC3", "064E"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC4", "064E"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC2", "064E"), new Vector2(0, 100));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062A", "064F"), new Vector2(60, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE97", "064F"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE98", "064F"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE96", "064F"), new Vector2(60, 0));
            // teh_dammah_tanwin
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062A", "064C"), new Vector2(60, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE97", "064C"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE98", "064C"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE96", "064C"), new Vector2(60, 0));
            // teh_fathah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062A", "064E"), new Vector2(70, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE96", "064E"), new Vector2(70, 0));
            // teh_marbuta_dammah_tanwin
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0629", "064C"), new Vector2(-10, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE94", "064C"), new Vector2(0, 80));
            // teh_marbuta_fathah_tanwin
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0629", "064B"), new Vector2(-10, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE94", "064B"), new Vector2(0, 80));
            // Diacritic Song
            // alef_hamza_hi_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0623", "064F"), new Vector2(-20, 130));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE84", "064F"), new Vector2(-20, 130));
            // alef_hamza_low_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0625", "0650"), new Vector2(0, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE88", "0650"), new Vector2(0, -120));
            // theh_fathah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062B", "064E"), new Vector2(60, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9B", "064E"), new Vector2(0, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9C", "064E"), new Vector2(0, 60));
            // theh_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062B", "064F"), new Vector2(60, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9B", "064F"), new Vector2(0, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9C", "064F"), new Vector2(0, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9A", "064F"), new Vector2(60, 40));
            // jeem_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062C", "0650"), new Vector2(50, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9F", "0650"), new Vector2(20, -90));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEA0", "0650"), new Vector2(20, -90));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9E", "0650"), new Vector2(50, -200));
            // khah_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062E", "0650"), new Vector2(50, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEA6", "0650"), new Vector2(50, -200));
            // thal_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0630", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEAC", "064F"), new Vector2(0, 80));
            // zain_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0632", "0650"), new Vector2(70, -180));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEB0", "0650"), new Vector2(70, -180));
            // seen_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0633", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEB2", "0650"), new Vector2(50, -120));
            // sheen_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0634", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEB6", "0650"), new Vector2(50, -120));
            // sad_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0635", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEBA", "0650"), new Vector2(50, -120));
            // dad_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0636", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEBE", "0650"), new Vector2(50, -120));
            // tah_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0637", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC3", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC4", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC2", "064F"), new Vector2(0, 80));
            // zah_fathah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0638", "064E"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC7", "064E"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC8", "064E"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC6", "064E"), new Vector2(0, 80));
            // zah_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0638", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC7", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC8", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC6", "064F"), new Vector2(0, 80));
            // ghain_fathah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("063A", "064E"), new Vector2(0, 50));
            // ghain_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("063A", "064F"), new Vector2(0, 50));
            // ghain_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("063A", "0650"), new Vector2(0, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FECE", "0650"), new Vector2(0, -200));
            // feh_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0641", "064F"), new Vector2(100, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED3", "064F"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED4", "064F"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED2", "064F"), new Vector2(100, 0));
            // qaf_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0642", "064F"), new Vector2(80, 20));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED7", "064F"), new Vector2(0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED8", "064F"), new Vector2(0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED6", "064F"), new Vector2(80, 20));
            // qaf_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0642", "0650"), new Vector2(50, -140));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED6", "0650"), new Vector2(50, -140));
            // lam_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0644", "064F"), new Vector2(80, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDF", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE0", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDE", "064F"), new Vector2(80, 40));
            // noon_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0646", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE6", "0650"), new Vector2(50, -120));
            // waw_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0648", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEEE", "0650"), new Vector2(50, -120));
            // yeh_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("064A", "0650"), new Vector2(120, -230));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEF2", "0650"), new Vector2(100, -260));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEF3", "0650"), new Vector2(0, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEF4", "0650"), new Vector2(0, -120));

            //List<LetterData> list = AppManager.I.DB.FindLetterData((x) => (x.Symbol_DeltaY != 0));
            //foreach (var letter in list) {
            //    DiacriticCombos2Fix.Add(new DiacriticComboEntry(letter.Isolated_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    if (letter.Initial_Unicode != "") {
            //        DiacriticCombos2Fix.Add(new DiacriticComboEntry(letter.Initial_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    }
            //    if (letter.Medial_Unicode != "") {
            //        DiacriticCombos2Fix.Add(new DiacriticComboEntry(letter.Medial_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    }
            //    if (letter.Final_Unicode != "") {
            //        DiacriticCombos2Fix.Add(new DiacriticComboEntry(letter.Final_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    }
            //}
        }

        private static Vector2 FindDiacriticCombo2Fix(string Unicode1, string Unicode2)
        {
            if (DiacriticCombos2Fix == null) { BuildDiacriticCombos2Fix(); }

            Vector2 newDelta = new Vector2(0, 0);
            DiacriticCombos2Fix.TryGetValue(new DiacriticComboEntry(Unicode1, Unicode2), out newDelta);
            return newDelta;
        }

        /// <summary>
        /// Adjusts the diacritic positions.
        /// </summary>
        /// <returns><c>true</c>, if diacritic positions was adjusted, <c>false</c> otherwise.</returns>
        /// <param name="textInfo">Text info.</param>
        public static bool FixTMProDiacriticPositions(TMPro.TMP_TextInfo textInfo)
        {
            //Debug.Log("FixDiacriticPositions " + textInfo.characterCount);
            int characterCount = textInfo.characterCount;
            bool changed = false;

            if (characterCount > 1) {
                // output unicodes for DiacriticCombos2Fix
                //string combo = "";
                //for (int i = 0; i < characterCount; i++) {
                //    combo += '"' + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[i].character) + '"' + ',';
                //}
                //Debug.Log("DiacriticCombos2Fix.Add(new DiacriticComboEntry(" + combo + " 0, 0));");

                Vector2 modificationDelta = new Vector2(0, 0);
                for (int charPosition = 0; charPosition < characterCount - 1; charPosition++) {
                    modificationDelta = FindDiacriticCombo2Fix(
                        GetHexUnicodeFromChar(textInfo.characterInfo[charPosition].character),
                        GetHexUnicodeFromChar(textInfo.characterInfo[charPosition + 1].character)
                    );

                    if (modificationDelta.sqrMagnitude > 0f) {
                        changed = true;
                        int materialIndex = textInfo.characterInfo[charPosition + 1].materialReferenceIndex;
                        int vertexIndex = textInfo.characterInfo[charPosition + 1].vertexIndex;
                        Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;

                        float charsize = (sourceVertices[vertexIndex + 2].y - sourceVertices[vertexIndex + 0].y);
                        float dx = charsize * modificationDelta.x / 100f;
                        float dy = charsize * modificationDelta.y / 100f;
                        Vector3 offset = new Vector3(dx, dy, 0f);

                        Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
                        destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + offset;
                        destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + offset;
                        destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + offset;
                        destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + offset;

                        //Debug.Log("DIACRITIC FIX: "
                        //          + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[charPosition].character)
                        //          + " + "
                        //          + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[charPosition + 1].character)
                        //          + " by " + modificationDelta);
                    }
                }
            }
            return changed;
        }

        #endregion
    }
}