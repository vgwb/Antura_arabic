using System;
using ModularFramework.Modules;
using UnityEngine;
using System.Collections.Generic;
using EA4S.Antura;
using EA4S.Core;
using EA4S.Database;
using EA4S.Rewards;

namespace EA4S.Profile
{
    public enum ProfileCompletionState
    {
        New = 0,
        FirstContact1 = 1,
        FirstContact2 = 5,
        BookVisited = 10,
        GameCompleted = 100,
        GameCompletedAndFinalShowed = 101,
    }

    /// <summary>
    /// A Player Profile contains persistent data on details and on the progression status of a single player.
    /// </summary>
    [Serializable]
    public class PlayerProfile : IPlayerProfile
    {
        public string Uuid;
        public int AvatarId;
        public PlayerGender Gender;
        public PlayerTint Tint;
        public int Age;
        public bool IsDemoUser;
        public bool HasFinishedTheGame;
        public bool HasFinishedTheGameWithAllStars;

        //int to track first visit
        //First contact (ProfileCompletion = 1 & 2)
        //BookVisited: ProfileCompletion = 3
        public ProfileCompletionState ProfileCompletion = ProfileCompletionState.New;

        public string MoodLastVisit;

        public JourneyPosition MaxJourneyPosition = new JourneyPosition(1, 1, 1);
        public JourneyPosition CurrentJourneyPosition = new JourneyPosition(1, 1, 1);

        #region Bones/coins
        public int TotalNumberOfBones = 8;
        public int GetTotalNumberOfBones()
        {
            return TotalNumberOfBones;
        }
        public int AddBones(int _bonesToAdd)
        {
            TotalNumberOfBones += _bonesToAdd;
            Save();
            return TotalNumberOfBones;
        }
        #endregion

        #region API

        #region management

        /// <summary>
        /// Automatically select first avatar profile.
        /// </summary>
        public PlayerProfile()
        {

        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            AppManager.I.PlayerProfileManager.SavePlayerSettings(this);
        }

        /// <summary>
        /// TBD if accessible form player instance.
        /// Saves the general game settings.
        /// </summary>
        public void SaveGameSettings()
        {
            AppManager.I.PlayerProfileManager.SaveGameSettings();
        }
        #endregion

        #region properties

        public string GetShortUuid()
        {
            string[] tokens = Uuid.Split('-');
            return tokens[0];
        }

        public PlayerIconData GetIcon()
        {
            return new PlayerIconData(Uuid, AvatarId, Gender, Tint, IsDemoUser, HasFinishedTheGame, HasFinishedTheGameWithAllStars);
        }

        public Sprite GetAvatar()
        {
            return Resources.Load<Sprite>(AppConstants.AvatarsResourcesDir + AvatarId);
        }

        /// <summary>
        /// True if player already answered to mood question for today.
        /// </summary>
        /// <value>
        ///   True if player already answered to mood question for today.
        /// </value>
        public bool MoodAlreadyAnswered {
            get {
                int secondAmount = AppManager.I.Teacher.logAI.SecondsFromLastMoodLog();
                if (secondAmount > 86400)
                    return false;
                else
                    return true;
            }
        }

        #endregion

        #region journey position        
        /// <summary>
        /// Sets the actual journey position and save to profile.
        /// @note: check valid data before insert.
        /// </summary>
        /// <param name="_stage">The stage.</param>
        /// <param name="_lb">The lb.</param>
        /// <param name="_ps">The ps.</param>
        /// <param name="_save">if set to <c>true</c> [save] profile at the end.</param>
        public void SetCurrentJourneyPosition(int _stage, int _lb, int _ps, bool _save = true)
        {
            SetCurrentJourneyPosition(new JourneyPosition(_stage, _lb, _ps));
            if (_save)
                Save();
        }

