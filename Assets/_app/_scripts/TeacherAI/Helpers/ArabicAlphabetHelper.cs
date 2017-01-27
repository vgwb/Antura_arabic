using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using ArabicSupport;
using EA4S.MinigamesAPI;
using EA4S.Utilities;

namespace EA4S.Helpers
{
    // refactor: This should be among helpers and not be accessed through the TeacherAI.
    // refactor: We should create an intermediate layer for accessing language-specific helpers, so that they can be removed easily.
    // refactor: this class needs a large refactoring as it is used for several different purposes
    public static class ArabicAlphabetHelper
    {

        /// <summary>
        /// Prepares the string for display (say from Arabic into TMPro Text
        /// </summary>
        /// <returns>The string for display.</returns>
        /// <param name="">.</param>
        public static string PrepareArabicStringForDisplay(string str, bool reversed = true)
        {
            if (reversed)
            {
                // needed to be set in a TMPro RTL text
                return GenericUtilities.ReverseText(ArabicFixer.Fix(str, true, true));
            }
            return ArabicFixer.Fix(str, true, true);
        }

        // refactor: not used?
        public static void DebugLetter(Database.LetterData letterData)
        {
            byte[] bytesUtf16 = Encoding.Unicode.GetBytes(letterData.Isolated);
            foreach (var item in bytesUtf16)
            {
                Debug.Log("DebugLetter " + letterData.Id + " lenght: " + letterData.Isolated.Length + " - " + item);
            }
            // Encoding.Convert(Encoding.UTF8, Encoding.Unicode, encodedBytes);
            //Debug.Log("DebugLetter " + str + " lenght: " + str.Length);
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
        /// Returns the list of letters found in a word string
        /// </summary>
        public static List<Database.LetterData> SplitWordIntoLetters(Database.WordData arabicWord, bool reverseOrder = false, bool separateDiacritics = false)
        {
            List<Database.LetterData> allLetterData = new List<Database.LetterData>(AppManager.I.DB.StaticDatabase.GetLetterTable().GetValuesTyped());

            var returnList = new List<Database.LetterData>();

            char[] chars = arabicWord.Arabic.ToCharArray();
            if (reverseOrder)
                Array.Reverse(chars);

            //Debug.Log(arabicWord);
            for (int i = 0; i < chars.Length; i++)
            {
                char _char = chars[i];
                string unicodeString = GetHexUnicodeFromChar(_char);

                Database.LetterData letterData = allLetterData.Find(l => l.Isolated_Unicode == unicodeString);
                if (letterData != null)
                {
                    if (!separateDiacritics && letterData.Kind == Database.LetterDataKind.Symbol && letterData.Type == Database.LetterDataType.DiacriticSymbol)
                    {
                        var symbolId = letterData.Id;
                        var lastLetterData = allLetterData.Find(l => l.Id == returnList[returnList.Count - 1].Id);
                        var baseLetterId = lastLetterData.Id;
                        //Debug.Log(symbolId);
                        var diacriticLetterData = allLetterData.Find(l => l.Symbol == symbolId && l.BaseLetter == baseLetterId);
                        returnList.RemoveAt(returnList.Count - 1);
                        //Debug.Log(baseLetterId);
                        //Debug.Log(diacriticLetterData);
                        if (diacriticLetterData == null)
                        {
                            Debug.LogError("NULL " + baseLetterId + " + " + symbolId + ": we remove the diacritic for now.");
                            // returnList.Add(diacriticLetterData.Id);
                        }
                        else
                        {
                            returnList.Add(diacriticLetterData);
                        }
                    }
                    else
                    {
                        returnList.Add(letterData);
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// Returns the list of letters found in a word string
        /// </summary>
        public static List<string> ExtractLettersFromArabicWord(string arabicWord, Database.DatabaseObject db, bool reverseOrder = false, bool separateDiacritics = false)
        {
            List<Database.LetterData> allLetterData = new List<Database.LetterData>(db.GetLetterTable().GetValuesTyped());

            var returnList = new List<string>();

            char[] chars = arabicWord.ToCharArray();
            if (reverseOrder)
                Array.Reverse(chars);

            //Debug.Log(arabicWord);
            for (int i = 0; i < chars.Length; i++)
            {
                char _char = chars[i];
                string unicodeString = GetHexUnicodeFromChar(_char);

                Database.LetterData letterData = allLetterData.Find(l => l.Isolated_Unicode == unicodeString);
                if (letterData != null)
                {
                    if (!separateDiacritics && letterData.Kind == Database.LetterDataKind.Symbol && letterData.Type == Database.LetterDataType.DiacriticSymbol)
                    {
                        var symbolId = letterData.Id;
                        var lastLetterData = allLetterData.Find(l => l.Id == returnList[returnList.Count - 1]);
                        var baseLetterId = lastLetterData.Id;
                        //Debug.Log(symbolId);
                        var diacriticLetterData = allLetterData.Find(l => l.Symbol == symbolId && l.BaseLetter == baseLetterId);
                        returnList.RemoveAt(returnList.Count - 1);
                        //Debug.Log(baseLetterId);
                        //Debug.Log(diacriticLetterData);
                        if (diacriticLetterData == null)
                        {
                            Debug.LogError("NULL " + baseLetterId + " + " + symbolId + ": we remove the diacritic for now.");
                            // returnList.Add(diacriticLetterData.Id);
                        }
                        else
                        {
                            returnList.Add(diacriticLetterData.Id);
                        }
                    }
                    else
                    {
                        returnList.Add(letterData.Id);
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// Return list of letter data for any letter of param word.
        /// </summary>
        public static List<LL_LetterData> ExtractLetterDataFromArabicWord(string arabicWord)
        {
            var db = AppManager.I.DB.StaticDatabase;
            var lettersIds = ExtractLettersFromArabicWord(arabicWord, db);
            var returnList = new List<LL_LetterData>();
            foreach (var id in lettersIds)
            {
                var llLetterData = new LL_LetterData((Database.LetterData)db.GetLetterTable().GetValue(id));
                returnList.Add(llLetterData);
            }
            return returnList;
        }

        /// <summary>
        /// Return a string of a word without a character. Warning: the word is already reversed and fixed for rendering.
        /// This is mandatory since PrepareArabicStringForDisplay should be called before adding the "_" character.
        /// </summary>
        public static string GetWordWithMissingLetter(Database.WordData arabicWord, Database.LetterData letterToRemove, string removedLetterChar = "_")
        {
            var Letters = SplitWordIntoLetters(arabicWord);

            // Second attempt
            int charPosition = 0;
            bool found = false;
            for (int index = 0; index < Letters.Count; ++index)
            {
                if (Letters[index].Id == letterToRemove.Id)
                {
                    found = true;
                    break;
                }
                else
                    charPosition += Letters[index].GetChar().Length;
            }

            if (!found)
                return ArabicAlphabetHelper.PrepareArabicStringForDisplay(arabicWord.Arabic);

            string text = ArabicAlphabetHelper.PrepareArabicStringForDisplay(arabicWord.Arabic);
            text = text.Substring(0, charPosition) + removedLetterChar + text.Substring(charPosition + letterToRemove.GetChar().Length);

            /*
            

            for (int index = 0; index < Letters.Count; ++index)
            {
                Database.LetterPosition position = Database.LetterPosition.Isolated;

                if (Letters.Count > 0)
                {
                    if (index == 0)
                        position = Database.LetterPosition.Initial;
                    else if (index == Letters.Count - 1)
                        position = Database.LetterPosition.Final;
                    else
                        position = Database.LetterPosition.Medial;
                }

                if (Letters[index].Id == letterToRemove.Id)
                {
                    text = text + removedLetterChar;
                }
                else
                    text = text + Letters[index].GetChar(position);

            }
            */

            return text;
        }
    }
}