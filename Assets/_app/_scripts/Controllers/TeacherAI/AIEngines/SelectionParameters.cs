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
        public bool getAllData;
        public bool useJourney;
        public PackListHistory packListHistory;
        public List<string> filteringIds;

        public SelectionParameters(SelectionSeverity severity, int nRequired = 0, bool getAllData = false, bool useJourney = true, PackListHistory packListHistory = PackListHistory.NoFilter, List < string> filteringIds = null)
        {
            this.nRequired = nRequired;
            this.getAllData = getAllData;
            this.severity = severity;
            this.useJourney = useJourney;
            this.packListHistory = packListHistory;
            this.filteringIds = filteringIds;
        }
    }

}