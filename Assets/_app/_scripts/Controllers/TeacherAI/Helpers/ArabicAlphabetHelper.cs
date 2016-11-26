using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using ArabicSupport;

namespace EA4S
{
    public static class ArabicAlphabetHelper
    {

        /// <summary>
        /// Prepares the string for display (say from Arabic into TMPro Text
        /// </summary>
        /// <returns>The string for display.</returns>
        /// <param name="">.</param>
        public static string PrepareArabicStringForDisplay(string str, bool reversed = true)
        {
            if (reversed) {
                // needed to be set in a TMPro RTL text
                return GenericUtilities.ReverseText(ArabicFixer.Fix(str, true, true));
            }
            return ArabicFixer.Fix(str, true, true);
        }

        /// <summary>
        /// Return single letter string
        /// </summary>
        public static string GetLetterToDisplay(Db.LetterData letterData, Db.LetterPosition letterPosition = Db.LetterPosition.Isolated)
        {
            //DebugLetter(letterData);
            if (letterData.Kind == Db.LetterDataKind.Letter) {
                return GetLetterFromUnicode(letterData.GetUnicode(letterPosition));
            } else {
                return letterData.GetChar(letterPosition);
            }
        }

        public static void DebugLetter(Db.LetterData letterData)
        {
            byte[] bytesUtf16 = Encoding.Unicode.GetBytes(letterData.Isolated);
            foreach (var item in bytesUtf16) {
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
            if (hexCode == "") {
                Debug.LogError("Letter requested with an empty hexacode (data is probably missing from the DataBase). Returning - for now.");
                hexCode = "002D";
            }

            int unicode = int.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);
            var character = (char)unicode;
            return character.ToString();
        }

        public static char GetCharFromUnicode(string hexCode)
        {
            if (hexCode == "") {
                Debug.LogError("Letter requested with an empty hexacode (data is probably missing from the DataBase). Returning - for now.");
                hexCode = "002D";
            }

            int unicode = int.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);
            return (char)unicode;
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
        /// <param name="word"></param>
        /// <param name="alphabet"></param>
        /// <param name="reverseOrder">Return in list position 0 most right letter in input string and last the most left.</param>
        /// <returns></returns>
        public static List<string> LetterDataList(string word, List<Db.LetterData> alphabet, bool reverseOrder = false)
        {
            var returnList = new List<string>();

            char[] chars = word.ToCharArray();
            if (reverseOrder) {
                Array.Reverse(chars);
            }

            for (int i = 0; i < chars.Length; i++) {
                char _char = chars[i];
                string unicodeString = GetHexUnicodeFromChar(_char);
                Db.LetterData letterData = alphabet.Find(l => l.Isolated_Unicode == unicodeString);
                if (letterData != null)
                    returnList.Add(letterData.Id);
            }

            return returnList;
        }


        /// <summary>
        /// Return list of letter data for any letter of param word.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="alphabet"></param>
        /// <param name="reverseOrder">Return in list position 0 most right letter in input string and last the most left.</param>
        /// <returns></returns>
        public static List<LL_LetterData> LetterDataListFromWord(string word, List<LL_LetterData> alphabet, bool reverseOrder = false)
        {
            var returnList = new List<LL_LetterData>();

            char[] chars = word.ToCharArray();
            if (reverseOrder)
                Array.Reverse(chars);
            for (int i = 0; i < chars.Length; i++) {
                char _char = chars[i];
                string unicodeString = GetHexUnicodeFromChar(_char);
                LL_LetterData letterData = alphabet.Find(l => l.Data.Isolated_Unicode == unicodeString);
                if (letterData != null)
                    returnList.Add(letterData);
            }
            return returnList;
        }

    }
}