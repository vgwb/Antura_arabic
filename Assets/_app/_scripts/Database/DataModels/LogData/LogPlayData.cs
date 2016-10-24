using SQLite;
using System;

namespace EA4S.Db
{

    public enum PlaySkill
    {
        None = 0,
        Precision = 1,
        Reaction = 2,
        Memory = 3,
        Logic = 4,
        Rhythm = 5,
        Musicality = 6,
        Sight = 7
    }

    public enum PlayEvent
    {
        GameStarted = 0,
        GameFinished = 1,
        Skill = 2
    }

    [System.Serializable]
    public class LogPlayData : IData
    {
        public string Id { get; set; }
        public string Session { get; set; } // DailySession Id
        public int Timestamp { get; set; }

        public string PlaySession { get; set; }
        public MiniGameCode MiniGame { get; set; }
        public PlayEvent PlayEvent { get; set; }
        public PlaySkill PlaySkill { get; set; }
        public float Score { get; set; }
        public string RawData { get; set; }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},PS{2},MG{3},PE{4},SK{5},S{6},RD{7}",
                Session,
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