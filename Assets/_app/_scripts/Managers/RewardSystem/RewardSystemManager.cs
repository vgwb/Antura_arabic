using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EA4S {
    public static class RewardSystemManager {

        #region CONST
        public const string ANTURA_REWARDS_CONFIG_PATH = "Configs/" + "AnturaRewardsConfig";
        public const string COLOR_PAIRS_CONFIG_PATH = "Configs/" + "ColorPairs";
        public const string ANTURA_REWARDS_PREFABS_PATH = "Prefabs/Rewards/";
        #endregion

        #region Configurations
        /// <summary>
        /// The configuration
        /// </summary>
        static RewardConfig config;

        /// <summary>
        /// GetConfig if not already loaded load it from disk.
        /// </summary>
        /// <returns></returns>
        public static RewardConfig GetConfig() {
            if (config == null)
                LoadFromConfig();
            return config;
        }

        /// <summary>
        /// Init
        /// </summary>
        public static void Init() {
            LoadFromConfig();
        }

        /// <summary>
        /// Loads from configuration.
        /// </summary>
        static void LoadFromConfig() {
            TextAsset configData = Resources.Load(ANTURA_REWARDS_CONFIG_PATH) as TextAsset;
            string configString = configData.text;
            config = JsonUtility.FromJson<RewardConfig>(configString);

        }
        #endregion

        #region API

        #region UI Interactions

        /// <summary>
        /// Gets the reward items by category (always 9 items, if not presente item is null).
        /// </summary>
        /// <param name="_categoryId">The category identifier.</param>
        public static List<RewardItem> GetRewardItemsByCategory(string _categoryRewardId, RewardTypes _rewardType) {
            List<RewardItem> returnList = new List<RewardItem>();
            /// TODO: logic
            /// Charge models
            return returnList;
        }

        /// <summary>
        /// Selects the reward item.
        /// </summary>
        /// <param name="_rewardItemId">The reward item identifier.</param>
        /// <returns></returns>
        public static List<RewardColor> SelectRewardItem(string _rewardItemId, RewardTypes _rewardType) {
            List<RewardColor> returnList = new List<RewardColor>();
            /// TODO: logic
            return returnList;
        }

        /// <summary>
        /// Gets the reward colors by identifier.
        /// </summary>
        /// <param name="_rewardItemId">The reward item identifier.</param>
        /// <returns></returns>
        public static List<RewardColor> GetRewardColorsById(string _rewardItemId, RewardTypes _rewardType) {
            List<RewardColor> returnList = new List<RewardColor>();
            // TODO: logic
            return returnList;
        }
        
        #endregion

        /// <summary>
        /// Gets the reward by identifier.
        /// </summary>
        /// <param name="_rewardId">The reward identifier.</param>
        /// <returns></returns>
        public static Reward GetRewardById(string _rewardId) {
            Reward reward = config.Rewards.Find(r => r.ID == _rewardId);
            return reward;
        }

        /// <summary>
        /// Get material pair from Reward id and colors name.
        /// </summary>
        /// <param name="_rewardId"></param>
        /// <param name="_color1"></param>
        /// <param name="_color2"></param>
        /// <returns></returns>
        public static MaterialPair GetMaterialPairFromRewardAndColor(string _rewardId, string _color1, string _color2) {
            Reward reward = RewardSystemManager.GetRewardById(_rewardId);
            MaterialPair mp = new MaterialPair(_color1, reward.Material1, _color2, reward.Material2);
            return mp;
        }

        #endregion
    }

    #region rewards data structures

    #region static DB
    [Serializable]
    public class RewardConfig {
        public List<Reward> Rewards;
        public List<RewardColor> RewardsColorPairs;
    }

    [Serializable]
    public class Reward {
        public string ID;
        public string RewardName;
        public string BoneAttach;
        public string Material1;
        public string Material2;
        public string Category;
        public string RemTongue;
    }

    [Serializable]
    public class RewardColor {
        public string ID;
        public string Color1Name;
        public string Color2Name;
        public string Color1RGB;
        public string Color2RGB;
    }
    #endregion

    #region reward UI data structures    
    /// <summary>
    /// Structure focused to comunicate about items from e to UI.
    /// </summary>
    public class RewardItem {
        public string ID;
        public bool IsSelected;
        public bool IsNew;
    }

    /// <summary>
    /// Structure focused to comunicate about colors from e to UI.
    /// </summary>
    /// <seealso cref="EA4S.RewardColor" />
    public class RewardColorItem : RewardColor {
        public bool IsSelected;
        public bool IsNew;
    }

    #endregion

    public enum RewardTypes {
        reward,
        texture,
        decal,
    }

    #endregion
}