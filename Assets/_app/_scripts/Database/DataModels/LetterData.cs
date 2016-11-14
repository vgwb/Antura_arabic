using System;

namespace EA4S.Db
{
    public enum LetterKindCategory
    {
        Real = 0,   // default: Base + Combo
        Combo,
        Base
    }

    [Serializable]
    public class LetterData : IData, IConvertibleToLivingLetterData
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


        public bool IsOfKindCategory(LetterKindCategory category)
        {
            bool isIt = false;
            switch (category)
            {
                case LetterKindCategory.Base:
                    isIt = IsBaseLetter();
                    break;
                case LetterKindCategory.Combo:
                    isIt = IsComboLetter(); 
                    break;
                case LetterKindCategory.Real:
                    isIt = IsRealLetter();
                    break;
            }
            return isIt;
        }

        private bool IsRealLetter()
        {
            return this.IsBaseLetter() || this.IsComboLetter();
        }

        private bool IsBaseLetter()
        {
            return this.Kind == LetterDataKind.Letter;
        }

        private bool IsComboLetter()
        {
            return this.Kind == LetterDataKind.Combination || this.Kind == LetterDataKind.LetterVariation;
        }

        public ILivingLetterData ConvertToLivingLetterData()
        {
            return new LL_LetterData(GetId(), this);
        }

    }
}