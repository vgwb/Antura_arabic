using Antura.Core;
using Antura.Helpers;
using Antura.Rewards;
using Antura.Tutorial;
using SQLite;

namespace Antura.Database
{
    /// <summary>
    /// Serialized data relative to a reward, used for unlocking. Updated at runtime.
    /// </summary>
    [System.Serializable]
    public class RewardPackUnlockData : IData, IDataEditable
    {
        /// <summary>
        /// Primary key for the database.
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// Unique identifier for the player. empty during game. compiled at export/import
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Identifier of the application session.
        /// </summary>
        public int AppSession { get; set; }

        #region Reward Keys

        /// <summary>
        /// Part of the keys used to define the complete reward.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Part of the keys used to define the complete reward.
        /// </summary>
        public string ColorId { get; set; }

        /// <summary>
        /// Part of the keys used to define the complete reward.
        /// </summary>
        public RewardTypes Type { get; set; }

        #endregion

        /// <summary>
        /// Stage at which the reward data has been unlocked.
        /// </summary>
        public int Stage { get; set; }

        /// <summary>
        /// LearningBlock at which the reward data has been unlocked.
        /// </summary>
        public int LearningBlock { get; set; }

        /// <summary>
        /// PlaySession at which the reward data has been unlocked.
        /// </summary>
        public int PlaySession { get; set; }

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

        /// <summary>
        /// Timestamp of creation of the reward.
        /// </summary>
        public int Timestamp { get; set; }

        /// <summary>
        /// JSON-serialized additional data, may be added as needed.
        /// </summary>
        public string AdditionalData { get; set; }

        public RewardPackUnlockData()
        {
        }

        public RewardPackUnlockData(int appSession, string itemId, string colorId, RewardTypes type, JourneyPosition journeyPosition)
        {
            AppSession = appSession;
            ItemId = itemId;
            ColorId = colorId;
            Type = type;
            Id = GetIdAccordingToDBRules();
            Stage = journeyPosition.Stage;
            LearningBlock = journeyPosition.LearningBlock;
            PlaySession = journeyPosition.PlaySession;
            Order = 0;
            IsNew = true;
            IsLocked = true;
            Timestamp = GenericHelper.GetTimestampForNow();
        }

        public string GetIdAccordingToDBRules()
        {
            return ItemId + "." + ColorId + "." + Type;
        }

        #region Rewards API

        public MaterialPair GetMaterialPair()
        {
            return RewardSystemManager.GetMaterialPairFromRewardIdAndColorId(ItemId, ColorId);
        }

        public Reward GetReward()
        {
            if (Type != RewardTypes.reward) {
                return null;
            }
            return RewardSystemManager.GetConfig().Rewards.Find(r => r.ID == ItemId);
        }

        public string GetRewardCategory()
        {
            if (Type != RewardTypes.reward) {
                return string.Empty;
            }
            Reward reward = RewardSystemManager.GetConfig().Rewards.Find(r => r.ID == ItemId);
            if (reward != null) {
                return reward.Category;
            }
            return string.Empty;
        }

        public JourneyPosition GetJourneyPosition()
        {
            return new JourneyPosition(Stage, LearningBlock, PlaySession);
        }

        #endregion

        #region Database API

        public string GetId()
        {
            return Id;
        }

        public void SetId(string _Id)
        {
            Id = _Id;
        }

        public override string ToString()
        {
            return string.Format("{0} : {1} [{2}] [{3}]", ItemId, ColorId, Type, PlaySession);
        }

        #endregion
    }
}