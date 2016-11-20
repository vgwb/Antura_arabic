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

        string id;
        public string Id {
            get { return id; }
            set { id = value; }
        }

        public LL_ImageData(string _id, Db.WordData _data)
        {
            Id = _id;
            Data = _data;
        }

        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get { return ArabicAlphabetHelper.ParseWord(Data.Arabic, AppManager.Instance.Teacher.GetAllTestLetterDataLL()); }
        }

        public string DrawingCharForLivingLetter {
            get { return AppManager.Instance.Teacher.wordHelper.GetWordDrawing(Data); }
        }

        /// <summary>
        /// Return draw of word.
        /// </summary>
        [Obsolete("Use DrawingCharForLivingLetter instead of this.")]
        public Sprite DrawForLivingLetter {
            get { return Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + Id); }
        }

    }
}