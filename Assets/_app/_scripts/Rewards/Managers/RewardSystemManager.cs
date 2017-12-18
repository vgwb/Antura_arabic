using Antura.Core;
using Antura.Database;
using Antura.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.Rewards
{
    public class RewardSystemManager
    {
        private const string ANTURA_REWARDS_ITEMS_CONFIG_PATH = "Configs/AnturaRewardsItemsConfig";
        private const string ANTURA_REWARDS_UNLOCKS_CONFIG_PATH = "Configs/AnturaRewardsUnlocksConfig";
        //public const string COLOR_PAIRS_CONFIG_PATH = "Configs/" + "ColorPairs";
        public const string ANTURA_REWARDS_PREFABS_PATH = "Prefabs/Rewards/";

        /// <summary>
        /// The maximum rewards unlockable for playsession.
        /// </summary>
        // TODO: use this? public const int MaxRewardsUnlockableForPlaysession = 2;

        #region Events

        public delegate void RewardSystemEventHandler(RewardPackUnlockData rewardPackUnlockData);

        public static event RewardSystemEventHandler OnRewardChanged;

        public static event RewardSystemEventHandler OnNewRewardUnlocked;

        #endregion

        #region Configurations (i.e. WHAT and WHEN we can unlock rewards)

        public void Init()
        {
            LoadConfigs();
        }

        /// <summary>
        /// The configuration of items that can be unlocked
        /// </summary>
        RewardsItemsConfig itemsConfig;

        /// <summary>
        /// The configuration of items that can be unlocked
        /// </summary>
        RewardsUnlocksConfig unlocksConfig;

        /// <summary>
        /// GetConfig if not already loaded load it from disk.
        /// </summary>
        /// <returns></returns>
        public RewardsItemsConfig ItemsConfig
        {
            get
            {
                return itemsConfig;
            }
        }

        /// <summary>
        /// Loads the reward system configurations
        /// </summary>
        private void LoadConfigs()
        {
            TextAsset itemsConfigData = Resources.Load(ANTURA_REWARDS_ITEMS_CONFIG_PATH) as TextAsset;
            itemsConfig = JsonUtility.FromJson<RewardsItemsConfig>(itemsConfigData.text);

            TextAsset unlocksConfigData = Resources.Load(ANTURA_REWARDS_ITEMS_CONFIG_PATH) as TextAsset;
            unlocksConfig = JsonUtility.FromJson<RewardsUnlocksConfig>(unlocksConfigData.text);
        }

        /// <summary>
        /// Gets the total count of all rewards. 
        /// Any type with any color variation available in game.
        /// </summary>
        public int GetTotalRewardsCount()
        {
            int total = 0;
            total += ItemsConfig.PropBases.Count * ItemsConfig.PropColors.Count;
            total += ItemsConfig.DecalBases.Count * ItemsConfig.DecalColors.Count;
            total += ItemsConfig.TextureBases.Count * ItemsConfig.TextureColors.Count;
            return total;
        }

        /// <summary>
        /// Gets a reward by its string identifier.
        /// </summary>
        /// <param name="_rewardId">The reward identifier.</param>
        /// <returns></returns>
        public RewardProp GetPropRewardById(string _rewardId)
        {
            RewardProp reward = ItemsConfig.PropBases.Find(r => r.ID == _rewardId);
            return reward;
        }

        /// <summary>
        /// Get material pair from Reward id and colors name.
        /// </summary>
        /// <param name="_rewardId"></param>
        /// <param name="_color1"></param>
        /// <param name="_color2"></param>
        /// <returns></returns>
        /*[Obsolete("...", true)]
        public static MaterialPair GetMaterialPairFromRewardAndColor(string _rewardId, string _color1, string _color2)
        {
            Reward reward = GetRewardById(_rewardId);
            MaterialPair mp = new MaterialPair(_color1, reward.Material1, _color2, reward.Material2);
            return mp;
        }*/

        /// <summary>
        /// Gets the material pair for standard reward.
        /// </summary>
        /// <param name="_rewardId">The reward identifier.</param>
        /// <param name="_colorId">The color identifier.</param>
        /// <returns></returns>
        public MaterialPair GetMaterialPairFromRewardIdAndColorId(string _rewardId, string _colorId)
        {
            RewardProp reward = GetPropRewardById(_rewardId);
            RewardColor color = ItemsConfig.PropColors.Find(c => c.ID == _colorId);
            if (color == null || reward == null)
            {
                return new MaterialPair();
            }
            MaterialPair mp = new MaterialPair(color.Color1Name, reward.Material1, color.Color2Name, reward.Material2);
            return mp;
        }

        #endregion

        #region Unlocked Rewards

        /// <summary>
        /// Gets the unlocked reward count for the current player. 0 if current player is null.
        /// </summary>
        /// <returns></returns>
        public int GetUnlockedRewardsCount()
        {
            return AppManager.I.Player != null ? AppManager.I.Player.RewardsUnlocked.Count : 0;
        }

        /// <summary>
        /// Gets the unlocked reward for specified playsession.
        /// </summary>
        /// <param name="journeyPosition">The playsession identifier (format 1.4.2).</param>
        /// <returns></returns>
        public int GetNumberOfUnlockedRewardsAtJP(JourneyPosition journeyPosition)
        {
            int rCount = AppManager.I.Player.RewardsUnlocked.FindAll(ur => ur.GetJourneyPosition().Equals(journeyPosition)).Count;
            return rCount > 2 ? 2 : rCount; // max 2 becasue the results screen does not allow for more
        }

        // this unlocks ALL rewards that have not been unlocked yet
        public IEnumerator UnlockAllMissingRewards()
        {
            JourneyPosition extraRewardJourney = new JourneyPosition(100, 100, 100);
            List<RewardPackUnlockData> alreadyUnlocked = AppManager.I.Player.RewardsUnlocked;
            for (int i = 0; i < ItemsConfig.PropBases.Count; i++)
            {
                for (int y = 0; y < ItemsConfig.PropColors.Count; y++)
                {
                    if (!alreadyUnlocked.Exists(ur =>
                        ur.ItemId == ItemsConfig.PropBases[i].ID && ur.ColorId == ItemsConfig.PropColors[y].ID))
                    {
                        AppManager.I.Player.AddRewardUnlocked(
                            new RewardPackUnlockData(
                                AppManager.I.LogManager.AppSession,
                                ItemsConfig.PropBases[i].ID,
                                ItemsConfig.PropColors[y].ID, RewardTypes.reward, extraRewardJourney));
                        yield return null;
                    }
                }
            }
            for (int i = 0; i < ItemsConfig.DecalBases.Count; i++)
            {
                for (int y = 0; y < ItemsConfig.DecalColors.Count; y++)
                {
                    if (!alreadyUnlocked.Exists(ur =>
                        ur.ItemId == ItemsConfig.DecalBases[i].ID && ur.ColorId == ItemsConfig.DecalColors[y].ID))
                    {
                        AppManager.I.Player.AddRewardUnlocked(
                            new RewardPackUnlockData(
                                AppManager.I.LogManager.AppSession,
                                ItemsConfig.DecalBases[i].ID,
                                ItemsConfig.DecalColors[y].ID, RewardTypes.decal, extraRewardJourney));
                        yield return null;
                    }
                }
            }
            for (int i = 0; i < ItemsConfig.TextureBases.Count; i++)
            {
                for (int y = 0; y < ItemsConfig.TextureColors.Count; y++)
                {
                    if (!alreadyUnlocked.Exists(ur =>
                        ur.ItemId == ItemsConfig.TextureBases[i].ID && ur.ColorId == ItemsConfig.TextureColors[y].ID))
                    {
                        AppManager.I.Player.AddRewardUnlocked(
                            new RewardPackUnlockData(
                                AppManager.I.LogManager.AppSession,
                                ItemsConfig.TextureBases[i].ID,
                                ItemsConfig.TextureColors[y].ID, RewardTypes.texture, extraRewardJourney));
                        yield return null;
                    }
                }
            }
            yield return null;
        }

        /// <summary>
        /// Unlocks all rewards.
        /// </summary>
        public void UnlockAllRewards()
        {
            int RewardCount = 0;
            int TextureCount = 0;
            int DecalCount = 0;
            int OtherCount = 0;
            var actualCurrentJourneyPosition = AppManager.I.Player.CurrentJourneyPosition;
            var allPlaySessionInfos = AppManager.I.ScoreHelper.GetAllPlaySessionInfo();

            for (int i = 0; i < allPlaySessionInfos.Count; i++)
            {
                // Check if already unlocked reward for this playSession.
                JourneyPosition journeyPosition = allPlaySessionInfos[i].data.GetJourneyPosition();
                if (RewardAlreadyUnlocked(journeyPosition)) { continue; }
                List<RewardPackUnlockData> newUnlocked = new List<RewardPackUnlockData>();
                AppManager.I.Player.SetCurrentJourneyPosition(
                    AppManager.I.JourneyHelper.PlaySessionIdToJourneyPosition(allPlaySessionInfos[i].data.Id));
                foreach (RewardPackUnlockData pack in GetNextRewardPack())
                {
                    pack.IsLocked = false;
                    //AppManager.I.Player.AddRewardUnlockedAll(pack);

                    newUnlocked.Add(pack);

                    switch (pack.Type)
                    {
                        case RewardTypes.reward:
                            RewardCount++;
                            break;
                        case RewardTypes.texture:
                            TextureCount++;
                            break;
                        case RewardTypes.decal:
                            DecalCount++;
                            break;
                        default:
                            OtherCount++;
                            break;
                    }
                    Debug.LogFormat("Unlocked reward for playsession {0} : {1}", journeyPosition, pack);
                }
                AppManager.I.Player.AddRewardUnlockedRange(newUnlocked);
            }


            AppManager.I.Player.SetCurrentJourneyPosition(actualCurrentJourneyPosition);
            Debug.LogFormat("Bulk unlocking rewards result: rewards: {0} | texture: {1} | decal: {2} | other: {3}", RewardCount,
                TextureCount, DecalCount, OtherCount);

            AppManager.I.StartCoroutine(UnlockAllMissingRewards());
            Debug.LogFormat("Unlock also all extra rewards!");
            Init();
        }

        /// <summary>
        /// Return true if Reward for this JourneyPosition is already unlocked.
        /// </summary>
        /// <param name="_journeyPosition">The journey position.</param>
        /// <returns></returns>
        public bool RewardAlreadyUnlocked(JourneyPosition journeyPosition)
        {
            List<RewardPackUnlockData> unlocked = AppManager.I.Player.RewardsUnlocked;
            RewardPackUnlockData rewardPackUnlockData = unlocked.Find(r => r.GetJourneyPosition().Equals(journeyPosition));
            if (rewardPackUnlockData != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Rewards the already unlocked.
        /// </summary>
        /// <param name="_itemId">The item identifier.</param>
        /// <param name="_colorId">The color identifier.</param>
        /// <param name="_type">The type.</param>
        /// <returns></returns>
        public bool RewardAlreadyUnlocked(string _itemId, string _colorId, RewardTypes _type)
        {
            return AppManager.I.Player.RewardsUnlocked.Find(r => r.ItemId == _itemId && r.ColorId == _colorId && r.Type == _type) != null;
        }

        /// <summary>
        /// Return list of right rewards for actual playsession if not already.
        /// </summary>
        /// <returns></returns>
        public List<RewardPackUnlockData> GetNextRewardPack(bool _forceToReturnReward = false)
        {
            var returnList = new List<RewardPackUnlockData>();
            var journeyPosition = AppManager.I.Player.CurrentJourneyPosition;
            // not _forceToReturnReward check if reward is already unlocked for this playsession and if true return empty list
            if (RewardAlreadyUnlocked(journeyPosition) && !_forceToReturnReward)
                return returnList;

            // What kind of reward it is?
            PlaySessionRewardUnlock unlock = ItemsConfig.PlaySessionRewardsUnlock.Find(r => r.PlaySession == journeyPosition.Id);
            if (unlock == null)
            {
                Debug.LogErrorFormat("Unable to find reward type for this playsession {0}", journeyPosition);
            }

            // -- check kind
            if (unlock.RewardColor != string.Empty)
            {
                // Get reward color for reward item already unlocked
                for (int i = 0; i < int.Parse(unlock.RewardColor); i++)
                {
                    returnList.Add(GetRewardPack(journeyPosition, RewardTypes.reward, false));
                }
            }
            else if (unlock.Reward != string.Empty)
            {
                // Get new reward item with random color
                var newItemReward = GetRewardPack(journeyPosition, RewardTypes.reward, true);
                if (OnNewRewardUnlocked != null)
                {
                    OnNewRewardUnlocked(newItemReward);
                }
                returnList.Add(newItemReward);
            }
            else if (unlock.Texture != string.Empty)
            {
                // Get new texture
                returnList.Add(GetRewardPack(journeyPosition, RewardTypes.texture, true));
            }
            else if (unlock.Decal != string.Empty)
            {
                // Get new decal
                returnList.Add(GetRewardPack(journeyPosition, RewardTypes.decal, true));
            }

            ////////////////////////////////////////////////////
            return returnList;
        }

        // OK
        /// <summary>
        /// Gets the next reward pack. Contains all logic to create new reward.
        /// </summary>
        /// <param name="_rewardType">Type of the reward.</param>
        /// <returns></returns>
        public RewardPackUnlockData GetRewardPack(JourneyPosition journeyPosition, RewardTypes _rewardType, bool _random)
        {
            // NDM: _random is actually "does not have it already" and will select colors if false

            /// TODOs:
            /// - Filter without already unlocked items
            /// - Automatic select reward type by situation
            var rp = new RewardPackUnlockData();
            string itemId = string.Empty;
            RewardColor color = null;
            bool alreadyUnlocked = false;
            var alreadyUnlockedRewardOfType = AppManager.I.Player.RewardsUnlocked.FindAll(r => r.Type == _rewardType);
            switch (_rewardType)
            {
                case RewardTypes.reward:
                    int countAvoidInfiniteLoop = 300;
                    // If not random take id from list of already unlocked rewards of this type
                    if (_random)
                    {
                        // HERE: NEW MODEL + COLOR

                        // Need to create new reward and first color pair
                        bool duplicated = false;
                        do
                        {
                            //int count = AppManager.I.Player.GetNotYetUnlockedRewardCountForType(_rewardType);
                            List<RewardProp> availableItems = ItemsConfig.PropBases;
                            // Try fix for #421
                            List<RewardProp> itemsForRandomSelection = availableItems
                                .Where(r => !alreadyUnlockedRewardOfType.Exists(ur => ur.ItemId == r.ID)).ToList();
                            if (itemsForRandomSelection.Count == 0)
                            {
                                duplicated = true;
                                continue;
                            }
                            // ----------------
                            itemId = itemsForRandomSelection.GetRandomAlternative().ID;
                            color = ItemsConfig.PropColors.GetRandomAlternative();
                            List<RewardPackUnlockData> unlocked = AppManager.I.Player.RewardsUnlocked;
                            duplicated = unlocked.Find(r => r.ItemId == itemId) != null;
                            if (duplicated) { Debug.LogFormat("Reward {0} already unlocked! Retry!", itemId); }
                            countAvoidInfiniteLoop--;
                            if (countAvoidInfiniteLoop == 0) { Debug.LogFormat("-------------- Reward {0} infinite loop!!!!", itemId); }
                        } while (duplicated && countAvoidInfiniteLoop > 0);
                        //} while (duplicated && AppManager.I.Player.RewardForTypeAvailableYet(_rewardType) || countAvoidInfiniteLoop < 1) ;
                    }
                    else {
                        // HERE: OLD MODEL + COLOR

                        // need only to create new color pair for one of already unlocked reward
                        color = null;
                        var alreadyUnlockeds = AppManager.I.Player.RewardsUnlocked.Where(r => r.Type == RewardTypes.reward).ToList();
                        var availableRewardIds = new List<string>();
                        foreach (var reward in alreadyUnlockeds)
                        {
                            if (!availableRewardIds.Contains(reward.ItemId))
                            {
                                int alreadyUnlockedsCount = alreadyUnlockeds.Where(r => r.ItemId == reward.ItemId).ToList().Count;
                                if (alreadyUnlockedsCount < ItemsConfig.PropColors.Count)
                                {
                                    // TODO: quick fix
                                    availableRewardIds.Add(reward.ItemId);
                                }
                            }
                        }
                        if (availableRewardIds.Count < 1)
                        {
                            Debug.LogWarning("No rewards with lockable color variation. Unable to unlock new color variation.");
                            return null;
                        }
                        itemId = availableRewardIds.GetRandomAlternative();
                        List<RewardColor> availableColors = ItemsConfig.PropColors;
                        //availableColors.Where(r => !alreadyUnlockeds.Exists(ur => ur.ItemId == itemId && ur.ColorId == r.ID));
                        color = availableColors.Where(r => !alreadyUnlockeds.Exists(ur => ur.ItemId == itemId && ur.ColorId == r.ID))
                            .ToList().GetRandomAlternative();
                    }
                    rp = new RewardPackUnlockData(AppManager.I.LogManager.AppSession, itemId, color.ID, _rewardType, journeyPosition);
                    break;
                case RewardTypes.texture:
                    // HERE: NEW TEXTURE + COLOR
                    do
                    {
                        itemId = ItemsConfig.TextureBases.GetRandomAlternative().ID;
                        color = ItemsConfig.TextureColors.GetRandomAlternative();
                        alreadyUnlocked = RewardAlreadyUnlocked(itemId, color.ID, _rewardType);
                    } while (alreadyUnlocked);
                    rp = new RewardPackUnlockData(AppManager.I.LogManager.AppSession, itemId, color.ID, _rewardType, journeyPosition);
                    break;
                case RewardTypes.decal:
                    // HERE: NEW DECAL + COLOR
                    do
                    {
                        itemId = ItemsConfig.DecalBases.GetRandomAlternative().ID;
                        color = ItemsConfig.DecalColors.GetRandomAlternative();
                        alreadyUnlocked = RewardAlreadyUnlocked(itemId, color.ID, _rewardType);
                    } while (alreadyUnlocked);
                    rp = new RewardPackUnlockData(AppManager.I.LogManager.AppSession, itemId, color.ID, _rewardType, journeyPosition);
                    break;
            }
            return rp;
        }

        // OK
        /// <summary>
        /// Unlocks the first set of rewards for current player.
        /// </summary>
        public void UnlockFirstSetOfRewards(Profile.PlayerProfile _player = null)
        {
            if (_player == null)
            {
                if (AppManager.I.Player == null)
                {
                    Debug.LogError("No current player available!");
                    return;
                }
            }
            _player = AppManager.I.Player;

            _player.ResetRewardsUnlockedData();
            _player.AddRewardUnlocked(GetFirstAnturaReward(RewardTypes.reward));    // 1 model
            // decal
            RewardPackUnlockData defaultDecal = GetFirstAnturaReward(RewardTypes.decal);    // 1 decal
            _player.AddRewardUnlocked(defaultDecal);
            // force to to wear decal
            _player.CurrentAnturaCustomizations.DecalTexture = defaultDecal;
            _player.CurrentAnturaCustomizations.DecalTextureId = defaultDecal.GetIdAccordingToDBRules();
            // texture
            RewardPackUnlockData defaultTexture = GetFirstAnturaReward(RewardTypes.texture);    // 1 texture
            _player.AddRewardUnlocked(defaultTexture);
            // force to to wear texture
            _player.CurrentAnturaCustomizations.TileTexture = defaultTexture;
            _player.CurrentAnturaCustomizations.TileTexture.Id = defaultTexture.GetIdAccordingToDBRules();
            // Add all 3 rewards
            //_player.AddRewardUnlockedAll();
            // Save actual customization
            _player.SaveAnturaCustomization();
        }

        // OK
        /// <summary>
        /// Gets the first antura reward.
        /// </summary>
        /// <param name="_rewardType">Type of the reward.</param>
        /// <returns></returns>
        public RewardPackUnlockData GetFirstAnturaReward(RewardTypes _rewardType)
        {
            // this returns the DEFAULT rewards for one of the three types
            RewardPackUnlockData rp = new RewardPackUnlockData();
            switch (_rewardType)
            {
                case RewardTypes.reward:
                    rp = GetRewardPack(new JourneyPosition(0, 0, 0), _rewardType, true);
                    break;
                case RewardTypes.texture:
                    rp = new RewardPackUnlockData(AppManager.I.LogManager.AppSession, "Antura_wool_tilemat", "color1", _rewardType,
                        new JourneyPosition(0, 0, 0));
                    rp.IsNew = false; // Because is automatically selected
                    break;
                case RewardTypes.decal:
                    rp = new RewardPackUnlockData(AppManager.I.LogManager.AppSession, "Antura_decalmap01", "color1", _rewardType,
                        new JourneyPosition(0, 0, 0));
                    rp.IsNew = false; // Because is automatically selected
                    break;
            }
            return rp;
        }

        #endregion

        #region Customization (i.e. UI, selection, view)

        // used during customization
        private RewardPackUnlockData CurrentSelectedReward = new RewardPackUnlockData();

        // OK
        /// <summary>
        /// Gets the reward items by rewardType (always 9 items, if not presente item in the return list is null).
        /// </summary>
        /// <param name="_rewardType">Type of the reward.</param>
        /// <param name="_parentsTransForModels">The parents trans for models.</param>
        /// <param name="_categoryRewardId">The category reward identifier.</param>
        /// <returns></returns>
        public List<RewardItem> GetRewardItemsByRewardType(RewardTypes _rewardType, List<Transform> _parentsTransForModels,
            string _categoryRewardId = "")
        {
            List<RewardItem> returnList = new List<RewardItem>();
            /// TODO: logic
            /// - Load returnList by type and category checking unlocked and if exist active one
            switch (_rewardType) {
                case RewardTypes.reward:
                    // Filter from unlocked elements (only items with this category and only one for itemID)
                    List<RewardProp> rewards = ItemsConfig.GetClone().PropBases;
                    foreach (var item in rewards.FindAll(r => r.Category == _categoryRewardId)) {
                        if (AppManager.I.Player.RewardsUnlocked.FindAll(ur => ur.GetRewardCategory() == _categoryRewardId)
                            .Exists(ur => ur.ItemId == item.ID)) {
                            returnList.Add(new RewardItem() {
                                ID = item.ID,
                                IsNew = AppManager.I.Player.RewardItemIsNew(item.ID),
                                IsSelected = AppManager.I.Player.CurrentAnturaCustomizations.Fornitures.Exists(f => f.ItemId == item.ID)
                            });
                        } else {
                            returnList.Add(null);
                        }
                    }
                    /// - Charge models
                    for (int i = 0; i < returnList.Count; i++) {
                        if (returnList[i] != null) {
                            ModelsManager.MountModel(returnList[i].ID, _parentsTransForModels[i]);
                        }
                    }
                    break;
                case RewardTypes.texture:
                    // Filter from unlocked elements (only one for itemID)
                    foreach (var item in ItemsConfig.TextureBases) {
                        if (AppManager.I.Player.RewardsUnlocked.FindAll(ur => ur.Type == RewardTypes.texture)
                            .Exists(ur => ur.ItemId == item.ID)) {
                            returnList.Add(new RewardItem() {
                                ID = item.ID,
                                IsNew = AppManager.I.Player.RewardItemIsNew(item.ID),
                                IsSelected = AppManager.I.Player.CurrentAnturaCustomizations.TileTexture.ItemId == item.ID
                            });
                        } else {
                            returnList.Add(null);
                        }
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
                    // Filter from unlocked elements (only one for itemID)
                    foreach (var item in ItemsConfig.DecalBases) {
                        if (AppManager.I.Player.RewardsUnlocked.FindAll(ur => ur.Type == RewardTypes.decal)
                            .Exists(ur => ur.ItemId == item.ID)) {
                            returnList.Add(new RewardItem() {
                                ID = item.ID,
                                IsNew = AppManager.I.Player.RewardItemIsNew(item.ID),
                                IsSelected = AppManager.I.Player.CurrentAnturaCustomizations.DecalTexture.ItemId == item.ID
                            });
                        } else {
                            returnList.Add(null);
                        }
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

        // OK
        /// <summary>
        /// Selects the reward item (in the UI)
        /// </summary>
        /// <param name="_rewardItemId">The reward item identifier.</param>
        /// <returns></returns>
        public List<RewardColorItem> SelectRewardItem(string _rewardItemId, RewardTypes _rewardType)
        {
            List<RewardColorItem> returnList = new List<RewardColorItem>();
            /// logic
            /// - Trigger selected reward event.
            /// - Load returnList of color for reward checking unlocked and if exist active one


            switch (_rewardType) {
                case RewardTypes.reward:
                    foreach (RewardColor color in ItemsConfig.GetClone().PropColors) {
                        if (AppManager.I.Player.RewardsUnlocked.Exists(ur => ur.ItemId == _rewardItemId && ur.ColorId == color.ID)) {
                            RewardColorItem rci = new RewardColorItem(color);
                            rci.IsNew = AppManager.I.Player.RewardsUnlocked.Exists(ur =>
                                ur.ItemId == _rewardItemId && ur.ColorId == color.ID && ur.IsNew == true);
                            returnList.Add(rci);
                        } else {
                            returnList.Add(null);
                        }
                    }
                    // set current reward in modification
                    CurrentSelectedReward = new RewardPackUnlockData() { ItemId = _rewardItemId, Type = RewardTypes.reward };
                    break;
                case RewardTypes.texture:
                    foreach (RewardColor color in ItemsConfig.TextureColors) {
                        if (AppManager.I.Player.RewardsUnlocked.Exists(ur => ur.ItemId == _rewardItemId && ur.ColorId == color.ID)) {
                            RewardColorItem rci = new RewardColorItem(color);
                            rci.IsNew = AppManager.I.Player.RewardsUnlocked.Exists(ur =>
                                ur.ItemId == _rewardItemId && ur.ColorId == color.ID && ur.IsNew == true);
                            rci.Color2RGB = rci.Color1RGB; // to avoid exadecimal conversion error on ui rgb code conversion.
                            returnList.Add(rci);
                        } else {
                            returnList.Add(null);
                        }
                    }
                    // set current reward in modification
                    CurrentSelectedReward = new RewardPackUnlockData() { ItemId = _rewardItemId, Type = RewardTypes.texture };
                    break;
                case RewardTypes.decal:
                    foreach (RewardColor color in ItemsConfig.DecalColors) {
                        if (AppManager.I.Player.RewardsUnlocked.Exists(ur => ur.ItemId == _rewardItemId && ur.ColorId == color.ID)) {
                            RewardColorItem rci = new RewardColorItem(color);
                            rci.IsNew = AppManager.I.Player.RewardsUnlocked.Exists(ur =>
                                ur.ItemId == _rewardItemId && ur.ColorId == color.ID && ur.IsNew == true);
                            rci.Color2RGB = rci.Color1RGB; // to avoid exadecimal conversion error on ui rgb code conversion.
                            returnList.Add(rci);
                        } else {
                            returnList.Add(null);
                        }
                    }
                    //foreach (RewardColor color in config.DecalColors) {
                    //    RewardColorItem rci = new RewardColorItem(color);
                    //    rci.Color2RGB = rci.Color1RGB; // to avoid exadecimal conversion error on ui rgb code conversion.
                    //    returnList.Add(rci);
                    //}
                    // set current reward in modification
                    CurrentSelectedReward = new RewardPackUnlockData() { ItemId = _rewardItemId, Type = RewardTypes.decal };
                    break;
                default:
                    Debug.LogWarningFormat("Reward typology requested {0} not found", _rewardType);
                    break;
            }

            // Color selection
            RewardPackUnlockData alreadySelectedReward = null;
            switch (_rewardType) {
                case RewardTypes.reward:
                    List<RewardPackUnlockData> fornitures = AppManager.I.Player.CurrentAnturaCustomizations.Fornitures;
                    alreadySelectedReward = fornitures.Find(r => r.ItemId == _rewardItemId && r.Type == _rewardType);
                    break;
                case RewardTypes.texture:
                    if (AppManager.I.Player.CurrentAnturaCustomizations.TileTexture.ItemId == _rewardItemId)
                        alreadySelectedReward = AppManager.I.Player.CurrentAnturaCustomizations.TileTexture;
                    break;
                case RewardTypes.decal:
                    if (AppManager.I.Player.CurrentAnturaCustomizations.DecalTexture.ItemId == _rewardItemId)
                        alreadySelectedReward = AppManager.I.Player.CurrentAnturaCustomizations.DecalTexture;
                    break;
                default:
                    Debug.LogErrorFormat("Reward type {0} not found!", _rewardType);
                    return returnList;
            }

            if (alreadySelectedReward != null) {
                // if previous selected this reward use previous color...
                returnList.Find(color => color != null && color.ID == alreadySelectedReward.ColorId).IsSelected = true;
            } else {
                // ...else selecting first available color
                foreach (var firstItem in returnList) {
                    if (firstItem != null) {
                        firstItem.IsSelected = true;
                        return returnList;
                    }
                }
            }

            return returnList;
        }

        // OK
        /// <summary>
        /// Selects the reward color item.
        /// </summary>
        /// <param name="_rewardColorItemId">The reward color item identifier.</param>
        /// <param name="_rewardType">Type of the reward.</param>
        public void SelectRewardColorItem(string _rewardColorItemId, RewardTypes _rewardType)
        {
            CurrentSelectedReward.ColorId = _rewardColorItemId;
            if (OnRewardChanged != null)
                OnRewardChanged(CurrentSelectedReward);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_categoryRewardId"></param>
        /*public static void DeselectAllRewardItemsForCategory(string _categoryRewardId = "")
        {
            AnturaModelManager.I.ClearLoadedRewardInCategory(_categoryRewardId);
        }*/

        /// <summary>
        /// TODO: public or private?
        /// Gets the reward colors by identifier.
        /// </summary>
        /// <param name="_rewardItemId">The reward item identifier.</param>
        /// <returns></returns>
        /*static List<RewardColorItem> GetRewardColorsById(string _rewardItemId, RewardTypes _rewardType)
        {
            List<RewardColorItem> returnList = new List<RewardColorItem>();
            // TODO: logic
            return returnList;
        }*/

        /// <summary>
        /// Gets the antura rotation angle view for reward category.
        /// </summary>
        /// <param name="_categoryId">The category identifier.</param>
        /// <returns></returns>
        public float GetAnturaRotationAngleViewForRewardCategory(string _categoryId)
        {
            switch (_categoryId)
            {
                case "HEAD":
                    return 20;
                case "NOSE":
                    return -20;
                case "BACK":
                    return 200;
                case "NECK":
                    return 80;
                case "JAW":
                    return 30;
                case "TAIL":
                    return 160;
                case "EAR_R":
                    return -40;
                case "EAR_L":
                    return 40;
                default:
                    return 0;
            }
        }

        #endregion
        
    }

}