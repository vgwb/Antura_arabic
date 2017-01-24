namespace EA4S
{
    /// <summary>
    /// Represents the position of the player in the learning journey.
    /// </summary>
    // refactor: this being a class may create some pesky bugs. Make it a struct?
    // refactor: merge JourneyPosition, JourneyHelper
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

        public void SetPosition(int _stage, int _lb, int _ps)
        {
            Stage = _stage;
            LearningBlock = _lb;
            PlaySession = _ps;
        }

        // refactor: this is used by part of the application to convert hourney to an ID for DB purposes. Make this more robust.
        public override string ToString()
        {
            return Stage + "." + LearningBlock + "." + PlaySession;
        }

        public bool IsMinor(JourneyPosition other)
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