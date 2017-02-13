using EA4S.Helpers;
using EA4S.Utilities;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Serialized information on the database. Used for versioning.
    /// </summary>
    [System.Serializable]
    public class DatabaseInfoData : IData
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Version { get; set; }
        public int CreationTimestamp { get; set; }

        public DatabaseInfoData()
        {
        }

        public DatabaseInfoData(string _Version)
        {
            this.Id = 1;  // Only one record
            this.Version = _Version;
            this.CreationTimestamp = GenericHelper.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return string.Format("ID{0},V{1},Ts{3}",
                Id,
                Version,
                CreationTimestamp
            );
        }

    }
}