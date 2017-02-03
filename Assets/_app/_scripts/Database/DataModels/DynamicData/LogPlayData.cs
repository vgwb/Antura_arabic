using EA4S.Helpers;
using EA4S.Utilities;
using SQLite;

namespace EA4S.Database
{

    /// <summary>
    /// Play-related measurements obtained at a given timestamp. Logged at runtime.
    /// </summary>
    [System.Serializable]
    public class LogPlayData : IData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string AppSession { get; set; }
        public int Timestamp { get; set; }

        public string PlaySession { get; set; }
        public MiniGameCode MiniGame { get; set; }
        public PlayEvent PlayEvent { get; set; }
        public PlaySkill PlaySkill { get; set; }
        public float Score { get; set; }
        public string RawData { get; set; }

        public LogPlayData()
        {
        }

        public LogPlayData(string _AppSession, string _PlaySession, MiniGameCode _MiniGame, PlayEvent _PlayEvent, PlaySkill _PlaySkill, float _Score)
            : this(_AppSession, _PlaySession, _MiniGame, _PlayEvent, _PlaySkill, _Score, "")
        {
        }

        public LogPlayData(string _AppSession, string _PlaySession, MiniGameCode _MiniGame, PlayEvent _PlayEvent, PlaySkill _PlaySkill, float _Score, string _RawData)
        {
            AppSession = _AppSession;
            PlaySession = _PlaySession;
            MiniGame = _MiniGame;
            PlayEvent = _PlayEvent;
            PlaySkill = _PlaySkill;
            Score = _Score;
            RawData = _RawData;
            Timestamp = GenericHelper.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return string.Format("AS{0},T{1},PS{2},MG{3},PE{4},SK{5},S{6},RD{7}",
                AppSession,
                Timestamp,
                PlaySession,
                MiniGame,
                PlayEvent,
                PlaySkill,
                Score,
                RawData
                );
        }

    }
}