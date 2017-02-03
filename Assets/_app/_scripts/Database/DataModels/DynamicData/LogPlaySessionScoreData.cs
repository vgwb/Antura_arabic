using EA4S.Core;
using EA4S.Helpers;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Journey score obtained at a given timestamp. Logged at runtime.
    /// </summary>
    [System.Serializable]
    public class LogPlaySessionScoreData : IData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string AppSession { get; set; } 
        public int Timestamp { get; set; }

        public int Stage { get; set; }
        public int LearningBlock { get; set; }
        public int PlaySession { get; set; }

        public int Score { get; set; } // 0:3
        public float PlayTime { get; set; } 

        public LogPlaySessionScoreData()
        {
        }

        public LogPlaySessionScoreData(string _AppSession, JourneyPosition _pos, int _score, float _playTime)
        {
            AppSession = _AppSession;
            Stage = _pos.Stage;
            LearningBlock = _pos.LearningBlock;
            PlaySession = _pos.PlaySession;
            Score = _score;
            PlayTime = _playTime;
            Timestamp = GenericHelper.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},(ST{2},LB{3},PS{4}),S{5},PT{6}",
                AppSession,
                Timestamp,
                Stage,
                LearningBlock,
                PlaySession,
                Score,
                PlayTime
                );
        }

    }
}