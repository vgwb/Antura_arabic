using System;

namespace EA4S.Db
{
    public enum LetterKind
    {
        Letter = 1,
        Special = 2,
        Symbol = 3,
        Diphtong = 4
    }
    public enum LetterType
    {
        None = 0,
        LongVowel = 1,
        Consonant = 2,
        Powerful = 3,
        DiacriticSymbol = 4,
        Variation = 5,
        Combination = 6
    }
    public enum LetterSunMoon
    {
        None = 0,
        Sun = 1,
        Moon = 2
    }

    [Serializable]
    public class LetterData : IData
    {
        public string Id;
        public int Number;
        public string Title;
        public LetterKind Kind;
        public LetterType Type;
        public string Notes;
        public LetterSunMoon SunMoon;
        public string Sound;
        public string Isolated;
        public string Initial;
        public string Medial;
        public string Final;
        public string Isolated_Unicode;
        public string Initial_Unicode;
        public string Medial_Unicode;
        public string Final_Unicode;

        public override string ToString()
        {
            return Id + ": " + Isolated;
        }

        public string GetId()
        {
            return Id;
        }

        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get { return ArabicAlphabetHelper.GetLetterFromUnicode(Isolated_Unicode); }
        }

    }
}