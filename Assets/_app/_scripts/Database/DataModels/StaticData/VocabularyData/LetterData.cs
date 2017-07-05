using System;
using System.Collections.Generic;
using System.Globalization;
using Antura.MinigamesAPI;
using SQLite;
using UnityEngine;

namespace Antura.Database
{
    public enum LetterKindCategory
    {
        Real = 0, // default: Base + Combo
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
    ///     Data defining a Letter.
    ///     This is one of the fundamental dictionary (i.e. learning content) elements.
    ///     <seealso cref="PhraseData" />
    ///     <seealso cref="WordData" />
    /// </summary>
    // TODO refactor: this requires heavy refactoring!
    // TODO refactor: we could make this general in respect to the language
    [Serializable]
    public class LetterData : IVocabularyData, IConvertibleToLivingLetterData
    {
        [PrimaryKey]
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        [SerializeField] private string _Id;

        public bool Active
        {
            get { return _Active; }
            set { _Active = value; }
        }
        [SerializeField] private bool _Active;

        public int Number
        {
            get { return _Number; }
            set { _Number = value; }
        }
        [SerializeField] private int _Number;

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }
        [SerializeField] private string _Title;

        public LetterDataKind Kind
        {
            get { return _Kind; }
            set { _Kind = value; }
        }
        [SerializeField] private LetterDataKind _Kind;

        public string BaseLetter
        {
            get { return _BaseLetter; }
            set { _BaseLetter = value; }
        }
        [SerializeField] private string _BaseLetter;

        public string Symbol
        {
            get { return _Symbol; }
            set { _Symbol = value; }
        }
        [SerializeField] private string _Symbol;

        public LetterDataType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        [SerializeField] private LetterDataType _Type;

        public string Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }
        [SerializeField] private string _Tag;

        public string Notes
        {
            get { return _Notes; }
            set { _Notes = value; }
        }
        [SerializeField] private string _Notes;

        public LetterDataSunMoon SunMoon
        {
            get { return _SunMoon; }
            set { _SunMoon = value; }
        }
        [SerializeField] private LetterDataSunMoon _SunMoon;

        public string Sound
        {
            get { return _Sound; }
            set { _Sound = value; }
        }
        [SerializeField] private string _Sound;

        public string SoundZone
        {
            get { return _SoundZone; }
            set { _SoundZone = value; }
        }
        [SerializeField] private string _SoundZone;

        public string Isolated
        {
            get { return _Isolated; }
            set { _Isolated = value; }
        }
        [SerializeField] private string _Isolated;

        public string Initial
        {
            get { return _Initial; }
            set { _Initial = value; }
        }
        [SerializeField] private string _Initial;

        public string Medial
        {
            get { return _Medial; }
            set { _Medial = value; }
        }
        [SerializeField] private string _Medial;

        public string Final
        {
            get { return _Final; }
            set { _Final = value; }
        }

        [SerializeField] private string _Final;

        public string Isolated_Unicode
        {
            get { return _Isolated_Unicode; }
            set { _Isolated_Unicode = value; }
        }

        [SerializeField] private string _Isolated_Unicode;

        public string Initial_Unicode
        {
            get { return _Initial_Unicode; }
            set { _Initial_Unicode = value; }
        }

        [SerializeField] private string _Initial_Unicode;

        public string Medial_Unicode
        {
            get { return _Medial_Unicode; }
            set { _Medial_Unicode = value; }
        }

        [SerializeField] private string _Medial_Unicode;

        public string Final_Unicode
        {
            get { return _Final_Unicode; }
            set { _Final_Unicode = value; }
        }

        [SerializeField] private string _Final_Unicode;

        public string Symbol_Unicode
        {
            get { return _Symbol_Unicode; }
            set { _Symbol_Unicode = value; }
        }

        [SerializeField] private string _Symbol_Unicode;

        public string InitialFix
        {
            get { return _InitialFix; }
            set { _InitialFix = value; }
        }

        [SerializeField] private string _InitialFix;

        public string FinalFix
        {
            get { return _FinalFix; }
            set { _FinalFix = value; }
        }

        [SerializeField] private string _FinalFix;

        public string MedialFix
        {
            get { return _MedialFix; }
            set { _MedialFix = value; }
        }

        [SerializeField] private string _MedialFix;

        public string Old_Isolated
        {
            get { return _Old_Isolated; }
            set { _Old_Isolated = value; }
        }

        [SerializeField] private string _Old_Isolated;

        public string Old_Initial
        {
            get { return _Old_Initial; }
            set { _Old_Initial = value; }
        }

        [SerializeField] private string _Old_Initial;

        public string Old_Medial
        {
            get { return _Old_Medial; }
            set { _Old_Medial = value; }
        }

        [SerializeField] private string _Old_Medial;

        public string Old_Final
        {
            get { return _Old_Final; }
            set { _Old_Final = value; }
        }

        [SerializeField] private string _Old_Final;

        public float Complexity
        {
            get { return _Complexity; }
            set { _Complexity = value; }
        }
        [SerializeField] private float _Complexity;


        public override string ToString()
        {
            return Id + ": " + Isolated;
        }

        public float GetIntrinsicDifficulty()
        {
            return Complexity;
        }

        public string GetId()
        {
            return Id;
        }


        public bool IsOfKindCategory(LetterKindCategory category)
        {
            var isIt = false;
            switch (category)
            {
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
            return IsBaseLetter() || IsDiacriticComboLetter();
        }

        private bool IsBaseLetter()
        {
            return Kind == LetterDataKind.Letter;
        }

        private bool IsVariationLetter()
        {
            return Kind == LetterDataKind.LetterVariation;
        }

        private bool IsSymbolLetter()
        {
            return Kind == LetterDataKind.Symbol;
        }

        private bool IsDiacriticComboLetter()
        {
            return Kind == LetterDataKind.DiacriticCombo;
        }

        private bool IsBaseOrVariationLetter()
        {
            return Kind == LetterDataKind.Letter || Kind == LetterDataKind.LetterVariation;
        }

        public ILivingLetterData ConvertToLivingLetterData()
        {
            return new LL_LetterData(this);
        }

        public string GetUnicode(LetterForm form = LetterForm.Isolated, bool fallback = true)
        {
            switch (Kind)
            {
                case LetterDataKind.Symbol:
                    return Isolated_Unicode;
                default:
                    switch (form)
                    {
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
            var output = "";
            var hexunicode = GetUnicode(form);
            if (hexunicode != "")
            {
                // add the "-" to diacritic symbols to indentify better if it's over or below hte mid line
                if (Type == LetterDataType.DiacriticSymbol) output = "\u0640";

                var unicode = int.Parse(hexunicode, NumberStyles.HexNumber);
                output += ((char) unicode).ToString();

                if (Symbol_Unicode != "")
                {
                    var unicode_added = int.Parse(Symbol_Unicode, NumberStyles.HexNumber);
                    output += ((char) unicode_added).ToString();
                }
            }
            return output;
        }

        // this just adds a "-" before medial and final single letters! if needed
        public string GetCharFixedForDisplay(LetterForm form = LetterForm.Isolated)
        {
            if (GetUnicode(form, false) == "")
                return "";

            var output = GetChar(form);

            if (form == LetterForm.Final && FinalFix != "" || form == LetterForm.Medial && MedialFix != "")
                output = "\u0640" + output;

            if (form == LetterForm.Initial && InitialFix != "" || form == LetterForm.Medial && InitialFix != "")
                output = output + "\u0640";

            return output;
        }

        public IEnumerable<LetterForm> GetAvailableForms()
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