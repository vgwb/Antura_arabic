using SQLite;
using System;

namespace EA4S.Db
{
    public enum LogDataType
    {
        Info,
        PlayData,
        LearnData,
        MoodData
    }

    public enum LogPlaySkill
    {
        Precision = 0,
        Reaction = 1,
        Memory = 2,
        Logic = 3,
        Rhythm = 4,
        Musicality = 5,
        Sight = 6
    }

    [System.Serializable]
    public class LogData : IData
    {
        public string Id { get; set; }
        public string Session { get; set; }
        public string Time { get; set; }
        public int PlayerID { get; set; }
        public LogDataType Type { get; set; }
        public LogPlaySkill PlaySkill { get; set; }
        public string Context { get; set; }
        public string Action { get; set; }
        public float Score { get; set; }
        public string RawData { get; set; }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},P{2},T{3},PS{4},C{5},A{6},S{7},RD{8}",
                Session,
                Time,
                PlayerID,
                Type,
                PlaySkill,
                Context,
                Action,
                Score,
                RawData
                );
        }

    }
}