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
        /// <summary>
        /// The maximum rewards unlockable for playsession.
        /// </summary>
        public const int MaxRewardsUnlockableForPlaysession = 2;
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

        public static RewardPack CurrentReward = new RewardPack();

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
                            string texturePath = "AnturaStuff/Textures_and_Materials/";
                            Texture2D inputTexture = Resources.Load<Texture2D>(texturePath + returnList[i].ID);
                            _parentsTransForModels[i].GetComponent<RawImage>().texture = inputTexture;
                        }
                    }
                    break;
                case RewardTypes.decal:
                    List<RewardDecal> RewardsDecal = config.RewardsDecal;
                    int countD = 0;
                    foreach (RewardDecal reward in RewardsDecal) {
                        countD++;
                        //if (countD == 1)
                        //    returnList.Add(new RewardItem() { ID = reward.ID, IsNew = false, IsSelected = true });
                        //else if (countD == 2)
                        //    returnList.Add(new RewardItem() { ID = reward.ID, IsNew = true, IsSelected = false });
                        //else if (countD == RewardsDecal.Count)
                        //    returnList.Add(null);
                        //else
                            returnList.Add(new RewardItem() { ID = reward.ID, IsNew = false, IsSelected = false });
                    }
                    /// - Charge texture
                    for (int i = 0; i < returnList.Count; i++) {
                        if (returnList[i] != null) {
                            string texturePath = "AnturaStuff/Textures_and_Materials/";
                            Texture2D inputTexture = Resources.Load<Texture2D>(texturePath + returnList[i].ID);
                            _parentsTransForModels[i].GetComponent<RawImage>().texture = inputTexture;
                        }
                    }
                    break;
                default:
                    Debug.LogWarningFormat("Reward typology requested {0} not found", _rewardType);
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
            /// logic
            /// - Trigger selected reward event.
            /// - Load returnList of color for reward checking unlocked and if exist active one


            switch (_rewardType) {
                case RewardTypes.reward:
                    // TODO: filter color selected from unlocked only
                    foreach (RewardColor color in config.RewardsColorPairs) {
                        RewardColorItem rci = new RewardColorItem(color);
                        ///...
                        returnList.Add(rci);
                    }
                    // set current reward in modification
                    CurrentReward = new RewardPack() { ItemID = _rewardItemId, Type = RewardTypes.reward };
                    return returnList;
                case RewardTypes.texture:
                    foreach (RewardColor color in config.RewardsTileColor) {
                        RewardColorItem rci = new RewardColorItem(color);
                        rci.Color2RGB = rci.Color1RGB; // to avoid exadecimal conversion error on ui rgb code conversion.
                        returnList.Add(rci);
                    }
                    // set current reward in modification
                    CurrentReward = new RewardPack() { ItemID = _rewardItemId, Type = RewardTypes.texture };
                    break;
                case RewardTypes.decal:
                    foreach (RewardColor color in config.RewardsDecalColor) {
                        RewardColorItem rci = new RewardColorItem(color);
                        rci.Color2RGB = rci.Color1RGB; // to avoid exadecimal conversion error on ui rgb code conversion.
                        returnList.Add(rci);
                    }
                    // set current reward in modification
                    CurrentReward = new RewardPack() { ItemID = _rewardItemId, Type = RewardTypes.decal };
                    break;
                default:
                    Debug.LogWarningFormat("Reward typology requested {0} not found", _rewardType);
                    break;
            }
            
            return returnList;
        }

        /// <summary>
        /// Selects the reward color item.
        /// </summary>
        /// <param name="_rewardColorItemId">The reward color item identifier.</param>
        /// <param name="_rewardType">Type of the reward.</param>
        public static void SelectRewardColorItem(string _rewardColorItemId, RewardTypes _rewardType) {
            CurrentReward.ColorId = _rewardColorItemId;
            if (OnRewardChanged != null)
                OnRewardChanged(CurrentReward);
        }

        public static void DeselectAllRewardItemsForCategory(string _categoryRewardId = "") {
            AnturaModelManager.Instance.ClearLoadedRewardInCategory(_categoryRewardId);
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
        [Obsolete("...", true)]
        public static MaterialPair GetMaterialPairFromRewardAndColor(string _rewardId, string _color1, string _color2) {
            Reward reward = RewardSystemManager.GetRewardById(_rewardId);
            MaterialPair mp = new MaterialPair(_color1, reward.Material1, _color2, reward.Material2);
            return mp;
        }

        /// <summary>
        /// Gets the material pair for standard reward.
        /// </summary>
        /// <param name="_rewardId">The reward identifier.</param>
        /// <param name="_colorId">The color identifier.</param>
        /// <returns></returns>
        public static MaterialPair GetMaterialPairFromRewardIdAndColorId(string _rewardId, string _colorId) {
            Reward reward = RewardSystemManager.GetRewardById(_rewardId);
            RewardColor color = config.RewardsColorPairs.Find(c => c.ID == _colorId);
            MaterialPair mp = new MaterialPair(color.Color1Name, reward.Material1, color.Color2Name, reward.Material2);
            return mp;
        }

        #endregion

        #endregion

        #region RewardAI                
        /// <summary>
        /// Gets the reward packs for play session ended, already created or create on fly now (and save on player profile).
        /// </summary>
        /// <param name="_playSession">The play session.</param>
        /// <param name="_itemsToUnlock">The items to unlock. Needed to know if must be saved element as unlocked or not.</param>
        /// <param name="_alreadyUnlocked">The already unlocked.</param>
        /// <returns></returns>
        public static List<RewardPack> GetRewardPacksForPlaySession(JourneyPosition _playSession, int _itemsToUnlock, out int _alreadyUnlocked) {
            List<RewardPack> rpList = AppManager.I.Player.RewardsUnlocked.FindAll(r => r.PlaySessionId == _playSession.ToString());
            _alreadyUnlocked = rpList.Count;
            int count = _alreadyUnlocked;
            while (rpList.Count < MaxRewardsUnlockableForPlaysession) {
                RewardPack newRewardPack = GetNextNextRewardPack(RewardTypes.reward);
                count++;
                if (count <= _itemsToUnlock) {
                    // Then this new reward is unlocked by gameplay result and after creation must be saved as unlocked to profile.
                    AppManager.I.Player.RewardsUnlocked.Add(newRewardPack);
                    AppManager.I.Player.Save();
                }
                rpList.Add(newRewardPack);
            }
            return rpList;
        }

        /// <summary>
        /// Gets the next reward pack. Contains all logic to create new reward.
        /// </summary>
        /// <param name="_rewardType">Type of the reward.</param>
        /// <returns></returns>
        public static RewardPack GetNextNextRewardPack(RewardTypes _rewardType) {
            /// TODOs:
            /// - Filter without already unlocked items
            /// - Automatic select reward type by situation
            RewardPack rp = new RewardPack();
            switch (_rewardType) {
                case RewardTypes.reward:
                    rp = new RewardPack() {
                        ItemID = config.Rewards.GetRandom().ID,
                        ColorId = config.RewardsColorPairs.GetRandom().ID,
                        Type = _rewardType,
                        PlaySessionId = AppManager.I.Player.CurrentJourneyPosition.ToString(),
                        IsNew = true,
                    };
                    break;
                case RewardTypes.texture:
                    rp = new RewardPack() {
                        ItemID = config.RewardsTile.GetRandom().ID,
                        ColorId = config.RewardsTileColor.GetRandom().ID,
                        Type = _rewardType,
                        PlaySessionId = AppManager.I.Player.CurrentJourneyPosition.ToString(),
                        IsNew = true,
                    };
                    break;
                case RewardTypes.decal:
                    rp = new RewardPack() {
                        ItemID = config.RewardsDecal.GetRandom().ID,
                        ColorId = config.RewardsDecalColor.GetRandom().ID,
                        Type = _rewardType,
                        PlaySessionId = AppManager.I.Player.CurrentJourneyPosition.ToString(),
                        IsNew = true,
                    };
                    break;
                default:
                    break;
            }
            return rp;
        }
            

        #endregion

        #region Events

        public delegate void RewardSystemEventHandler(RewardPack _rewardPack);

        /// <summary>
        /// Occurs when [on reward item changed].
        /// </summary>
        public static event RewardSystemEventHandler OnRewardChanged;
        

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

    #region Dynamic DB

    /// <summary>
    /// Class structure to identify reward pack used as price in game.
    /// </summary>
    [Serializable]
    public class RewardPack {
        public string ItemID;
        public string ColorId;
        public RewardTypes Type;
        /// <summary>
        /// The play session id where this reward is assigned.
        /// </summary>
        public string PlaySessionId;
        /// <summary>
        /// The order of playsession rewards in case of multi reward for same playsession.
        /// </summary>
        public int Order = 0;
        /// <summary>
        /// True if nevere used by player.
        /// </summary>
        public bool IsNew = true;

        public MaterialPair GetMaterialPair() {
            return RewardSystemManager.GetMaterialPairFromRewardIdAndColorId(ItemID, ColorId);
        }

        public Reward GetReward() {
            if (Type != RewardTypes.reward)
                return null;
            return RewardSystemManager.GetConfig().Rewards.Find(r => r.ID == ItemID);
        }

        public string GetRewardCategory() {
            if (Type != RewardTypes.reward)
                return string.Empty;
            Reward reward = RewardSystemManager.GetConfig().Rewards.Find(r => r.ID == ItemID);
            if (reward != null)
                return reward.Category;
            return string.Empty;
        }
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
        public RewardColorItem() { }
        public RewardColorItem(RewardColor _color) {
            ID = _color.ID;
            Color1Name = _color.Color1Name;
            Color1RGB = _color.Color1RGB;
            Color2Name = _color.Color2Name;
            Color2RGB = _color.Color2RGB;
        }

    }

    #endregion

    public enum RewardTypes {
        reward,
        texture,
        decal,
    }



    #endregion
}