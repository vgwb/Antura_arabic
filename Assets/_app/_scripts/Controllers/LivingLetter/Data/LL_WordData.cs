using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S
{
    public class LL_WordData : ILivingLetterData
    {

        public Db.WordData Data;

        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Word; }
        }

        public string Key {
            get { return key; }
            set { key = value; }
        }

        private string key;

        public LL_WordData(string _keyRow, Db.WordData _data)
        {
            Key = _keyRow;
            Data = _data;
        }

        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get { return ArabicAlphabetHelper.ParseWord(Data.Arabic, AppManager.Instance.Letters); }
        }

        /// <summary>
        /// Return draw of word.
        /// </summary>
        public Sprite DrawForLivingLetter {
            get { return Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + Key); }
        }

    }
}