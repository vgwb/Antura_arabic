using EA4S.Core;
using EA4S.Helpers;
using EA4S.MinigamesCommon;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Minigame score obtained at a given timestamp. Logged at runtime.
    /// </summary>
    [System.Serializable]
    public class LogMinigameScoreData : IData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string AppSession { get; set; }
        public int Timestamp { get; set; }

        public int Stage { get; set; }
        public int LearningBlock { get; set; }
        public int PlaySession { get; set; }

        public MiniGameCode MiniGameCode { get; set; }

        public int Score { get; set; } // 0:3
        public float PlayTime { get; set; }

        public LogMinigameScoreData()
        {
        }

        public LogMinigameScoreData(string _AppSession, JourneyPosition _pos, MiniGameCode _code, int _score, float _playTime)
        {
            AppSession = _AppSession;
            Stage = _pos.Stage;
            LearningBlock = _pos.LearningBlock;
            PlaySession = _pos.PlaySession;
            Score = _score;
            PlayTime = _playTime;
            MiniGameCode = _code;
            Timestamp = GenericHelper.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},(ST{2},LB{3},PS{4}),M{5},S{6},PT{7}",
                AppSession,
                Timestamp,
                Stage,
                LearningBlock,
                PlaySession,
                MiniGameCode,
                Score,
                PlayTime
                );
        }
    }
}