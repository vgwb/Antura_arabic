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
        public MiniGameCode MiniGameCode { get; set; }

        public int Score { get; set; }  // [0,3]
        public float TotalPlayTime { get; set; } 
        public int LastAccessTimestamp { get; set; }

        public MinigameScoreData()
        {
        }
        public MinigameScoreData(MiniGameCode code, int score, float totalPlayTime) : this(code, score, totalPlayTime, GenericHelper.GetTimestampForNow())
        {
        }
        public MinigameScoreData(MiniGameCode code, int score, float totalPlayTime, int timestamp)
        {
            MiniGameCode = code;
            Id = ((int) code).ToString();
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
            return string.Format("MG{0},S{1},T{2},TS{3}",
                MiniGameCode,
                Score,
                TotalPlayTime,
                LastAccessTimestamp
                );
        }

    }
}