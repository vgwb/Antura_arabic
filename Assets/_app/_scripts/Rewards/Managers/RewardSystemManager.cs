using System;
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

        public delegate void RewardSystemEventHandler(RewardPack rewardPack);

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

            TextAsset unlocksConfigData = Resources.Load(ANTURA_REWARDS_UNLOCKS_CONFIG_PATH) as TextAsset;
            unlocksConfig = JsonUtility.FromJson<RewardsUnlocksConfig>(unlocksConfigData.text);

            BuildAllPacks();
        }

        #region Reward Packs

        private Dictionary<RewardBaseType, List< RewardPack>> rewardPacksDict = new Dictionary<RewardBaseType, List<RewardPack>>();

        public RewardPack GetRewardPackByUniqueId(string uniqueId)
        {
            return GetAllRewardPacks().FirstOrDefault(p => p.UniqueId == uniqueId);
        }

        public RewardPack GetRewardPackByPartsIds(string baseId, string colorId)
        {
            return GetAllRewardPacks().FirstOrDefault(p => p.BaseId == baseId && p.ColorId == colorId);
        }

        public List<RewardPack> GetRewardPacksOfType(RewardBaseType baseType)
        {
            return rewardPacksDict[baseType];
        }

        public IEnumerable<RewardPack> GetAllRewardPacks()
        {
            foreach (var rewardPack in rewardPacksDict[RewardBaseType.Prop])
                yield return rewardPack;
            foreach (var rewardPack in rewardPacksDict[RewardBaseType.Decal])
                yield return rewardPack;
            foreach (var rewardPack in rewardPacksDict[RewardBaseType.Texture])
                yield return rewardPack;
        }

        void BuildAllPacks()
        {
            rewardPacksDict.Clear();
            rewardPacksDict[RewardBaseType.Prop] = BuildPacks(RewardBaseType.Prop);
            rewardPacksDict[RewardBaseType.Decal] = BuildPacks(RewardBaseType.Decal);
            rewardPacksDict[RewardBaseType.Texture] = BuildPacks(RewardBaseType.Texture);
        }

        private List<RewardPack> BuildPacks(RewardBaseType baseType)
        {
            var bases = ItemsConfig.GetBasesForType(baseType);
            var colors = ItemsConfig.GetColorsForType(baseType);
            List<RewardPack> rewardPacks = new List<RewardPack>();
            foreach (var b in bases)
            {
                foreach (var c in colors)
                {
                    RewardPack pack = new RewardPack() { BaseId = b.ID, ColorId = c.ID, baseType = baseType};
                    // TODO: also augment with UnlockData
                    rewardPacks.Add(pack);
                }
            }
            return rewardPacks;
        }


        #endregion

        /// <summary>
        /// Gets the total count of all rewards. 
        /// Any type with any color variation available in game.
        /// </summary>
        public int GetTotalRewardsCount()
        {
            return GetAllRewardPacks().Count();
        }

        /// <summary>
        /// Gets a PROP by its string identifier.
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
        public MaterialPair GetMaterialPairForPack(RewardPack pack)
        {
            RewardProp prop = ItemsConfig.PropBases.Find(c => c.ID == pack.BaseId);
            RewardColor color = ItemsConfig.PropColors.Find(c => c.ID == pack.ColorId);
            if (color == null || prop == null)
            {
                return new MaterialPair();
            }
            MaterialPair mp = new MaterialPair(color.Color1Name, prop.Material1, color.Color2Name, prop.Material2);
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
            return AppManager.I.Player != null ? AppManager.I.Player.UnlockedRewardsData.Count : 0;
        }

        /// <summary>
        /// Gets the unlocked reward for specified playsession.
        /// </summary>
        /// <param name="journeyPosition">The playsession identifier (format 1.4.2).</param>
        /// <returns></returns>
        public int GetNumberOfUnlockedRewardsAtJP(JourneyPosition journeyPosition)
        {
            int rCount = AppManager.I.Player.UnlockedRewardsData.FindAll(ur => ur.GetJourneyPosition().Equals(journeyPosition)).Count;
            return rCount > 2 ? 2 : rCount; // max 2 becasue the results screen does not allow for more
        }

        void UnlockPacks(List<RewardPack> packs, JourneyPosition jp)
        {
            foreach (var pack in packs)
            {
                UnlockPack(pack, jp);
            }
        }

        void UnlockPack(RewardPack pack, JourneyPosition jp)
        {
            pack.unlockData.SetJourneyPosition(jp);
            pack.unlockData.IsLocked = false;
            // TODO: save it too?
        }

        // this unlocks ALL rewards that have not been unlocked yet
        public IEnumerator UnlockAllMissingRewards()
        {
            JourneyPosition extraRewardJP = new JourneyPosition(100, 100, 100);

            UnlockPacks(GetLockedRewardPacks(RewardBaseType.Prop), extraRewardJP);
            UnlockPacks(GetLockedRewardPacks(RewardBaseType.Decal), extraRewardJP);
            UnlockPacks(GetLockedRewardPacks(RewardBaseType.Texture), extraRewardJP);
            // TODO: save in the profile too
            yield return null;
        }

        /// <summary>
        /// Unlocks all rewards in the game.
        /// </summary>
        public void UnlockAllRewards()
        {
            var allPlaySessionInfos = AppManager.I.ScoreHelper.GetAllPlaySessionInfo();
            for (int i = 0; i < allPlaySessionInfos.Count; i++)
            {
                var jp = AppManager.I.JourneyHelper.PlaySessionIdToJourneyPosition(allPlaySessionInfos[i].data.Id);
                var packs = UnlockRewardPacksForJourneyPosition(jp);
                Debug.LogFormat("Unlocked rewards for playsession {0} : {1}", jp, packs.Count);
            }

            Debug.LogFormat("Unlocking also all extra rewards!");
            AppManager.I.StartCoroutine(UnlockAllMissingRewards());
        }

        public bool IsRewardPackAlreadyUnlocked(RewardPack pack)
        {
            return !pack.unlockData.IsLocked;
        }

        /// <summary>
        /// Return true if all rewards for this JourneyPosition are already unlocked.
        /// </summary>
        /// <param name="_journeyPosition">The journey position.</param>
        public bool JourneyPositionRewardsAlreadyUnlocked(JourneyPosition journeyPosition)
        {
            return GetAllRewardPacks().Any(p => p.unlockData.GetJourneyPosition().Equals(journeyPosition));
        }

        /// <summary>
        /// Rewards the already unlocked.
        /// </summary>
        /// <param name="_itemId">The item identifier.</param>
        /// <param name="_colorId">The color identifier.</param>
        /// <param name="baseType">The type.</param>
        /// <returns></returns>
        /*private bool IsRewardAlreadyUnlocked(string _itemId, string _colorId, RewardBaseType baseType)
        {
            return AppManager.I.Player.UnlockedRewardsData.Find(r => r.ItemId == _itemId && r.ColorId == _colorId && r.BaseType == baseType) != null;
        }*/


        public List<RewardPack> UnlockRewardPacksForJourneyPosition(JourneyPosition journeyPosition)
        {
            var packs = GenerateRewardPacksForJourneyPosition(journeyPosition);
            UnlockPacks(packs, journeyPosition);
            return packs;
        }

        public IEnumerable<RewardPack> GetRewardPacksUnlockedInJourneyPosition(JourneyPosition journeyPosition)
        {
            return GetAllRewardPacks().Where(x => x.unlockData.GetJourneyPosition().Equals(journeyPosition));
        }

        /// <summary>
        /// Return list of right rewards for actual playsession if not already.
        /// </summary>
        /// <returns></returns>
        public List<RewardPack> GenerateRewardPacksForJourneyPosition(JourneyPosition journeyPosition)
        {
            var newlyUnlockedPacks = new List<RewardPack>();

            // TODO: HANDLE THIS DIFFERENTLY
            // not _forceToReturnReward check if reward is already unlocked for this playsession and if true return empty list
            //if (JourneyPositionRewardsAlreadyUnlocked(journeyPosition) && !_forceToReturnReward)
            //     return newlyUnlockedPacks;

            // What kind of reward it is?
            RewardUnlocksAtJourneyPosition unlocksAtJP = ItemsConfig.PlaySessionRewardsUnlock.Find(r => r.JourneyPositionID == journeyPosition.Id);
            if (unlocksAtJP == null)
            {
                Debug.LogErrorFormat("Unable to find reward type for this playsession {0}", journeyPosition);
            }

            // Check numbers and base types
            for (int i = 0; i < unlocksAtJP.NewPropBase; i++)
                newlyUnlockedPacks.Add(GetNewRewardPack(RewardBaseType.Prop, UnlockType.NewBase));
            //if (OnNewRewardUnlocked != null)  OnNewRewardUnlocked(newItemReward);

            for (int i = 0; i < unlocksAtJP.NewPropColor; i++)
                newlyUnlockedPacks.Add(GetNewRewardPack(RewardBaseType.Prop, UnlockType.NewColor));

            if (unlocksAtJP.NewTexture > 0)
                newlyUnlockedPacks.Add(GetNewRewardPack(RewardBaseType.Texture, UnlockType.Any));

            if (unlocksAtJP.NewDecal > 0)
                newlyUnlockedPacks.Add(GetNewRewardPack(RewardBaseType.Decal, UnlockType.Any));

            //UnlockPacks(newlyUnlockedPacks, journeyPosition);

            ////////////////////////////////////////////////////
            return newlyUnlockedPacks;
        }

        List<RewardPack> GetUnlockedRewardPacks(RewardBaseType baseType)
        {
            var packsOfBase = rewardPacksDict[baseType];
            var unlockedPacks = packsOfBase.Where(IsRewardPackAlreadyUnlocked);
            return unlockedPacks.ToList();
        }
        List<RewardPack> GetLockedRewardPacks(RewardBaseType baseType)
        {
            var packsOfBase = rewardPacksDict[baseType];
            var lockedPacks = packsOfBase.Where(x => !IsRewardPackAlreadyUnlocked(x));
            return lockedPacks.ToList();
        }

        List<RewardBase> GetLockedRewardBases(RewardBaseType baseType)
        {
            var unlockedBases = GetUnlockedRewardBases(baseType);
            var allBases = itemsConfig.GetBasesForType(baseType);
            List<RewardBase> lockedBases = new List<RewardBase>();

            foreach (var rewardBase in allBases)
            {
                if (!unlockedBases.Contains(rewardBase))
                    lockedBases.Add(rewardBase);
            }
            return lockedBases;
        }

        List<RewardBase> GetUnlockedRewardBases(RewardBaseType baseType)
        {
            var packsOfBase = rewardPacksDict[baseType];
            var unlockedPacksOfBase = packsOfBase.Where(IsRewardPackAlreadyUnlocked);
            var allBases = itemsConfig.GetBasesForType(baseType);

            HashSet<RewardBase> unlockedBases = new HashSet<RewardBase>();
            foreach (var rewardPack in unlockedPacksOfBase)
            {
                var rewardBase = allBases.First(x => x.ID == rewardPack.BaseId);
                if (rewardBase != null)
                {
                    unlockedBases.Add(rewardBase);
                }
            }
            return unlockedBases.ToList();
        }

        public enum UnlockType
        {
            Any,
            NewBase,
            NewColor
        }

        // OK
        /// <summary>
        /// Gets the next reward pack. Contains all logic to create new reward.
        /// </summary>
        /// <param name="baseType">Type of the reward.</param>
        /// <returns></returns>
        private RewardPack GetNewRewardPack(RewardBaseType baseType, UnlockType unlockType)
        {
            RewardPack newUnlockedPack = null;

            switch (unlockType)
            {
                case UnlockType.NewBase:
                {
                    // We force a NEW base
                    var lockedBases = GetLockedRewardBases(baseType);
                    var newBase = lockedBases.RandomSelectOne();
                    var lockedPacks = GetLockedRewardPacks(baseType);
                    var lockedPacksOfNewBase = lockedPacks.Where(x => x.BaseId == newBase.ID).ToList();
                    newUnlockedPack = lockedPacksOfNewBase.RandomSelectOne();
                }
                    break;
                case UnlockType.NewColor:
                {
                    // We force an OLD base
                    var unlockedBases = GetLockedRewardBases(baseType);
                    var oldBase = unlockedBases.RandomSelectOne();
                    // TODO: select only those that have colors to be unlocked!
                    var lockedPacks = GetLockedRewardPacks(baseType);
                    var lockedPacksOfOldBase = lockedPacks.Where(x => x.BaseId == oldBase.ID).ToList();
                    if (lockedPacksOfOldBase.Count == 0)
                        throw new NullReferenceException(
                            "We do not have enough rewards to get a new color for an old base of type " + baseType);
                    newUnlockedPack = lockedPacksOfOldBase.RandomSelectOne();
                }
                    break;
                case UnlockType.Any:
                {
                    // We get any reward pack
                    var lockedPacks = GetLockedRewardPacks(baseType);
                    if (lockedPacks.Count == 0)
                        throw new NullReferenceException(
                            "We do not have enough rewards left of type " + baseType);
                    newUnlockedPack = lockedPacks.RandomSelectOne();
                }
                    break;

            }

            //var rpud = new RewardPackUnlockData(AppManager.I.LogManager.AppSession, newUnlockedPack.UniqueId, journeyPosition);
            return newUnlockedPack;
        }


        // OK
        /// <summary>
        /// Unlocks the first set of rewards for current player.
        /// </summary>
        public void UnlockFirstSetOfRewards()
        {
            var _player = AppManager.I.Player;
            if (_player == null)
            {
                Debug.LogError("No current player available!");
                return;
            }

            // TODO: add the JP and isNew=false too
            var zeroJP = new JourneyPosition(0, 0, 0);

            var propPack = GetFirstAnturaReward(RewardBaseType.Prop);
            UnlockPack(propPack, zeroJP);
            _player.ResetUnlockedRewardsData();
            //_player.AddRewardUnlocked(defaultProp);    // 1 prop

            // decal
            var decalPack = GetFirstAnturaReward(RewardBaseType.Decal);    // 1 decal
            UnlockPack(decalPack, zeroJP);
            // _player.AddRewardUnlocked(defaultDecal);

            // force to to wear decal
            _player.CurrentAnturaCustomizations.DecalPack = decalPack;
            _player.CurrentAnturaCustomizations.DecalPackId = decalPack.UniqueId;

            // texture
            var texturePack = GetFirstAnturaReward(RewardBaseType.Texture);    // 1 texture
            UnlockPack(texturePack, zeroJP);
            //_player.AddRewardUnlocked(texturePack);
            // force to to wear texture
            _player.CurrentAnturaCustomizations.TexturePack = texturePack;
            _player.CurrentAnturaCustomizations.TexturePackId = texturePack.UniqueId;

            // TODO:
           // _player.SaveUnlockData();

            // Add all 3 rewards
            //_player.AddRewardUnlockedAll();
            // Save actual customization
            _player.SaveAnturaCustomization();
        }

        // OK
        /// <summary>
        /// Gets the first antura reward.
        /// </summary>
        /// <param name="baseType">Type of the reward.</param>
        private RewardPack GetFirstAnturaReward(RewardBaseType baseType)
        {
            // this returns the DEFAULT rewards for one of the three types
            RewardPack rp = null;
            switch (baseType)
            {
                case RewardBaseType.Prop:
                    rp = GetNewRewardPack(baseType, UnlockType.NewBase);
                    break;
                case RewardBaseType.Texture:
                    rp = GetRewardPackByPartsIds("Antura_wool_tilemat","color1");
                    //rp.IsNew = false; // Because is automatically selected
                    break;
                case RewardBaseType.Decal:
                    rp = GetRewardPackByPartsIds("Antura_decalmap01", "color1");
                    //rp.IsNew = false; // Because is automatically selected
                    break;
            }
            return rp;
        }

        #endregion

        #region Customization (i.e. UI, selection, view)

        // used during customization
        private RewardPackUnlockData CurrentSelectedReward = new RewardPackUnlockData();

        public string GetRewardCategory(RewardPack rewardPack)
        {
            /*if (BaseType != RewardBaseType.Prop) {
                return string.Empty;
            }*/
            RewardProp reward = ItemsConfig.PropBases.Find(r => r.ID == rewardPack.BaseId);
            if (reward != null)
            {
                return reward.Category;
            }
            return string.Empty;
        }

        // TODO:
        /// <summary>
        /// Gets the reward items by rewardType (always 9 items, if not presente item in the return list is null).
        /// </summary>
        /// <param name="baseType">Type of the reward.</param>
        /// <param name="_parentsTransForModels">The parents trans for models.</param>
        /// <param name="_categoryRewardId">The category reward identifier.</param>
        /// <returns></returns>
        public List<RewardItem> GetRewardItemsByRewardType(RewardBaseType baseType, List<Transform> _parentsTransForModels,
            string _categoryRewardId = "")
        {
            List<RewardItem> returnList = new List<RewardItem>();
            /// TODO: logic
            /// - Load returnList by type and category checking unlocked and if exist active one
            switch (baseType) {
                case RewardBaseType.Prop:
                    // Filter from unlocked elements (only items with this category and only one for itemID)
                    List<RewardProp> rewards = ItemsConfig.GetClone().PropBases;
                    foreach (var item in rewards.FindAll(r => r.Category == _categoryRewardId))
                    {
                        //var rewardPack = GetRewardPackByUniqueId(r.ItemId);
                        var unlockedPacks = GetUnlockedRewardPacks(baseType);

                        if (unlockedPacks.FindAll(pack => GetRewardCategory(pack) == _categoryRewardId)
                            .Exists(pack => pack.UniqueId == item.ID)) {
                            returnList.Add(new RewardItem() {
                                ID = item.ID,
                                IsNew = AppManager.I.Player.RewardItemIsNew(item.ID),
                                IsSelected = AppManager.I.Player.CurrentAnturaCustomizations.PropPacks.Exists(f => f.ItemId == item.ID)
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
                case RewardBaseType.Texture:
                    // Filter from unlocked elements (only one for itemID)
                    foreach (var item in ItemsConfig.TextureBases) {
                        if (AppManager.I.Player.UnlockedRewardsData.FindAll(ur => ur.BaseType == RewardBaseType.Texture)
                            .Exists(ur => ur.ItemId == item.ID)) {
                            returnList.Add(new RewardItem() {
                                ID = item.ID,
                                IsNew = AppManager.I.Player.RewardItemIsNew(item.ID),
                                IsSelected = AppManager.I.Player.CurrentAnturaCustomizations.TexturePack.ItemId == item.ID
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
                case RewardBaseType.Decal:
                    // Filter from unlocked elements (only one for itemID)
                    foreach (var item in ItemsConfig.DecalBases) {
                        if (AppManager.I.Player.UnlockedRewardsData.FindAll(ur => ur.BaseType == RewardBaseType.Decal)
                            .Exists(ur => ur.ItemId == item.ID)) {
                            returnList.Add(new RewardItem() {
                                ID = item.ID,
                                IsNew = AppManager.I.Player.RewardItemIsNew(item.ID),
                                IsSelected = AppManager.I.Player.CurrentAnturaCustomizations.DecalPack.ItemId == item.ID
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
                    Debug.LogWarningFormat("Reward typology requested {0} not found", baseType);
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
        public List<RewardColorItem> SelectRewardItem(string _rewardItemId, RewardBaseType rewardBaseType)
        {
            List<RewardColorItem> returnList = new List<RewardColorItem>();
            /// logic
            /// - Trigger selected reward event.
            /// - Load returnList of color for reward checking unlocked and if exist active one


            switch (rewardBaseType) {
                case RewardBaseType.Prop:
                    foreach (RewardColor color in ItemsConfig.GetClone().PropColors) {
                        if (AppManager.I.Player.UnlockedRewardsData.Exists(ur => ur.ItemId == _rewardItemId && ur.ColorId == color.ID)) {
                            RewardColorItem rci = new RewardColorItem(color);
                            rci.IsNew = AppManager.I.Player.UnlockedRewardsData.Exists(ur =>
                                ur.ItemId == _rewardItemId && ur.ColorId == color.ID && ur.IsNew == true);
                            returnList.Add(rci);
                        } else {
                            returnList.Add(null);
                        }
                    }
                    // set current reward in modification
                    CurrentSelectedReward = new RewardPackUnlockData() { ItemId = _rewardItemId, BaseType = RewardBaseType.Prop };
                    break;
                case RewardBaseType.Texture:
                    foreach (RewardColor color in ItemsConfig.TextureColors) {
                        if (AppManager.I.Player.UnlockedRewardsData.Exists(ur => ur.ItemId == _rewardItemId && ur.ColorId == color.ID)) {
                            RewardColorItem rci = new RewardColorItem(color);
                            rci.IsNew = AppManager.I.Player.UnlockedRewardsData.Exists(ur =>
                                ur.ItemId == _rewardItemId && ur.ColorId == color.ID && ur.IsNew == true);
                            rci.Color2RGB = rci.Color1RGB; // to avoid exadecimal conversion error on ui rgb code conversion.
                            returnList.Add(rci);
                        } else {
                            returnList.Add(null);
                        }
                    }
                    // set current reward in modification
                    CurrentSelectedReward = new RewardPackUnlockData() { ItemId = _rewardItemId, BaseType = RewardBaseType.Texture };
                    break;
                case RewardBaseType.Decal:
                    foreach (RewardColor color in ItemsConfig.DecalColors) {
                        if (AppManager.I.Player.UnlockedRewardsData.Exists(ur => ur.ItemId == _rewardItemId && ur.ColorId == color.ID)) {
                            RewardColorItem rci = new RewardColorItem(color);
                            rci.IsNew = AppManager.I.Player.UnlockedRewardsData.Exists(ur =>
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
                    CurrentSelectedReward = new RewardPackUnlockData() { ItemId = _rewardItemId, BaseType = RewardBaseType.Decal };
                    break;
                default:
                    Debug.LogWarningFormat("Reward typology requested {0} not found", rewardBaseType);
                    break;
            }

            // Color selection
            RewardPackUnlockData alreadySelectedReward = null;
            switch (rewardBaseType) {
                case RewardBaseType.Prop:
                    List<RewardPackUnlockData> fornitures = AppManager.I.Player.CurrentAnturaCustomizations.PropPacks;
                    alreadySelectedReward = fornitures.Find(r => r.ItemId == _rewardItemId && r.BaseType == rewardBaseType);
                    break;
                case RewardBaseType.Texture:
                    if (AppManager.I.Player.CurrentAnturaCustomizations.TexturePack.ItemId == _rewardItemId)
                        alreadySelectedReward = AppManager.I.Player.CurrentAnturaCustomizations.TexturePack;
                    break;
                case RewardBaseType.Decal:
                    if (AppManager.I.Player.CurrentAnturaCustomizations.DecalPack.ItemId == _rewardItemId)
                        alreadySelectedReward = AppManager.I.Player.CurrentAnturaCustomizations.DecalPack;
                    break;
                default:
                    Debug.LogErrorFormat("Reward type {0} not found!", rewardBaseType);
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
        /// <param name="rewardBaseType">Type of the reward.</param>
        public void SelectRewardColorItem(string _rewardColorItemId, RewardBaseType rewardBaseType)
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