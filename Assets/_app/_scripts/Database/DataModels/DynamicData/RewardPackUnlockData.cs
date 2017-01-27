using EA4S.Core;
using EA4S.Rewards;
using EA4S.Utilities;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Serialized data relative to a reward, used for unlocking. Updated at runtime.
    /// </summary>
    [System.Serializable]
    public class RewardPackUnlockData : IData
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string ItemID { get; set; }
        public string ColorId { get; set; }
        public RewardTypes Type { get; set; }

        /// <summary>
        /// The play session id where this reward is assigned.
        /// </summary>
        public string PlaySessionId { get; set; }

        /// <summary>
        /// The order of playsession rewards in case of multi reward for same playsession.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// True if never used by player.
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// True if not unlocked yet
        /// </summary>
        public bool IsLocked { get; set; }

        public int CreationTimestamp { get; set; }

        public RewardPackUnlockData()
        {
        }

        public RewardPackUnlockData(string _ItemID, string _ColorId, RewardTypes _Type, string _PlaySessionId)
        {
            Id = _ItemID + "." + _ColorId + "." + _Type;
            ItemID = _ItemID;
            ColorId = _ColorId;
            Type = _Type;
            PlaySessionId = PlaySessionId;
            Order = 0;
            IsNew = true;
            IsLocked = true;
            CreationTimestamp = GenericUtilities.GetTimestampForNow();
        }


        #region Rewards API

        public MaterialPair GetMaterialPair()
        {
            return RewardSystemManager.GetMaterialPairFromRewardIdAndColorId(ItemID, ColorId);
        }

        public Reward GetReward()
        {
            if (Type != RewardTypes.reward)
                return null;
            return RewardSystemManager.GetConfig().Rewards.Find(r => r.ID == ItemID);
        }

        public string GetRewardCategory()
        {
            if (Type != RewardTypes.reward)
                return string.Empty;
            Reward reward = RewardSystemManager.GetConfig().Rewards.Find(r => r.ID == ItemID);
            if (reward != null)
                return reward.Category;
            return string.Empty;
        }

        #endregion

        #region Database API

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("{0} : {1} [{2}] [{3}]", ItemID, ColorId, Type, PlaySessionId);
        }

        #endregion
    }
}