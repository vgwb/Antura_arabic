using UnityEngine;
using System.Collections.Generic;
using System;

namespace CGL.Antura {
    public static class ArabicAlphabetHelper {

        /// <summary>
        /// Return single letter string start from unicode hexa code.
        /// </summary>
        /// <param name="_hexaCode"></param>
        /// <returns></returns>
        public static string GetLetterFromUnicode(string _hexaCode) {
            int unicode = int.Parse(_hexaCode, System.Globalization.NumberStyles.HexNumber);
            char character = (char)unicode;
            string text = character.ToString();
            return text;
        }

        /// <summary>
        /// Get char hexa code.
        /// </summary>
        /// <param name="_c"></param>
        /// <param name="_forConversion"></param>
        /// <returns></returns>
        public static string GetHexaUnicodeFromChar(char _c, bool _forConversion = false) {
            return string.Format("{1}{0:X4}", Convert.ToUInt16(_c), _forConversion ? "/U" : string.Empty);
        }

        /// <summary>
        /// Return list of letter data for any letter of param word.
        /// </summary>
        /// <param name="_word"></param>
        /// <returns></returns>
        public static List<LetterData> LetterDataListFromWord(string _word, List<LetterData> _vocabulary){
            List<LetterData> returnList = new List<LetterData>();

            char[] chars = _word.ToCharArray();
            for (int i = 0; i < chars.Length; i++) {
                char ch = chars[i];
                string unicodeString = GetHexaUnicodeFromChar(ch);
                LetterData let = _vocabulary.Find(l => l.Isolated_Unicode == unicodeString);
                if(let != null)
                    returnList.Add(let);
            }
            return returnList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_word"></param>
        /// <returns></returns>
        public static string ParseWord(string _word, List<LetterData> _vocabulary) {
            string returnString = string.Empty;
            List<LetterData> letters = LetterDataListFromWord(_word, _vocabulary);
            if (letters.Count == 1)
                return returnString = letters[0].Isolated;
            for (int i = 0; i < letters.Count; i++) {
                LetterData let = letters[i];
                if (let != null) {
                    if (i == 0) {
                        returnString += ArabicAlphabetHelper.GetLetterFromUnicode(let.Initial_Unicode);
                        continue;
                    } else if (i == letters.Count - 1) {
                        returnString += ArabicAlphabetHelper.GetLetterFromUnicode(let.Final_Unicode);
                        continue;
                    } else {
                        returnString += ArabicAlphabetHelper.GetLetterFromUnicode(let.Medial_Unicode);
                        continue;
                    }
                } else {
                    returnString += string.Format("{0}{2}{1}", "<color=red>", "</color>", ArabicAlphabetHelper.GetLetterFromUnicode(let.Isolated_Unicode));
                }
            }
            return returnString;
        }
    }
}