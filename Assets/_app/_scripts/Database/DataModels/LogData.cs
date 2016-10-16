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

    public class LogData
    {
        public string Session;
        public string Time;
        public int PlayerID;
        public LogDataType Type;
        public LogPlaySkill PlaySkill;
        public string Context;
        public string Action;
        public float Score;
        public string RawData;

        public override string ToString()
        {
            return string.Format("{0},{1},{3},{4},{5},{6}" + Environment.NewLine,
                Session,
                Time,
                Type,
                Context,
                Action,
                RawData
                );
        }
    }
}