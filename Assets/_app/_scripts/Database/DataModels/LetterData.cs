using System;

namespace EA4S.Db
{
    [Serializable]
    public class LetterData : IData
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Kind { get; set; }
        public string Type { get; set; }
        public string Notes { get; set; }
        public string SunMoon { get; set; }
        public string Sound { get; set; }
        public string Isolated { get; set; }
        public string Initial { get; set; }
        public string Medial { get; set; }
        public string Final { get; set; }
        public string Isolated_Unicode { get; set; }
        public string Initial_Unicode { get; set; }
        public string Medial_Unicode { get; set; }
        public string Final_Unicode { get; set; }

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