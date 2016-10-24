using SQLite;
using System;

namespace EA4S.Db
{

    [System.Serializable]
    public class LogMoodData : IData
    {
        public string Id { get; set; }
        public string Session { get; set; }
        public int Timestamp { get; set; }

        public float MoodValue { get; set; }

        public LogMoodData() : this(0) {}

        public LogMoodData(int _mood)
        {
            this.Id = "TODO?";  // @TODO: give a unique auto-incrementing ID? (maybe just let SQLite do that)
            this.MoodValue = _mood;
            this.Timestamp = GenericUtilites.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},MV{2}",
                Session,
                Timestamp,
                MoodValue
                );
        }

    }
}