using EA4S.Helpers;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Saved data on achievements related to a minigame. Updated at runtime.
    /// </summary>
    [System.Serializable]
    public class MinigameScoreData : IData, IScoreData
    {
        public float GetScore()
        {
            return Score;
        }

        [PrimaryKey]
        public string Id { get; set; } 
        public string ElementId { get; set; }

        public int Score { get; set; }  // [0,3]
        public float TotalPlayTime { get; set; } 
        public int LastAccessTimestamp { get; set; }

        public MinigameScoreData()
        {
        }
        public MinigameScoreData(string elementId, int score, float totalPlayTime) : this(elementId, score, totalPlayTime, GenericHelper.GetTimestampForNow())
        {
        }
        public MinigameScoreData(string elementId, int score, float totalPlayTime, int timestamp)
        {
            ElementId = elementId;
            Id = ElementId;
            Score = score;
            TotalPlayTime = totalPlayTime;
            LastAccessTimestamp = timestamp;
        }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("E{0},S{1},T{2},TS{3}",
                ElementId,
                Score,
                TotalPlayTime,
                LastAccessTimestamp
                );
        }

    }
}