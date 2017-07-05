using System;
using Antura.Core;
using UnityEngine;

namespace Antura.MinigamesAPI
{
    /// <summary>
    /// View of a LetterData shown as a single letter on a LivingLetter.
    /// </summary>
    // TODO refactor: rename to better indicate that this is a view
    public class LL_LetterData : ILivingLetterData
    {
        public Database.LetterData Data;
        public Database.LetterForm Form = Database.LetterForm.Isolated; // TODO refactor: this is tied to the Arabic language

        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Letter; }
        }

        public string Id {
            get { return Data.Id; }
            set { Data = AppManager.I.DB.GetLetterDataById(value); }
        }

        public LL_LetterData(string _id) : this(AppManager.I.DB.GetLetterDataById(_id)) /// TODO refactor: inject the value, no reference to the DB
        {
        }

        public LL_LetterData(Database.LetterData _data)
        {
            Data = _data;
        }

        public LL_LetterData(string _id, Database.LetterForm form) : this(AppManager.I.DB.GetLetterDataById(_id), form)
        {
        }

        public LL_LetterData(Database.LetterData _data, Database.LetterForm form)
        {
            Data = _data;
            Form = form;
        }

        #region API
        public string TextForLivingLetter {
            get
            {
                return Data.GetCharFixedForDisplay(Form);
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

            return other.Data.Id == Data.Id && other.Form == Form;
        }
        #endregion
    }
}