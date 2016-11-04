using System;

namespace EA4S.Db
{
    [Serializable]
    public class LetterData : IData
    {
        public string Id;
        public int Number;
        public string Title;
        public LetterDataKind Kind;
        public string BaseLetter;
        public string Symbol;
        public LetterDataType Type;
        public string Tag;
        public string Notes;
        public LetterDataSunMoon SunMoon;
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