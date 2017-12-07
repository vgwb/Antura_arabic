namespace Antura.Teacher
{
    public class LetterAlterationFilters
    {
        public static readonly LetterAlterationFilters FormsOfSingleLetter = new LetterAlterationFilters()
        {
            includeForms = true
        };
        public static readonly LetterAlterationFilters MultipleLetters = new LetterAlterationFilters()
        {
            differentBaseLetters = true
        };
        public static readonly LetterAlterationFilters PhonemesOfSingleLetter = new LetterAlterationFilters()
        {
            ExcludeDiacritics = LetterFilters.ExcludeDiacritics.None,
            ExcludeLetterVariations = LetterFilters.ExcludeLetterVariations.None,
            excludeDipthongs = false
        };
        public static readonly LetterAlterationFilters FormsOfMultipleLetters = new LetterAlterationFilters()
        {
            differentBaseLetters = true,
            includeForms = true
        };
        public static readonly LetterAlterationFilters PhonemesOfMultipleLetters = new LetterAlterationFilters()
        {
            differentBaseLetters = true,
            ExcludeDiacritics = LetterFilters.ExcludeDiacritics.None,
            ExcludeLetterVariations = LetterFilters.ExcludeLetterVariations.None,
            excludeDipthongs = false
        };
        public static readonly LetterAlterationFilters FormsAndPhonemesOfMultipleLetters = new LetterAlterationFilters()
        {
            differentBaseLetters = true,
            includeForms = true,
            ExcludeDiacritics = LetterFilters.ExcludeDiacritics.None,
            ExcludeLetterVariations = LetterFilters.ExcludeLetterVariations.None,
            excludeDipthongs = false,
        };


        public bool addBaseLetterToo = false;

        // Can add different letters as bases?
        public bool differentBaseLetters;

        // Can add the various variations on bases?
        public LetterFilters.ExcludeDiacritics ExcludeDiacritics;
        public LetterFilters.ExcludeLetterVariations ExcludeLetterVariations;
        public bool excludeDipthongs;

        // Can add forms?
        public bool includeForms;

        public LetterAlterationFilters() : this(false, false, LetterFilters.ExcludeDiacritics.All, LetterFilters.ExcludeLetterVariations.All, true, false)
        {
            
        }

        public LetterAlterationFilters(bool addBaseLetterToo, bool differentBaseLetters, LetterFilters.ExcludeDiacritics excludeDiacritics, LetterFilters.ExcludeLetterVariations excludeLetterVariations, bool excludeDipthongs, bool includeForms)
        {
            this.addBaseLetterToo = addBaseLetterToo;
            this.differentBaseLetters = differentBaseLetters;
            ExcludeDiacritics = excludeDiacritics;
            ExcludeLetterVariations = excludeLetterVariations;
            this.excludeDipthongs = excludeDipthongs;
            this.includeForms = includeForms;
        }
    }
}