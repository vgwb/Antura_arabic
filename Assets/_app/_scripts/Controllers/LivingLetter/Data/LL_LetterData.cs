using System;
using UnityEngine;

namespace EA4S
{

    public class LL_LetterData : ILivingLetterData
    {
        public Db.LetterData Data;
        public Db.LetterPosition ShowAs = Db.LetterPosition.Isolated;

        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Letter; }
        }

        string id;
        public string Id {
            get { return id; }
            set { id = value; }
        }

        public LL_LetterData(string _id) : this(_id, AppManager.Instance.DB.GetLetterDataById(_id))
        {
        }

        public LL_LetterData(string _id, Db.LetterData _data)
        {
            Id = _id;
            Data = _data;
        }

        #region API
        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get {
                return ArabicAlphabetHelper.GetLetterToDisplay(Data, ShowAs);
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