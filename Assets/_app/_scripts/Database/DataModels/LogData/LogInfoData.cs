using SQLite;
using System;

namespace EA4S.Db
{

    public enum InfoEvent
    {
        ProfileCreated = 1,

        AppStarted = 20,
        AppClosed = 21,

        Book = 30,
    }


    [System.Serializable]
    public class LogInfoData : IData
    {
        public string Id { get; set; }
        public string Session { get; set; }
        public int Timestamp { get; set; }

        public InfoEvent Event { get; set; }
        public string Parameters { get; set; } // playerId:0, rewardType:2

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("T{0},T{1},E{2},PARS{3}",
                Session,
                Timestamp,
                Event,
                Parameters
            );
        }

    }
}