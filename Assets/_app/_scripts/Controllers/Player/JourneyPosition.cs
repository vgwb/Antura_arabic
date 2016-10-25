namespace EA4S {
    [System.Serializable]
    public class JourneyPosition {

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

    }
}