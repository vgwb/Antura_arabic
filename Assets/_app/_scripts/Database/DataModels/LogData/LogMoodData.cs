using SQLite;
using System;

namespace EA4S.Db
{

    [System.Serializable]
    public class LogMoodData : IData
    {
        public string Id { get; set; }
        public string Session { get; set; }
        public string Time { get; set; }
        public int PlayerID { get; set; }
        public int MoodValue { get; set; }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},P{2},MV{3}",
                Session,
                Time,
                PlayerID,
                MoodValue
                );
        }

    }
}