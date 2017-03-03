using EA4S.Helpers;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Generic information on application usage at a given timestamp. Logged at runtime.
    /// </summary>
    [System.Serializable]
    public class LogInfoData : IData
    {
        /// <summary>
        /// Primary key for the database.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Identifier of the application session.
        /// </summary>
        public int AppSession { get; set; }

        /// <summary>
        /// Timestamp of creation of this entry.
        /// </summary>
        public int Timestamp { get; set; }

        /// <summary>
        /// Event recorded.
        /// </summary>
        public InfoEvent Event { get; set; }

        /// <summary>
        /// Additional raw JSON data saved alongside the event to record more details.
        /// Example: "{playerId:0, rewardType:2}"
        /// </summary>
        public string AdditionalData { get; set; } 

        /// <summary>
        /// Empty constructor required by MySQL.
        /// </summary>
        public LogInfoData() { }

        public LogInfoData(int appSession, InfoEvent _event, string additionalData)
        {
            AppSession = appSession;
            Event = _event;
            AdditionalData = additionalData;
            Timestamp = GenericHelper.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return string.Format("AS{0},T{1},E{2},JSON{3}",
                AppSession,
                Timestamp,
                Event,
                AdditionalData
            );
        }

    }
}