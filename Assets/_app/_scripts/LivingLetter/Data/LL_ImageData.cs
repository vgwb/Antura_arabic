using System;
using EA4S.Core;
using EA4S.Helpers;
using UnityEngine;

namespace EA4S.MinigamesAPI
{
    /// <summary>
    /// View of a WordData shown as a drawing or image on a LivingLetter.
    /// </summary>
    // refactor: rename to better indicate that this is a view
    public class LL_ImageData : ILivingLetterData
    {

        public Database.WordData Data;

        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Image; }
        }

        public string Id {
            get { return Data.Id; }
            set { Data = (AppManager.Instance as AppManager).DB.GetWordDataById(value); }  // refactor: inject the value, no reference to the DB
        }

        public LL_ImageData(string _id) : this((AppManager.Instance as AppManager).DB.GetWordDataById(_id))  // refactor: inject the value, no reference to the DB
        {
        }

        public LL_ImageData(string _id, Database.WordData _data) : this(_data)
        {
        }

        public LL_ImageData(Database.WordData _data)
        {
            Data = _data;
        }

        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get { return ArabicAlphabetHelper.ProcessArabicString(Data.Arabic); }  // refactor: remove reference to the Arabic language
        }

        public string DrawingCharForLivingLetter {
            get { return (AppManager.Instance as AppManager).VocabularyHelper.GetWordDrawing(Data); }  // refactor: inject the value, no reference to the DB
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
            LL_ImageData other = data as LL_ImageData;
            if (other == null)
                return false;

            return other.Data.Id == Data.Id;
        }
    }
}