using System;
using EA4S.MinigamesAPI;

namespace EA4S.Database
{
    public enum LetterKindCategory
    {
        Real = 0,   // default: Base + Combo
        DiacriticCombo,
        Base,
        LetterVariation,
        Symbol,
        BaseAndVariations
    }

    [Flags]
    public enum LetterForm
    {
        None = 0,
        Isolated = 1,
        Initial = 2,
        Medial = 4,
        Final = 8
    }

    /// <summary>
    /// Data defining a Letter.
    /// This is one of the fundamental dictionary (i.e. learning content) elements.
    /// <seealso cref="PhraseData"/>
    /// <seealso cref="WordData"/>
    /// </summary>
    // refactor: this requires heavy refactoring!
    // refactor: we could make this general in respect to the language
    [Serializable]
    public class LetterData : IVocabularyData, IConvertibleToLivingLetterData
    {
        public string Id;
        public bool Active;
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
        public string Symbol_Unicode;
        public int Symbol_DeltaY;
        public string InitialFix;
        public string FinalFix;
        public string MedialFix;
        public string Old_Isolated;
        public string Old_Initial;
        public string Old_Medial;
        public string Old_Final;
        public float Intrinsic;

        public override string ToString()
        {
            return Id + ": " + Isolated;
        }

        public float GetIntrinsicDifficulty()
        {
            return Intrinsic;
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
                case LetterKindCategory.LetterVariation:
                    isIt = IsVariationLetter();
                    break;
                case LetterKindCategory.Symbol:
                    isIt = IsSymbolLetter();
                    break;
                case LetterKindCategory.DiacriticCombo:
                    isIt = IsDiacriticComboLetter();
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
            return this.IsBaseLetter() || this.IsDiacriticComboLetter();
        }

        private bool IsBaseLetter()
        {
            return this.Kind == LetterDataKind.Letter;
        }

        private bool IsVariationLetter()
        {
            return this.Kind == LetterDataKind.LetterVariation;
        }

        private bool IsSymbolLetter()
        {
            return this.Kind == LetterDataKind.Symbol;
        }

        private bool IsDiacriticComboLetter()
        {
            return this.Kind == LetterDataKind.DiacriticCombo;
        }

        private bool IsBaseOrVariationLetter()
        {
            return this.Kind == LetterDataKind.Letter || this.Kind == LetterDataKind.LetterVariation;
        }

        public ILivingLetterData ConvertToLivingLetterData()
        {
            return new LL_LetterData(this);
        }

        public string GetUnicode(LetterForm form = LetterForm.Isolated, bool fallback = true)
        {
            switch (Kind) {
                case LetterDataKind.Symbol:
                    return Isolated_Unicode;
                default:
                    switch (form) {
                        case LetterForm.Initial:
                            return Initial_Unicode != "" ? Initial_Unicode : (fallback ? Isolated_Unicode : "");
                        case LetterForm.Medial:
                            return Medial_Unicode != "" ? Medial_Unicode : (fallback ? Isolated_Unicode : "");
                        case LetterForm.Final:
                            return Final_Unicode != "" ? Final_Unicode : (fallback ? Isolated_Unicode : "");
                        default:
                            return Isolated_Unicode;
                    }
            }
        }

        public string GetChar(LetterForm form = LetterForm.Isolated)
        {
            string output = "";
            var hexunicode = GetUnicode(form);
            if (hexunicode != "") {

                // add the "-" to diacritic symbols to indentify better if it's over or below hte mid line
                if (Type == LetterDataType.DiacriticSymbol) {
                    output = "\u0640";
                }

                int unicode = int.Parse(hexunicode, System.Globalization.NumberStyles.HexNumber);
                output += ((char)unicode).ToString();

                if (Symbol_Unicode != "") {
                    int unicode_added = int.Parse(Symbol_Unicode, System.Globalization.NumberStyles.HexNumber);
                    output += ((char)unicode_added).ToString();
                }
            }
            return output;
        }

        // this just adds a "-" before medial and final single letters! if needed
        public string GetCharFixedForDisplay(LetterForm form = LetterForm.Isolated)
        {
            if (GetUnicode(form, false) == "")
                return "";

            string output = GetChar(form);

            if ((form == LetterForm.Final && FinalFix != "") || (form == LetterForm.Medial && MedialFix != "")) {
                output = "\u0640" + output;
            }

            if ((form == LetterForm.Initial && InitialFix != "") || (form == LetterForm.Medial && InitialFix != "")) {
                output = output + "\u0640";
            }

            return output;
        }

        public System.Collections.Generic.IEnumerable<LetterForm> GetAvailableForms()
        {
            if (Isolated_Unicode != "")
                yield return LetterForm.Isolated;

            if (Initial_Unicode != "")
                yield return LetterForm.Initial;

            if (Medial_Unicode != "")
                yield return LetterForm.Medial;

            if (Final_Unicode != "")
                yield return LetterForm.Final;
        }

    }
}