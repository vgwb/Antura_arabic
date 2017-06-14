using UnityEngine;
using System;
using EA4S.Core;
using EA4S.Helpers;

namespace EA4S.MinigamesAPI
{
    /// <summary>
    /// View of a PhraseData shown as text on a LivingLetter.
    /// </summary>
    // refactor: rename to better indicate that this is a view
    public class LL_WordData : ILivingLetterData
    {

        public Database.WordData Data;

        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Word; }
        }

        public string Id {
            get { return Data.Id; }
            set { Data = AppManager.Instance.DB.GetWordDataById(value); } // refactor: inject the value, no reference to the DB
        }

        public LL_WordData(string _id) : this(AppManager.Instance.DB.GetWordDataById(_id)) // refactor: inject the value, no reference to the DB
        {
        }

        public LL_WordData(string _id, Database.WordData _data) : this(_data)
        {
        }

        public LL_WordData(Database.WordData _data)
        {
            Data = _data;
        }

        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get {
                return ArabicAlphabetHelper.ProcessArabicString(Data.Arabic); // refactor: remove reference to Arabic
            }
        }

        public string DrawingCharForLivingLetter {
            get { return AppManager.Instance.VocabularyHelper.GetWordDrawing(Data); } // refactor: inject the value, no reference to the DB
        }

        /// <summary>
        /// Return draw of word.
        /// </summary>
        [Obsolete("Use DrawingCharForLivingLetter instead of this.")]
        public Sprite DrawForLivingLetter {
            get { return Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + Id); }
        }

        public bool Equals(ILivingLetterData data)
        {
            LL_WordData other = data as LL_WordData;
            if (other == null)
                return false;

            return other.Data.Id == Data.Id;
        }
    }
}