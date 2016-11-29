using System;
using UnityEngine;

namespace EA4S
{

    public class LL_LetterData : ILivingLetterData
    {
        public Db.LetterData Data;
        public Db.LetterPosition Position = Db.LetterPosition.Isolated;

        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Letter; }
        }

        public string Id {
            get { return Data.Id; }
            set { Data = AppManager.I.DB.GetLetterDataById(value); }
        }

        public LL_LetterData(string _id) : this(AppManager.I.DB.GetLetterDataById(_id))
        {
        }

        public LL_LetterData(string _id, Db.LetterData _data) : this(_data)
        {
        }

        public LL_LetterData(Db.LetterData _data)
        {
            Data = _data;
        }

        #region API
        public string TextForLivingLetter {
            get
            {
                return Data.GetCharFixedForDisplay(Position);
            }
        }
        
        public string DrawingCharForLivingLetter {
            get { return null; }
        }

        [Obsolete("Use DrawingCharForLivingLetter instead of this.")]
        public Sprite DrawForLivingLetter {
            get { return null; }
        }

        public bool Equals(ILivingLetterData data)
        {
            LL_LetterData other = data as LL_LetterData;
            if (other == null)
                return false;

            return other.Data.Id == Data.Id && other.Position == Position;
        }
        #endregion
    }
}