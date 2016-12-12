using SQLite;

namespace EA4S
{
    public enum InfoEvent
    {
        ProfileCreated = 1,

        AppStarted = 20,
        AppClosed = 21,
        AppPlay = 22,
        AppSuspend = 23,
        AppResume = 24,
        Book = 30,
        GameStart = 40,
        GameEnd = 41,
        Reward = 50,
        AnturaSpace = 60,
        Map = 70,
    }
}

namespace EA4S.Db
{
    [System.Serializable]
    public class LogInfoData : IData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Session { get; set; }
        public int Timestamp { get; set; }

        public InfoEvent Event { get; set; }
        public string Parameters { get; set; } // examples: "playerId:0, rewardType:2"

        public LogInfoData()
        {
        }

        public LogInfoData(string _Session, InfoEvent _Event, string _Parameters)
        {
            this.Session = _Session;
            this.Event = _Event;
            this.Parameters = _Parameters;
            this.Timestamp = GenericUtilities.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
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