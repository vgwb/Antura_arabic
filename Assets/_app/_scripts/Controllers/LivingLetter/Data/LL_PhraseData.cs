using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S
{
    public class LL_PhraseData : ILivingLetterData
    {

        public Db.PhraseData Data;

        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Phrase; }
        }

        public string Id {
            get { return Data.Id; }
            set { Data = AppManager.Instance.DB.GetPhraseDataById(value); }
        }

        public LL_PhraseData(string _id) : this(_id, AppManager.Instance.DB.GetPhraseDataById(_id))
        {
        }

        public LL_PhraseData(string _id, Db.PhraseData _data) : this(_data)
        {
        }

        public LL_PhraseData(Db.PhraseData _data)
        {
            Data = _data;
        }

        /// <summary>
        /// @note Not ready yet!
        /// Living Letter Phrase Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get {
                return ArabicAlphabetHelper.PrepareArabicStringForDisplay(Data.Arabic);
            }
        }

        public string DrawingCharForLivingLetter {
            get {
                return null;
            }
        }

        /// <summary>
        /// Return draw of word.
        /// </summary>
        [Obsolete("Use DrawingCharForLivingLetter instead of this.")]
        public Sprite DrawForLivingLetter {
            get { return null; }
        }

    }
}