        /// <summary>
        /// Sets the actual journey position and save to profile.
        /// @note: check valid data before insert.
        /// </summary>
        /// <param name="_journeyPosition">The journey position.</param>
        /// <param name="_save">if set to <c>true</c> [save] profile at the end.</param>
        public void SetCurrentJourneyPosition(JourneyPosition _journeyPosition, bool _save = true)
        {
            CurrentJourneyPosition = _journeyPosition;
            if (_save)
                Save();

        }

        /// <summary>
        /// Advance the Max journey position based on the next after the Current one.
        /// </summary>
        public void AdvanceMaxJourneyPosition()
        {
            JourneyPosition p = AppManager.I.JourneyHelper.FindNextJourneyPosition(CurrentJourneyPosition);
            SetMaxJourneyPosition(p);
        }

        /// <summary>
        /// Sets the maximum journey position and save to profile.
        /// @note: check valid data before insert.
        /// </summary>
        /// <param name="newJourneyPosition">The journey position.</param>
        /// <param name="_save">if set to <c>true</c> [save] profile at the end.</param>
        public void SetMaxJourneyPosition(JourneyPosition newJourneyPosition, bool _save = true)
        {
            if (MaxJourneyPosition.IsMinor(newJourneyPosition)) {
                MaxJourneyPosition = new JourneyPosition(newJourneyPosition.Stage, newJourneyPosition.LearningBlock, newJourneyPosition.PlaySession);
                CurrentJourneyPosition = new JourneyPosition(newJourneyPosition.Stage, newJourneyPosition.LearningBlock, newJourneyPosition.PlaySession);

                if (_save) {
                    Save();
                }
            }
        }


        /// <summary>
        /// Check whether the game has finished and update the player icon.
        /// Called only when we actually finish the game.
        /// </summary>
        public void CheckGameFinished()
        {
            if (!HasFinishedTheGame)
            {
                HasFinishedTheGame = AppManager.I.JourneyHelper.HasFinishedTheGame();
                if (HasFinishedTheGame)
                {
                    AppManager.I.PlayerProfileManager.UpdateCurrentPlayerIconDataInSettings();
                    Save();
                }
            }
        }


        /// <summary>
        /// Check whether the game has finished with all starts and update the player icon.
        /// Called at each end of play session.
        /// </summary>
        public void CheckGameFinishedWithAllStars()
        {
            if (HasFinishedTheGame && !HasFinishedTheGameWithAllStars)
            {
                HasFinishedTheGameWithAllStars = AppManager.I.ScoreHelper.HasFinishedTheGameWithAllStars();
                if (HasFinishedTheGameWithAllStars)
                {
                    AppManager.I.PlayerProfileManager.UpdateCurrentPlayerIconDataInSettings();
                    Save();
                }
            }
        }

        /// <summary>
        /// Resets the maximum journey position to 1,1,1.
        /// </summary>
        public void ResetMaxJourneyPosition(bool _save = true)
        {
            MaxJourneyPosition = new JourneyPosition(1, 1, 1);
            CurrentJourneyPosition = new JourneyPosition(1, 1, 1);
            if (_save) {
                Save();
            }
        }
        #endregion

        #region Antura Customization

        private AnturaCustomization _currentAnturaCustomizations;
        /// <summary>
        /// The current antura customizations
        /// </summary>
        public AnturaCustomization CurrentAnturaCustomizations {
            get {
                if (_currentAnturaCustomizations == null) {
                    _currentAnturaCustomizations = new AnturaCustomization();
                    _currentAnturaCustomizations.LoadFromListOfIds(jsonAnturaCustimizationData);
                }
                return _currentAnturaCustomizations;
            }
            private set {
                _currentAnturaCustomizations = value;
                //jsonAnturaCustimizationData = _currentAnturaCustomizations.GetJsonListOfIds();
                SaveCustomization();
            }
        }

        #region Already unlocked rewards

        private List<RewardPackUnlockData> _rewardsUnlocked;
        /// <summary>
        /// Gets or sets the rewards unlocked.
        /// </summary>
        /// <value>
        /// The rewards unlocked.
        /// </value>
        public List<RewardPackUnlockData> RewardsUnlocked {
            get {
                if (_rewardsUnlocked == null)
                    _rewardsUnlocked = LoadRewardsUnlockedFromDB();
                return _rewardsUnlocked;
            }

            private set {
                _rewardsUnlocked = value;
            }
        }

 

