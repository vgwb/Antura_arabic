
namespace EA4S.Teacher
{
    public struct SelectionParameters
    {
        public SelectionSeverity severity;
        public int nRequired;
        public bool ignoreJourney;

        public SelectionParameters(SelectionSeverity _severity, int _nRequired = 0, bool _ignoreJourney = false)
        {
            nRequired = _nRequired;
            severity = _severity;
            ignoreJourney = _ignoreJourney;
        }
    }

}