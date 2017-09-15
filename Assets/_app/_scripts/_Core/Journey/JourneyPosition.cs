namespace Antura.Core
{
    /// <summary>
    /// Represents the position of the player in the learning journey.
    /// </summary>
    // TODO refactor: this being a class may create some pesky bugs. Make it a struct?
    // TODO refactor: merge JourneyPosition, JourneyHelper
    [System.Serializable]
    public class JourneyPosition
    {
        public int Stage = 1;
        public int LearningBlock = 1;
        public int PlaySession = 1;

        public static JourneyPosition InitialJourneyPosition = new JourneyPosition(1, 1, 1);

        public JourneyPosition()
        {
            Stage = 1;
            LearningBlock = 1;
            PlaySession = 1;
        }

        public JourneyPosition(int _stage, int _lb, int _ps)
        {
            Stage = _stage;
            LearningBlock = _lb;
            PlaySession = _ps;
        }

        public JourneyPosition(string psId)
        {
            var splits = psId.Split('.');
            Stage = int.Parse(splits[0]);
            LearningBlock = int.Parse(splits[1]);
            PlaySession = int.Parse(splits[2]);
        }

        public JourneyPosition(JourneyPosition other)
        {
            Stage = other.Stage;
            LearningBlock = other.LearningBlock;
            PlaySession = other.PlaySession;
        }

        public void SetPosition(int _stage, int _lb, int _ps)
        {
            Stage = _stage;
            LearningBlock = _lb;
            PlaySession = _ps;
        }

        public override bool Equals(object obj)
        {
            var otherPos = (JourneyPosition) obj;
            return Stage == otherPos.Stage && LearningBlock == otherPos.LearningBlock && PlaySession == otherPos.PlaySession;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // TODO refactor: this is used by part of the application to convert hourney to an ID for DB purposes. Make this more robust.
        public override string ToString()
        {
            return Stage + "." + LearningBlock + "." + PlaySession;
        }

        public string ToStringId()
        {
            return Stage + "." + LearningBlock + "." + PlaySession;
        }

        public string GetShortTitle()
        {
            return Stage + "." + LearningBlock;
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

        public bool IsMinorOrEqual(JourneyPosition other)
        {
            return IsMinor(other) || Equals(other);
        }

        public bool IsAssessment()
        {
            return PlaySession == 100;
        }
    }
}