using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class WordData : IData
    {
        public string Id { get; set; }
        public string Kind { get; set; }
        public string Category { get; set; }
        public string English { get; set; }
        public string Arabic { get; set; }
        public string[] Letters { get; set; }
        public string Transliteration { get; set; }
        public int Difficulty { get; set; }
        public string Group { get; set; }
        public int Drawing { get; set; }

        public int NumberOfLetters { get { return Letters.Length; } }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}",
                Id,
                Kind,
                Category,
                English,
                Arabic,
                Transliteration
                );
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