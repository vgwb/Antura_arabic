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
        public int PlayerID { get; set; }

        public float MoodValue { get; set; }

        public LogMoodData() : this(0,0) {}

        public LogMoodData(int _mood, int _playerId)
        {
            this.Id = "TODO?";
            this.MoodValue = _mood;
            this.PlayerID = _playerId;
            this.Timestamp = GenericUtilites.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},P{2},MV{3}",
                Session,
                Timestamp,
                PlayerID,
                MoodValue
                );
        }

    }
}