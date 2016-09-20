using System;
using Google2u;
using UnityEngine;

namespace EA4S
{

    public class LetterData : ILivingLetterData
    {
        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Letter; }
        }

        public string Key {
            get { return key; }
            set { key = value; }
        }
        public string Isolated_Unicode;
        public string Isolated;
        public string Initial_Unicode;
        public string Medial_Unicode;
        public string Final_Unicode;

        private string key;

        public LetterData(string _keyRow, lettersRow _letRow)
        {
            Key = _keyRow;
            Isolated = _letRow._isolated;
            Isolated_Unicode = _letRow._unicode;
            Initial_Unicode = _letRow._initial_unicode;
            Medial_Unicode = _letRow._medial_unicode;
            Final_Unicode = _letRow._final_unicode;
        }

        #region API
        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get { return ArabicAlphabetHelper.GetLetterFromUnicode(Isolated_Unicode); }
        }

        public Sprite DrawForLivingLetter {
            get { return null; }
        }
        #endregion
    }
}