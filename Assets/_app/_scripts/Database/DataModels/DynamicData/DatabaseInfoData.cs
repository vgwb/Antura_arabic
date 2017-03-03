using EA4S.Helpers;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Serialized information on the database. Used for versioning.
    /// </summary>
    [System.Serializable]
    public class DatabaseInfoData : IData
    {
        public const string UNIQUE_ID = "1";

        /// <summary>
        /// Primary key for the database.
        /// Unique, as there will be only one row for this table.
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// Timestamp of creation of the database.
        /// </summary>
        public int CreationTimestamp { get; set; }

        /// <summary>
        /// Version of the MySQL database.
        /// Different versions cannot be compared.
        /// </summary>
        public string MySqlDatabaseVersion { get; set; }

        /// <summary>
        /// Version of the Static database.
        /// Different versions cannot be compared.
        /// </summary>
        public string StaticDatabaseVersion { get; set; }  

        public DatabaseInfoData()
        {
        }

        public DatabaseInfoData(string mySqlDatabaseVersion, string staticDatabaseVersion)
        {
            this.Id = UNIQUE_ID;  // Only one record
            this.MySqlDatabaseVersion = mySqlDatabaseVersion;
            this.StaticDatabaseVersion = staticDatabaseVersion;
            this.CreationTimestamp = GenericHelper.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return string.Format("ID{0},sqlV{1},statV{2},Ts{3}",
                Id,
                MySqlDatabaseVersion,
                StaticDatabaseVersion,
                CreationTimestamp
            );
        }

    }
}