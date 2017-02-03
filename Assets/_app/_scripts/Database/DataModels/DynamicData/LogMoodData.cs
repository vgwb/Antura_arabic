﻿using EA4S.Helpers;
using EA4S.Utilities;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Daily mood level of a player at a given timestamp. Logged at runtime.
    /// </summary>
    [System.Serializable]
    public class LogMoodData : IData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string AppSession { get; set; }
        public int Timestamp { get; set; }

        public float MoodValue { get; set; }

        public LogMoodData() : this("XXX", 0)
        {
        }

        public LogMoodData(string _AppSession, float _mood)
        {
            AppSession = _AppSession;
            MoodValue = _mood;
            Timestamp = GenericHelper.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},MV{2}",
                AppSession,
                Timestamp,
                MoodValue
                );
        }

    }
}