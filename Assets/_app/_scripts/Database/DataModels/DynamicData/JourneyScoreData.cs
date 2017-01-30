using EA4S.Helpers;
using EA4S.Utilities;
using SQLite;

namespace EA4S.Database
{
    public enum JourneyDataType
    {
        Minigame,
        Stage,
        LearningBlock,
        PlaySession
    }

    /// <summary>
    /// Score (in stars) relative to a journey element or a minigame. Updated at runtime.
    /// </summary>
    [System.Serializable]
    public class JourneyScoreData : IData, IScoreData
    {
        public float GetScore()
        {
            return Score;
        }

        [PrimaryKey]
        public string Id { get; set; } 

        // refactor: this should instead be an enum
        public JourneyDataType JourneyDataType { get; set; } 
        public string ElementId { get; set; }

        public int Score { get; set; }  // [0,3]
        public int LastAccessTimestamp { get; set; }

        public JourneyScoreData()
        {

        }
        public JourneyScoreData(string elementId, JourneyDataType dataType, int score) : this(elementId, dataType, score, GenericHelper.GetTimestampForNow())
        {
        }
        public JourneyScoreData(string elementId, JourneyDataType dataType, int score, int timestamp)
        {
            ElementId = elementId;
            JourneyDataType = dataType;
            Id = JourneyDataType + "." + ElementId;
            Score = score;
            LastAccessTimestamp = timestamp;
        }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("T{0},E{1},S{2},T{3}",
                JourneyDataType,
                ElementId,
                Score,
                LastAccessTimestamp
                );
        }

    }
}