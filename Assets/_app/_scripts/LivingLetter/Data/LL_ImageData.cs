using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S
{
    public class LL_ImageData : ILivingLetterData
    {

        public Db.WordData Data;

        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Image; }
        }

        public string Id {
            get { return Data.Id; }
            set { Data = AppManager.I.DB.GetWordDataById(value); }
        }

        public LL_ImageData(string _id) : this(AppManager.I.DB.GetWordDataById(_id))
        {
        }

        public LL_ImageData(string _id, Db.WordData _data) : this(_data)
        {
        }

        public LL_ImageData(Db.WordData _data)
        {
            Data = _data;
        }

        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get { return ArabicAlphabetHelper.PrepareArabicStringForDisplay(Data.Arabic); }
        }

        public string DrawingCharForLivingLetter {
            get { return AppManager.I.Teacher.wordHelper.GetWordDrawing(Data); }
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