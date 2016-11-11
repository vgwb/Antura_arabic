using SQLite;
using System;

namespace EA4S.Db
{
    [System.Serializable]
    public class DatabaseInfoData : IData
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Version { get; set; }
        public int PlayerId { get; set; }
        public int CreationTimestamp { get; set; }

        public DatabaseInfoData()
        {
        }

        public DatabaseInfoData(string _Version, int _PlayerId)
        {
            this.Id = 1;  // Only one record
            this.Version = _Version;
            this.PlayerId = _PlayerId;
            this.CreationTimestamp = GenericUtilities.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return string.Format("ID{0},V{1},P{2},Ts{3}",
                Id,
                Version,
                PlayerId,
                CreationTimestamp
            );
        }

    }
}