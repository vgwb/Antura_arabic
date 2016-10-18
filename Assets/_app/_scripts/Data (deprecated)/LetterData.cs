using System;
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

        protected void chargeLetterData(string _keyRow, Db.LetterData _letRow)
        {
            Key = _keyRow;
            Isolated = _letRow.Isolated;
            Isolated_Unicode = _letRow.Isolated_Unicode;
            Initial_Unicode = _letRow.Initial_Unicode;
            Medial_Unicode = _letRow.Medial_Unicode;
            Final_Unicode = _letRow.Final_Unicode;
        }

        public LetterData(string _keyRow) {
            chargeLetterData(_keyRow, AppManager.Instance.DB.GetLetterDataById(_keyRow));
            
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