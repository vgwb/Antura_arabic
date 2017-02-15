using UnityEngine;
using System;
using System.Collections.Generic;
using ArabicSupport;

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
            if (hexCode == "")
            {
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
            text = text.Substring(0, partToRemove.fromCharacterIndex) + removedLetterChar + (toCharacterIndex >= text.Length - 1 ? "" : text.Substring(partToRemove.toCharacterIndex + 1));

            return text;
        }

        /// <summary>
        /// Find all the occurrences of "letterToFind" in "arabicWord"
        /// </summary>
        /// <returns>the list of occurrences</returns>
        public static List<ArabicStringPart> FindLetter(Database.WordData arabicWord, Database.LetterData letterToFind)
        {
            List<ArabicStringPart> result = new List<ArabicStringPart>();

            var parts = AnalyzeData(arabicWord);

            for (int i = 0, count = parts.Count; i < count; ++i)
            {
                if (parts[i].letter.Id == letterToFind.Id)
                {
                    result.Add(parts[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the list of letters found in a word string
        /// </summary>
        public static List<ArabicStringPart> AnalyzeData(Database.WordData arabicWord, bool separateDiacritics = false, bool separateVariations = true)
        {
            // Use ArabicFixer to deal only with combined unicodes
            return AnalyzeArabicString(ArabicFixer.Fix(arabicWord.Arabic, true, true), separateDiacritics, separateVariations);
        }

        /// <summary>
        /// Returns the list of letters found in a word string
        /// </summary>
        public static List<ArabicStringPart> AnalyzeData(Database.PhraseData phrase, bool separateDiacritics = false, bool separateVariations = true)
        {
            // Use ArabicFixer to deal only with combined unicodes
            return AnalyzeArabicString(ArabicFixer.Fix(phrase.Arabic, true, true), separateDiacritics, separateVariations);
        }

        static List<ArabicStringPart> AnalyzeArabicString(string arabicString, bool separateDiacritics = false, bool separateVariations = true)
        {
            List<Database.LetterData> allLetterData = new List<Database.LetterData>(AppManager.I.DB.StaticDatabase.GetLetterTable().GetValuesTyped());

            var result = new List<ArabicStringPart>();

            // If we used ArabicFixer, this char array will contain only combined unicodes
            char[] chars = arabicString.ToCharArray();

            int stringIndex = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                ++stringIndex;

                char character = chars[i];

                // Skip spaces
                if (character == ' ')
                    continue;

                string unicodeString = GetHexUnicodeFromChar(character);

                if (unicodeString == "0640") // arabic tatweel
                {
                    // just extends previous character
                    if (result.Count > 0)
                    {
                        var previous = result[result.Count - 1];
                        ++previous.toCharacterIndex;
                        result[result.Count - 1] = previous;
                    }

                    continue;
                }

                // Find the letter, and check its form
                Database.LetterForm letterForm = Database.LetterForm.None;
                Database.LetterData letterData = null;
                for (int l = 0; l < allLetterData.Count; ++l)
                {
                    var data = allLetterData[l];
                    if (data.Isolated_Unicode == unicodeString)
                    {
                        letterForm = Database.LetterForm.Isolated;
                        letterData = data;
                        break;
                    }
                    else if (data.Initial_Unicode == unicodeString)
                    {
                        letterForm = Database.LetterForm.Initial;
                        letterData = data;
                        break;
                    }
                    else if (data.Medial_Unicode == unicodeString)
                    {
                        letterForm = Database.LetterForm.Medial;
                        letterData = data;
                        break;
                    }
                    else if (data.Final_Unicode == unicodeString)
                    {
                        letterForm = Database.LetterForm.Final;
                        letterData = data;
                        break;
                    }
                }

                if (letterData != null)
                {
                    if (letterData.Kind == Database.LetterDataKind.DiacriticCombo && separateDiacritics)
                    {
                        // Separate Letter and Diacritic
                        result.Add(new ArabicStringPart(AppManager.I.DB.GetLetterDataById(letterData.BaseLetter), stringIndex, stringIndex, letterForm));
                        result.Add(new ArabicStringPart(AppManager.I.DB.GetLetterDataById(letterData.Symbol), stringIndex, stringIndex, letterForm));
                    }
                    else if (letterData.Kind == Database.LetterDataKind.LetterVariation && separateVariations && letterData.BaseLetter == "lam")
                    {
                        // Separate Lam and Alef
                        result.Add(new ArabicStringPart(AppManager.I.DB.GetLetterDataById(letterData.BaseLetter), stringIndex, stringIndex, letterForm));
                        result.Add(new ArabicStringPart(AppManager.I.DB.GetLetterDataById(letterData.Symbol), stringIndex, stringIndex, letterForm));
                    }
                    else
                        result.Add(new ArabicStringPart(letterData, stringIndex, stringIndex, letterForm));
                }
                else
                    Debug.Log("Cannot parse letter " + character + " (" + unicodeString + ") in " + arabicString);
            }

            return result;
        }
    }
}