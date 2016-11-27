using UnityEngine;
using UnityEngine.UI;
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
        /// Gets the reward items by rewardType (always 9 items, if not presente item in the return list is null).
        /// </summary>
        /// <param name="_rewardType">Type of the reward.</param>
        /// <param name="_parentsTransForModels">The parents trans for models.</param>
        /// <param name="_categoryRewardId">The category reward identifier.</param>
        /// <returns></returns>
        public static List<RewardItem> GetRewardItemsByRewardType(RewardTypes _rewardType, List<Transform> _parentsTransForModels, string _categoryRewardId = "") {
            List<RewardItem> returnList = new List<RewardItem>();
            /// TODO: logic
            /// - Load returnList by type and category checking unlocked and if exist active one
            switch (_rewardType) {
                case RewardTypes.reward:
                    // TODO: get reward from db of unlocked rewards
                    List<Reward> rewards = config.Rewards.FindAll(r => r.Category == _categoryRewardId);
                    int count = 0;
                    foreach (Reward reward in rewards) {
                        count++;
                        if (count == 1)
                            returnList.Add(new RewardItem() { ID = reward.ID, IsNew = false, IsSelected = true });
                        else if (count == 2)
                            returnList.Add(new RewardItem() { ID = reward.ID, IsNew = true, IsSelected = false });
                        else if (count == rewards.Count)
                            returnList.Add(null);
                        else
                            returnList.Add(new RewardItem() { ID = reward.ID, IsNew = false, IsSelected = false });
                    }
                    /// - Charge models
                    for (int i = 0; i < returnList.Count; i++) {
                        if (returnList[i] != null) {
                            ModelsManager.MountModel(returnList[i].ID, _parentsTransForModels[i]);
                        }
                    }
                    break;
                case RewardTypes.texture:
                    List<RewardTile> rewardsTiles = config.RewardsTile;
                    int countT = 0;
                    foreach (RewardTile reward in rewardsTiles) {
                        countT++;
                        if (countT == 1)
                            returnList.Add(new RewardItem() { ID = reward.ID, IsNew = false, IsSelected = true });
                        else if (countT == 2)
                            returnList.Add(new RewardItem() { ID = reward.ID, IsNew = true, IsSelected = false });
                        else if (countT == rewardsTiles.Count)
                            returnList.Add(null);
                        else
                            returnList.Add(new RewardItem() { ID = reward.ID, IsNew = false, IsSelected = false });
                    }
                    /// - Charge texture
                    for (int i = 0; i < returnList.Count; i++) {
                        if (returnList[i] != null) {
                            Texture2D inputTexture = Resources.Load<Texture2D>(returnList[i].ID);
                            _parentsTransForModels[i].GetComponent<RawImage>().texture = inputTexture;
                        }
                    }
                    break;
                case RewardTypes.decal:
                    List<RewardDecal> RewardsDecal = config.RewardsDecal;
                    int countD = 0;
                    foreach (RewardDecal reward in RewardsDecal) {
                        countD++;
                        if (countD == 1)
                            returnList.Add(new RewardItem() { ID = reward.ID, IsNew = false, IsSelected = true });
                        else if (countD == 2)
                            returnList.Add(new RewardItem() { ID = reward.ID, IsNew = true, IsSelected = false });
                        else if (countD == RewardsDecal.Count)
                            returnList.Add(null);
                        else
                            returnList.Add(new RewardItem() { ID = reward.ID, IsNew = false, IsSelected = false });
                    }
                    break;
                default:
                    Debug.LogWarningFormat("Reward typology {0} not found", _rewardType);
                    break;
            }

            //// add empty results
            //int emptyItemsCount = _parentsTransForModels.Count - returnList.Count;
            //for (int i = 0; i < emptyItemsCount; i++) {
            //    returnList.Add(null);
            //}
            return returnList;
        }

        /// <summary>
        /// Selects the reward item.
        /// </summary>
        /// <param name="_rewardItemId">The reward item identifier.</param>
        /// <returns></returns>
        public static List<RewardColorItem> SelectRewardItem(string _rewardItemId, RewardTypes _rewardType) {
            List<RewardColorItem> returnList = new List<RewardColorItem>();
            GetRewardColorsById(_rewardItemId, _rewardType);
            /// TODO: logic
            /// - Load returnList of color for reward checking unlocked and if exist active one
            return returnList;
        }

        /// <summary>
        /// Selects the reward color item.
        /// </summary>
        /// <param name="_rewardColorItemId">The reward color item identifier.</param>
        /// <param name="_rewardType">Type of the reward.</param>
        public static void SelectRewardColorItem(string _rewardColorItemId, RewardTypes _rewardType) {
            // TODO: logic
        }

        /// <summary>
        /// TODO: public or private?
        /// Gets the reward colors by identifier.
        /// </summary>
        /// <param name="_rewardItemId">The reward item identifier.</param>
        /// <returns></returns>
        static List<RewardColorItem> GetRewardColorsById(string _rewardItemId, RewardTypes _rewardType) {
            List<RewardColorItem> returnList = new List<RewardColorItem>();
            // TODO: logic
            return returnList;
        }

        #endregion

        #region General

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

        #endregion
    }

    #region rewards data structures

    #region static DB

    [Serializable]
    public class RewardConfig {
        public List<Reward> Rewards;
        public List<RewardColor> RewardsColorPairs;
        public List<RewardDecal> RewardsDecal;
        public List<RewardColor> RewardsDecalColor;
        public List<RewardTile> RewardsTile;
        public List<RewardColor> RewardsTileColor;
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
        public string Color1RGB; // "rrggbbaa"
        public string Color2RGB; // "rrggbbaa"
    }

    [Serializable]
    public class RewardDecal {
        public string ID;
    }

    [Serializable]
    public class RewardTile {
        public string ID;
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