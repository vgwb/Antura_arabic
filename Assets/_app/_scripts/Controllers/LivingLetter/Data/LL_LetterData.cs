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

        public LL_LetterData(string _keyRow) : this(_keyRow, AppManager.Instance.DB.GetLetterDataById(_keyRow))
        {
        }

        public LL_LetterData(string _key, Db.LetterData _data)
        {
            Key = _key;
            Data = _data;
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

        public string DrawingCharForLivingLetter {
            get { return null; }
        }

        public Sprite DrawForLivingLetter {
            get { return null; }
        }
        #endregion
    }
}