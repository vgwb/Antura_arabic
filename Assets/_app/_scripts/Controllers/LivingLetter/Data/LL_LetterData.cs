using System;
using UnityEngine;

namespace EA4S
{

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
        public Db.LetterPosition ShowAs = Db.LetterPosition.Isolated;

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
                    case Db.LetterPosition.Initial:
                        return ArabicAlphabetHelper.GetLetterFromUnicode(Data.Initial_Unicode);
                    case Db.LetterPosition.Medial:
                        return ArabicAlphabetHelper.GetLetterFromUnicode(Data.Medial_Unicode);
                    case Db.LetterPosition.Final:
                        return ArabicAlphabetHelper.GetLetterFromUnicode(Data.Final_Unicode);
                    default:
                        return ArabicAlphabetHelper.GetLetterFromUnicode(Data.Isolated_Unicode);
                }
            }
        }

        public string DrawingCharForLivingLetter {
            get { return null; }
        }

        [Obsolete("Use DrawingCharForLivingLetter instead of this.")]
        public Sprite DrawForLivingLetter {
            get { return null; }
        }
        #endregion
    }
}