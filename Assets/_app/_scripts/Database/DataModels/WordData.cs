﻿using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class WordData : IData
    {
        public string Id;
        public string Kind;
        public string Category;
        public string English;
        public string Arabic;
        public string[] Letters;
        public string Transliteration;
        public string DifficultyLevel;
        public string Group;

        // calculated during import
        public int NumberOfLetters;

        public string GetID()
        {
            return Id;
        }

        #region API

        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get { return ArabicAlphabetHelper.ParseWord(Arabic, AppManager.Instance.Letters); }
        }

        /// <summary>
        /// Return draw of word.
        /// </summary>
        public Sprite DrawForLivingLetter {
            get { return Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + Id); }
        }

        #endregion
    }
}