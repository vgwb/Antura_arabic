using System;

namespace EA4S.Db
{
    public enum LetterKindCategory
    {
        Real = 0,   // default: Base + Combo
        Combo,
        Base,
        BaseAndVariations
    }

    public enum LetterPosition
    {
        Isolated = 0,
        Initial = 1,
        Medial = 2,
        Final = 3
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
        public string SoundZone;
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
            switch (category) {
                case LetterKindCategory.Base:
                    isIt = IsBaseLetter();
                    break;
                case LetterKindCategory.Combo:
                    isIt = IsComboLetter();
                    break;
                case LetterKindCategory.Real:
                    isIt = IsRealLetter();
                    break;
                case LetterKindCategory.BaseAndVariations:
                    isIt = IsBaseOrVariationLetter();
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

        private bool IsBaseOrVariationLetter()
        {
            return this.Kind == LetterDataKind.Letter || this.Kind == LetterDataKind.LetterVariation;
        }

        public ILivingLetterData ConvertToLivingLetterData()
        {
            return new LL_LetterData(GetId(), this);
        }

        public string GetChar(LetterPosition position = LetterPosition.Isolated)
        {
            switch (position) {
                case LetterPosition.Initial:
                    return Initial;
                case LetterPosition.Medial:
                    return Medial;
                case LetterPosition.Final:
                    return Final;
                default:
                    return Isolated;
            }
        }

        public string GetUnicode(LetterPosition position = LetterPosition.Isolated)
        {
            switch (position) {
                case LetterPosition.Initial:
                    return Initial_Unicode;
                case LetterPosition.Medial:
                    return Medial_Unicode;
                case LetterPosition.Final:
                    return Final_Unicode;
                default:
                    return Isolated_Unicode;
            }
        }
    }
}