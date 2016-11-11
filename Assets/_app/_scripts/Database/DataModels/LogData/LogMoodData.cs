using SQLite;
using System;

namespace EA4S.Db
{

    [System.Serializable]
    public class LogMoodData : IData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Session { get; set; }
        public int Timestamp { get; set; }

        public float MoodValue { get; set; }

        public LogMoodData() : this(0)
        {
        }

        public LogMoodData(float _mood)
        {
            this.MoodValue = _mood;
            this.Timestamp = GenericUtilities.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
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