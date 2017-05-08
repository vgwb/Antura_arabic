using UnityEngine;
using System;
using System.Collections.Generic;
using ArabicSupport;
using EA4S.Database;

namespace EA4S.Helpers
{
    // refactor: We should create an intermediate layer for accessing language-specific helpers, so that they can be removed easily.
    // refactor: this class needs a large refactoring as it is used for several different purposes
    public static class ArabicAlphabetHelper
    {
        public struct ArabicStringPart
        {
            public Database.LetterData letter;
            public int fromCharacterIndex;
            public int toCharacterIndex;
            public Database.LetterForm letterForm;

            public ArabicStringPart(Database.LetterData letter, int fromCharacterIndex, int toCharacterIndex, Database.LetterForm letterForm)
            {
                this.letter = letter;
                this.fromCharacterIndex = fromCharacterIndex;
                this.toCharacterIndex = toCharacterIndex;
                this.letterForm = letterForm;
            }
        }

        struct UnicodeLookUpEntry
        {
            public Database.LetterData data;
            public Database.LetterForm form;

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

        static List<Database.LetterData> allLetterData;
        static Dictionary<string, UnicodeLookUpEntry> unicodeLookUpCache = new Dictionary<string, UnicodeLookUpEntry>();
        static Dictionary<DiacriticComboLookUpEntry, LetterData> diacriticComboLookUpCache = new Dictionary<DiacriticComboLookUpEntry, LetterData>();

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
                Debug.LogError("Letter requested with an empty hexacode (data is probably missing from the DataBase). Returning - for now.");
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
        public static string GetWordWithMissingLetterText(Database.WordData arabicWord, ArabicStringPart partToRemove, string removedLetterChar = "_")
        {
            string text = ProcessArabicString(arabicWord.Arabic);

            int toCharacterIndex = partToRemove.toCharacterIndex + 1;
            text = text.Substring(0, partToRemove.fromCharacterIndex) + removedLetterChar + (toCharacterIndex >= text.Length ? "" : text.Substring(toCharacterIndex));

            return text;
        }

        /// <summary>
        /// Find all the occurrences of "letterToFind" in "arabicWord"
        /// </summary>
        /// <returns>the list of occurrences</returns>
        public static List<ArabicStringPart> FindLetter(DatabaseManager database, Database.WordData arabicWord, Database.LetterData letterToFind)
        {
            List<ArabicStringPart> result = new List<ArabicStringPart>();

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
        public static List<ArabicStringPart> AnalyzeData(DatabaseManager database, WordData arabicWord, bool separateDiacritics = false, bool separateVariations = true)
        {
            // Use ArabicFixer to deal only with combined unicodes
            return AnalyzeArabicString(database.StaticDatabase, ProcessArabicString(arabicWord.Arabic), separateDiacritics, separateVariations);
        }
        public static List<ArabicStringPart> AnalyzeData(DatabaseObject staticDatabase, WordData arabicWord, bool separateDiacritics = false, bool separateVariations = true)
        {
            // Use ArabicFixer to deal only with combined unicodes
            return AnalyzeArabicString(staticDatabase, ProcessArabicString(arabicWord.Arabic), separateDiacritics, separateVariations);
        }

        /// <summary>
        /// Returns the list of letters found in a word string
        /// </summary>
        public static List<ArabicStringPart> AnalyzeData(DatabaseManager database, PhraseData phrase, bool separateDiacritics = false, bool separateVariations = true)
        {
            // Use ArabicFixer to deal only with combined unicodes
            return AnalyzeArabicString(database.StaticDatabase, ProcessArabicString(phrase.Arabic), separateDiacritics, separateVariations);
        }
        public static List<ArabicStringPart> AnalyzeData(DatabaseObject staticDatabase, PhraseData phrase, bool separateDiacritics = false, bool separateVariations = true)
        {
            // Use ArabicFixer to deal only with combined unicodes
            return AnalyzeArabicString(staticDatabase, ProcessArabicString(phrase.Arabic), separateDiacritics, separateVariations);
        }

        static List<ArabicStringPart> AnalyzeArabicString(DatabaseObject staticDatabase, string processedArabicString, bool separateDiacritics = false, bool separateVariations = true)
        {
            if (allLetterData == null) {
                allLetterData = new List<Database.LetterData>(staticDatabase.GetLetterTable().GetValuesTyped());

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
                Database.LetterForm letterForm = Database.LetterForm.None;
                Database.LetterData letterData = null;

                UnicodeLookUpEntry entry;
                if (unicodeLookUpCache.TryGetValue(unicodeString, out entry)) {
                    letterForm = entry.form;
                    letterData = entry.data;
                }

                if (letterData != null) {
                    if (letterData.Kind == Database.LetterDataKind.DiacriticCombo && separateDiacritics) {
                        // It's a diacritic combo
                        // Separate Letter and Diacritic
                        result.Add(new ArabicStringPart(staticDatabase.GetById(staticDatabase.GetLetterTable(), letterData.BaseLetter), stringIndex, stringIndex, letterForm));
                        result.Add(new ArabicStringPart(staticDatabase.GetById(staticDatabase.GetLetterTable(), letterData.Symbol), stringIndex, stringIndex, letterForm));
                    } else if (letterData.Kind == Database.LetterDataKind.Symbol && letterData.Type == Database.LetterDataType.DiacriticSymbol && !separateDiacritics) {
                        // It's a diacritic
                        // Merge Letter and Diacritic

                        var symbolId = letterData.Id;
                        var lastLetterData = result[result.Count - 1];
                        var baseLetterId = lastLetterData.letter.Id;

                        LetterData diacriticLetterData = null;

                        diacriticComboLookUpCache.TryGetValue(new DiacriticComboLookUpEntry(symbolId, baseLetterId), out diacriticLetterData);

                        if (diacriticLetterData == null) {
                            Debug.LogError("Cannot find a single character for " + baseLetterId + " + " + symbolId + ". Diacritic removed in (" + processedArabicString + ").");
                        } else {
                            var previous = result[result.Count - 1];
                            previous.letter = diacriticLetterData;
                            ++previous.toCharacterIndex;
                            result[result.Count - 1] = previous;
                        }
                    } else if (letterData.Kind == Database.LetterDataKind.LetterVariation && separateVariations && letterData.BaseLetter == "lam") {
                        // it's a lam-alef combo
                        // Separate Lam and Alef
                        result.Add(new ArabicStringPart(staticDatabase.GetById(staticDatabase.GetLetterTable(), letterData.BaseLetter), stringIndex, stringIndex, letterForm));

                        var secondPart = staticDatabase.GetById(staticDatabase.GetLetterTable(), letterData.Symbol);

                        if (secondPart.Kind == Database.LetterDataKind.DiacriticCombo && separateDiacritics) {
                            // It's a diacritic combo
                            // Separate Letter and Diacritic
                            result.Add(new ArabicStringPart(staticDatabase.GetById(staticDatabase.GetLetterTable(), secondPart.BaseLetter), stringIndex, stringIndex, letterForm));
                            result.Add(new ArabicStringPart(staticDatabase.GetById(staticDatabase.GetLetterTable(), secondPart.Symbol), stringIndex, stringIndex, letterForm));
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
    }
}