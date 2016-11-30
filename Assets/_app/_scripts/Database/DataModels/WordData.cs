using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class WordData : IData, IConvertibleToLivingLetterData
    {
        public string Id;
        public bool Active;
        public WordDataKind Kind;
        public WordDataCategory Category;
        public WordDataForm Form;
        public WordDataArticle Article;
        public string LinkedWord;
        public string Arabic;
        public string Value;
        public string[] Letters;
        //public LetterSymbol[] Symbols; //TODO
        public int Difficulty;
        public string Drawing;

        public int NumberOfLetters { get { return Letters.Length; } }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}",
                Id,
                Kind,
                Category,
                Arabic
                );
        }

        public ILivingLetterData ConvertToLivingLetterData()
        {
            return new LL_WordData(GetId(), this);
        }

        public bool HasDrawing()
        {
            return Drawing != "";
        }

    }

    /*[Serializable]
    public struct LetterSymbol
    {
        public string LetterId;
        public string SymbolId;
    }*/
}