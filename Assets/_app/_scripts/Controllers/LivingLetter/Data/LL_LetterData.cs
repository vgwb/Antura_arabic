using System;
using UnityEngine;

namespace EA4S
{
    public enum LetterDataForm : int
    {
        ISOLATED,
        INITIAL,
        MEDIAL,
        FINAL
    }

    public class LL_LetterData : ILivingLetterData
    {
        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Letter; }
        }

        private string key;
        public string Key {
            get { return key; }
            set { key = value; }
        }

        public Db.LetterData Data;

        public LetterDataForm ShowAs = LetterDataForm.ISOLATED;

        protected void chargeLetterData(string _keyRow, Db.LetterData _letRow)
        {
            Key = _keyRow;
            Data = _letRow;
        }

        public LL_LetterData(string _keyRow)
        {
            chargeLetterData(_keyRow, AppManager.Instance.DB.GetLetterDataById(_keyRow));

        }

        #region API
        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get {
                switch (ShowAs) {
                    case LetterDataForm.INITIAL:
                        return ArabicAlphabetHelper.GetLetterFromUnicode(Data.Initial_Unicode);
                    case LetterDataForm.MEDIAL:
                        return ArabicAlphabetHelper.GetLetterFromUnicode(Data.Medial_Unicode);
                    case LetterDataForm.FINAL:
                        return ArabicAlphabetHelper.GetLetterFromUnicode(Data.Final_Unicode);
                    default:
                        return ArabicAlphabetHelper.GetLetterFromUnicode(Data.Isolated_Unicode);
                }
            }
        }

        public Sprite DrawForLivingLetter {
            get { return null; }
        }
        #endregion
    }
}