        #endregion

        /// <summary>
        /// Resets the rewards unlocked data.
        /// </summary>
        public void ResetRewardsUnlockedData() {
            RewardsUnlocked = new List<RewardPackUnlockData>();
        }

        public string Key {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Loads the rewards unlocked from database.
        /// </summary>
        /// <returns></returns>
        public List<RewardPackUnlockData> LoadRewardsUnlockedFromDB()
        {
            return AppManager.I.DB.GetAllRewardPackUnlockData();
        }

        /// <summary>
        /// Gets the not yet unlocked rewards list.
        /// </summary>
        /// <param name="_rewardType">Type of the reward.</param>
        /// <returns></returns>
        public int GetNotYetUnlockedRewardCountForType(EA4S.Rewards.RewardTypes _rewardType)
        {
            int counter = 0;
            //foreach (PlaySessionRewardUnlock plsRew in RewardSystemManager.GetConfig().PlaySessionRewardsUnlock) {
            //    // Check if PlaySessionRewardUnlock contain requested type.
            //    switch (_rewardType) {
            //        case RewardTypes.reward:
            //            if (plsRew.Reward == "")
            //                continue;
            //            break;
            //        case RewardTypes.texture:
            //            if (plsRew.Texture == "")
            //                continue;
            //            break;
            //        case RewardTypes.decal:
            //            if (plsRew.Decal == "")
            //                continue;
            //            break;
            //        default:
            //            continue;
            //            break;
            //    }

            //    RewardPackUnlockData unlockedRewardData = RewardsUnlocked.Find(r => r.Type == _rewardType && r.JourneyPosition == plsRew.PlaySession);
            //    if (unlockedRewardData == null)
            //        counter++;
            //}
            switch (_rewardType) {
                case RewardTypes.reward:
                    counter = RewardSystemManager.GetConfig().Rewards.Count - RewardsUnlocked.FindAll(r => r.Type == _rewardType).Count;
                    break;
                case RewardTypes.texture:
                    counter = RewardSystemManager.GetConfig().RewardsTile.Count - RewardsUnlocked.FindAll(r => r.Type == _rewardType).Count;
                    break;
                case RewardTypes.decal:
                    counter = RewardSystemManager.GetConfig().RewardsDecal.Count - RewardsUnlocked.FindAll(r => r.Type == _rewardType).Count;
                    break;
            }

            return counter;
        }

        /// <summary>
        /// Return true if rewards for this type available.
        /// </summary>
        /// <param name="_rewardType">Type of the reward.</param>
        /// <returns></returns>
        public bool RewardForTypeAvailableYet(EA4S.Rewards.RewardTypes _rewardType)
        {
            return GetNotYetUnlockedRewardCountForType(_rewardType) <= 0 ? false : true;
        }

        /// <summary>
        /// Used to store antura custumization data in json and load it at runtime.
        /// </summary>
        string jsonAnturaCustimizationData = string.Empty;

        #region API

        /// <summary>
        /// True if there is at least one new reward for this player.
        /// </summary>
        /// <returns></returns>
        public bool ThereIsSomeNewReward() {
            if (RewardsUnlocked.Exists(r => r.IsNew == true)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool RewardColorIsNew(string _itemId, string _colorId) {
            if (RewardsUnlocked.Exists(r => r.ItemId == _itemId && r.ColorId == _colorId && r.IsNew == true)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Return true if Reward is never used by player.
        /// </summary>
        /// <returns></returns>
        public bool RewardItemIsNew(string _itemId) {
            if (RewardsUnlocked.Exists(r => r.ItemId == _itemId && r.IsNew == true)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Return true if Reward category container at least one reward never used by player.
        /// </summary>
        /// <returns></returns>
        public bool RewardCategoryContainsNewElements(RewardTypes _rewardType, string _rewardCategory = "") {
            if (RewardsUnlocked.Exists(r => r.Type == _rewardType && r.GetRewardCategory() == _rewardCategory && r.IsNew == true)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Mark RewardPackUnlockData as not new and update db entry.
        /// </summary>
        public void SetRewardPackUnlockedToNotNew(string _rewardPackId) {
            RewardPackUnlockData rewardPackToUpdate = RewardsUnlocked.Find(r => r.Id == _rewardPackId && r.IsNew == true);
            if (rewardPackToUpdate != null)
                rewardPackToUpdate.IsNew = false;
            AppManager.I.DB.UpdateRewardPackUnlockData(rewardPackToUpdate);
        }

        /// <summary>
        /// Delete all reward unlocks from the Dynamic DB.
        /// </summary>
        private void DeleteAllRewardUnlocks() {
            AppManager.I.DB.DeleteAll<RewardPackUnlockData>();
        }

        /// <summary>
        /// Adds or update the reward unlocked and persist it.
        /// </summary>
        /// <param name="rewardPackUnlockData">The reward pack.</param>
        public void AddRewardUnlocked(RewardPackUnlockData rewardPackUnlockData)
        {
            AppManager.I.Player.RewardsUnlocked.Add(rewardPackUnlockData);
            AppManager.I.DB.UpdateRewardPackUnlockData(rewardPackUnlockData);
        }

        /// <summary>
        /// Add update to db all 'this' reward unlocked.
        /// </summary>
        public void AddRewardUnlockedAll(RewardPackUnlockData _rewardPackUnlockData) {
            List<RewardPackUnlockData> rewards = new List<RewardPackUnlockData>();
            rewards.Add(_rewardPackUnlockData);
            AppManager.I.DB.UpdateRewardPackUnlockDataAll(rewards);
        }

        /// <summary>
        /// Adds or update a list of unlocked rewards and persist it.
        /// </summary>
        public void AddRewardUnlockedRange(List<RewardPackUnlockData> rewardPackUnlockDatas)
        {
            Debug.Log(this.RewardsUnlocked); 
            AppManager.I.Player.RewardsUnlocked.AddRange(rewardPackUnlockDatas);
            AppManager.I.DB.UpdateRewardPackUnlockDataAll(rewardPackUnlockDatas);
        }

        /// <summary>
        /// Saves the customization on db.
        /// </summary>
        /// <param name="_anturaCustomization">The antura customization. If null save only on db.</param>
        public void SaveCustomization(AnturaCustomization _anturaCustomization = null)
        {
            if (_anturaCustomization != null) { 
                CurrentAnturaCustomizations = _anturaCustomization;
            }
            jsonAnturaCustimizationData = CurrentAnturaCustomizations.GetJsonListOfIds();
            Save();

            AppManager.I.LogManager.LogInfo(InfoEvent.AnturaCustomization, CurrentAnturaCustomizations.GetJsonListOfIds());
        }

        #endregion

        #endregion

        #region Profile completion

        #region First Contact (ProfileCompletion = 1 & 2)
        /// <summary>
        /// Determines whether [is first contact].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is first contact]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFirstContact()
        {
            return (ProfileCompletion < ProfileCompletionState.FirstContact2);
        }

        /// <summary>
        /// Determines whether [is first contact] whit [the specified step] (1 or 2).
        /// </summary>
        /// <param name="_step">The step.</param>
        /// <returns>
        ///   <c>true</c> if [is first contact] [the specified step]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFirstContact(int _step)
        {
            if (_step < (int)ProfileCompletionState.FirstContact1) return true;
            if (_step >= (int)ProfileCompletionState.FirstContact2) return false;

            if ((int)ProfileCompletion == _step - 1) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Set firsts contact flag as passed for step specified.
        /// </summary>
        /// <param name="_step">The step (1 (default) or 2).</param>
        public void FirstContactPassed(int _step = 1)
        {
            switch (_step) {
                case 1:
                    ProfileCompletion = ProfileCompletionState.FirstContact1;
                    break;
                case 2:
                    ProfileCompletion = ProfileCompletionState.FirstContact2;
                    break;
            }
            Save();
        }

        /// <summary>
        /// Resets the player profile completion.
        /// </summary>
        public void ResetPlayerProfileCompletion()
        {
            ProfileCompletion = ProfileCompletionState.New;
            Save();
        }
        #endregion

        #region BookVisited (ProfileCompletion = 3)                
        /// <summary>
        /// Determines whether [is first time book].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is first time book]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBookVisited()
        {
            return (ProfileCompletion < ProfileCompletionState.BookVisited);
        }

        /// <summary>
        /// Firsts the time book passed.
        /// </summary>
        public void SetBookVisited()
        {
            ProfileCompletion = ProfileCompletionState.BookVisited;
            Save();
        }
        #endregion

        #region GameEnded
        public bool IsGameCompleted() {
            if (ProfileCompletion < ProfileCompletionState.GameCompleted)
                return false;
            return true;
        }

        public void SetGameCompleted() {
            ProfileCompletion = ProfileCompletionState.GameCompleted;
            AppManager.I.StartCoroutine(RewardSystemManager.UnlockExtraRewards());
            AppManager.I.PlayerProfileManager.UpdateCurrentPlayerIconDataInSettings();
            CheckGameFinished();
        }

        public bool HasFinalBeenShown() {
            if (ProfileCompletion < ProfileCompletionState.GameCompletedAndFinalShowed)
                return false;
            return true;
        }

        public void SetFinalShown() {
            ProfileCompletion = ProfileCompletionState.GameCompletedAndFinalShowed;
        }
        #endregion

        #endregion

        #region To/From PlayerProfileData        
        /// <summary>
        /// Converts this instance to PlayerProfileData.
        /// </summary>
        /// <returns></returns>
        public PlayerProfileData ToData()
        {
            PlayerProfileData newProfileData = new PlayerProfileData(new PlayerIconData(Uuid, AvatarId, Gender, Tint, IsDemoUser, HasFinishedTheGame, HasFinishedTheGameWithAllStars), Age, TotalNumberOfBones, ProfileCompletion);
            newProfileData.SetCurrentJourneyPosition(this.CurrentJourneyPosition);
            newProfileData.SetMaxJourneyPosition(this.MaxJourneyPosition);
            string jsonStringForAnturaCustomization = this.CurrentAnturaCustomizations.GetJsonListOfIds();
            newProfileData.CurrentAnturaCustomization = jsonStringForAnturaCustomization;
            return newProfileData;
        }

        /// <summary>
        /// Charge this with PlayerProfileData.
        /// </summary>
        public PlayerProfile FromData(PlayerProfileData _data)
        {
            Uuid = _data.Uuid;

            AvatarId = _data.AvatarId;
            Age = _data.Age;
            Gender = _data.Gender;
            Tint = _data.Tint;
            IsDemoUser = _data.IsDemoUser;
            HasFinishedTheGame = _data.JourneyCompleted;
            HasFinishedTheGameWithAllStars = _data.HasFinishedTheGameWithAllStars();
            ProfileCompletion = _data.ProfileCompletion;
            TotalNumberOfBones = _data.TotalBones;
            this.SetCurrentJourneyPosition(_data.GetCurrentJourneyPosition(), false);
            this.SetMaxJourneyPosition(_data.GetMaxJourneyPosition(), false);
            // Antura customization save only customization data
            jsonAnturaCustimizationData = _data.CurrentAnturaCustomization;

            return this;
        }
        #endregion

        #region player icon data
        public PlayerIconData GetPlayerIconData()
        {
            PlayerIconData returnData = new PlayerIconData() { Uuid = this.Uuid, AvatarId = this.AvatarId, Gender = this.Gender, Tint = this.Tint, IsDemoUser = this.IsDemoUser, HasFinishedTheGame = this.HasFinishedTheGame, HasFinishedTheGameWithAllStars = this.HasFinishedTheGameWithAllStars };
            return returnData;
        }
        #endregion

        #endregion

    }
}