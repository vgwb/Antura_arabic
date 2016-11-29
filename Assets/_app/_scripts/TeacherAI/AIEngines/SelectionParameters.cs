using System.Collections.Generic;

namespace EA4S.Teacher
{
    public enum SelectionSeverity
    {
        AsManyAsPossible,       // If possible, the given number of data valuesis asked for, or less if there are not enough.
        AllRequired,            // The given number of data values is required. Error if it is not reached.
        MayRepeatIfNotEnough    // @todo: may repeat the same values if not enough values are found
    }

    public enum PackListHistory
    {
        NoFilter,               // Multiple packs in the game have no influence one over the other
        ForceAllDifferent,      // Make sure that multiple packs in a list do not contain the same values
        RepeatWhenFull,         // Try to make sure that multiple packs have not the same values, fallback to NoFilter if we cannot get enough data
        SkipPacks,              // If we cannot find enough data, reduce the number of packs to be generated
    }

    public struct SelectionParameters
    {
        public SelectionSeverity severity;
        public int nRequired;
        public bool getMaxData;
        public bool useJourney;
        public PackListHistory packListHistory;
        public List<string> filteringIds;

        public SelectionParameters(SelectionSeverity severity, int nRequired = 0, bool getMaxData = false, bool useJourney = true, PackListHistory packListHistory = PackListHistory.NoFilter, List < string> filteringIds = null)
        {
            this.nRequired = nRequired;
            this.getMaxData = getMaxData;
            this.severity = severity;
            this.useJourney = useJourney;
            this.packListHistory = packListHistory;
            this.filteringIds = filteringIds;
        }
    }

    public class QuestionBuilderParameters
    {
        public PackListHistory correctChoicesHistory;
        public PackListHistory wrongChoicesHistory;
        public bool useJourneyForWrong;
        public bool useJourneyForCorrect;
        public SelectionSeverity correctSeverity;
        public SelectionSeverity wrongSeverity;

        // data-based params
        public LetterFilters letterFilters;
        public WordFilters wordFilters;
        public PhraseFilters phraseFilters;

        public QuestionBuilderParameters()
        {
            this.correctChoicesHistory = PackListHistory.RepeatWhenFull;
            this.wrongChoicesHistory = PackListHistory.RepeatWhenFull;
            this.useJourneyForCorrect = true;
            this.useJourneyForWrong = false;
            this.correctSeverity = SelectionSeverity.MayRepeatIfNotEnough;
            this.wrongSeverity = SelectionSeverity.MayRepeatIfNotEnough;
            this.letterFilters = new LetterFilters();
            this.wordFilters = new WordFilters();
            this.phraseFilters = new PhraseFilters();
        }
    }


}