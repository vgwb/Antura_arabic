using System;
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
        public int Difficulty;
        public string Group;
        public int Drawing;

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

    }
}