using SQLite;
using System;

namespace EA4S.Db
{

    public enum InfoEvent
    {
        AppStarted = 1,
        AppClosed = 2,
        Book = 3
    }


    [System.Serializable]
    public class LogInfoData : IData
    {
        public string Id { get; set; }
        public string Session { get; set; }
        public int Timestamp { get; set; }
        public int PlayerID { get; set; }

        public InfoEvent Event { get; set; }
        public string Description { get; set; }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("T{0},T{1},P{2},E{3},D{4}",
                Session,
                Timestamp,
                PlayerID, Event,
                Description
            );
        }

    }
}