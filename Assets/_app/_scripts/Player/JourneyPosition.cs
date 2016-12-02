namespace EA4S
{
    [System.Serializable]
    public class JourneyPosition
    {

        public int Stage = 1;
        public int LearningBlock = 1;
        public int PlaySession = 1;

        public JourneyPosition(int _stage, int _lb, int _ps)
        {
            Stage = _stage;
            LearningBlock = _lb;
            PlaySession = _ps;
        }

        public override string ToString()
        {
            return Stage + "." + LearningBlock + "." + PlaySession;
        }

        public bool isMinor(JourneyPosition other)
        {
            if (Stage < other.Stage) {
                return true;
            }
            if (Stage <= other.Stage && LearningBlock < other.LearningBlock) {
                return true;
            }
            if (Stage <= other.Stage && LearningBlock <= other.LearningBlock && PlaySession < other.PlaySession) {
                return true;
            }
            return false;
        }

    }
}