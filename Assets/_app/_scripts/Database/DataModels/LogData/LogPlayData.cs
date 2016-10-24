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
        public int PlayerID { get; set; }

        public string PlaySession { get; set; }
        public MiniGameCode MiniGame { get; set; }
        public PlayEvent Action { get; set; }
        public PlaySkill PlaySkill { get; set; }
        public float Score { get; set; }
        public string RawData { get; set; }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},P{2},PS{3},C{4},A{5},S{6},RD{7}",
                Session,
                Timestamp,
                PlayerID,
                PlaySkill,
                Action,
                Score,
                RawData
                );
        }

    }
}