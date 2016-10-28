using System;
using UnityEngine;

namespace EA4S.Db
{
    public enum WordKind
    {
        Noun = 1,
        Adjective = 2,
        Verb = 3,
        Adverb = 4,
        Article = 5
    }

    public enum WordCategory
    {
        None = 0,
        Animal = 1,
        BodyPart = 2,
        Color = 3,
        FamilyMember = 4,
        Food = 5,
        Fruit = 6,
        Job = 7,
        Nature = 8,
        Number = 9,
        Place = 10,
        Sport = 11,
        Thing = 12,
        Time = 13,
        Vegetable = 14,
        Vehicle = 15
    }

    [Serializable]
    public class WordData : IData
    {
        public string Id;
        public WordKind Kind;
        public WordCategory Category;
        public string Arabic;
        public string[] Letters;
        public LetterSymbol[] Symbols; //TODO
        public int Difficulty;
        public int Drawing;

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

    }

    [Serializable]
    public struct LetterSymbol
    {
        public string LetterId;
        public string SymbolId;
    }
}