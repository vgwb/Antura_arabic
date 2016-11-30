namespace EA4S
{
    // Filter parameters for letters
    public class LetterFilters
    {
        public bool excludeDiacritics;
        public bool excludeDiacritics_keepMain; // HACK filter
        public bool excludeLetterVariations;
        public bool requireDiacritics;

        public LetterFilters(
            bool excludeDiacritics = false,
            bool excludeDiacritics_keepMain = false,
            bool excludeLetterVariations = false,
            bool requireDiacritics = false)
        {
            this.excludeDiacritics = excludeDiacritics;
            this.excludeDiacritics_keepMain = excludeDiacritics_keepMain;
            this.excludeLetterVariations = excludeLetterVariations;
            this.requireDiacritics = requireDiacritics;
        }
    }

    // Filter parameters for words
    public class WordFilters
    {
        public bool excludeDiacritics;
        public bool excludeLetterVariations;
        public bool requireDiacritics;
        public bool excludeArticles;
        public bool excludePluralDual;
        public bool requireDrawings;
        public bool excludeColorWords;

        public WordFilters(
            bool excludeDiacritics = false,
            bool excludeLetterVariations = false,
            bool requireDiacritics = false,
            bool excludeArticles = false,
            bool excludePluralDual = false,
            bool requireDrawings = false,
            bool excludeColorWords = false)
        {
            this.excludeDiacritics = excludeDiacritics;
            this.excludeLetterVariations = excludeLetterVariations;
            this.requireDiacritics = requireDiacritics;
            this.excludeArticles = excludeArticles;
            this.excludePluralDual = excludePluralDual;
            this.requireDrawings = requireDrawings;
            this.excludeColorWords = excludeColorWords;
        }
    }

    // Filter parameters for phrases
    public class PhraseFilters
    {
        public bool requireWords;
        public bool requireAnswersOrWords;
        public bool requireAtLeastTwoWords;

        public PhraseFilters(
            bool requireWords = false,
            bool requireAnswersOrWords = false,
            bool requireAtLeastTwoWords = false // @todo: this could be reworked with Phrase Categories so to create better filters, or allow filters to have a numeric value
            )
        {
            this.requireWords = requireWords;
            this.requireAnswersOrWords = requireAnswersOrWords;
            this.requireAtLeastTwoWords = requireAtLeastTwoWords;
        }
    }

}
