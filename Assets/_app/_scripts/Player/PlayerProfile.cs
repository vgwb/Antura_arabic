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
        public int ProfileCompletion = 0;

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

        public void DeleteThisProfile()
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
            SetMaxJourneyPosition(AppManager.I.JourneyHelper.FindNextJourneyPosition(CurrentJourneyPosition));

            if (!HasFinishedTheGame)
            {
                HasFinishedTheGame = AppManager.I.JourneyHelper.HasFinishedTheGame();
            }
            Debug.Log("Has finished? " + HasFinishedTheGame);

            if (HasFinishedTheGame && !HasFinishedTheGameWithAllStars)
            {
                HasFinishedTheGameWithAllStars = AppManager.I.ScoreHelper.HasFinishedTheGameWithAllStars();
            }
            Debug.Log("Has finished with all the stars? " + HasFinishedTheGameWithAllStars);
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
                jsonAnturaCustimizationData = _currentAnturaCustomizations.GetJsonListOfIds();
                //SaveCustomization();
            }
        }

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
                    return LoadRewardsUnlockedFromDB();
                return _rewardsUnlocked;
            }

            private set { _rewardsUnlocked = value; }
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

            //    RewardPackUnlockData unlockedRewardData = RewardsUnlocked.Find(r => r.Type == _rewardType && r.PlaySessionId == plsRew.PlaySession);
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
        /// Adds or update the reward unlocked and persist it.
        /// </summary>
        /// <param name="rewardPackUnlockData">The reward pack.</param>
        public void AddRewardUnlocked(RewardPackUnlockData rewardPackUnlockData)
        {
            AppManager.I.DB.UpdateRewardPackUnlockData(rewardPackUnlockData);
        }

        /// <summary>
        /// Adds or update a list of unlocked rewards and persist it.
        /// </summary>
        public void AddRewardUnlockedAll(List<RewardPackUnlockData> rewardPackUnlockDatas)
        {
            AppManager.I.DB.UpdateRewardPackUnlockDataAll(rewardPackUnlockDatas);
        }

        /// <summary>
        /// Saves the customization on db.
        /// </summary>
        /// <param name="_anturaCustomization">The antura customization. If null save only on db.</param>
        public void SaveCustomization(AnturaCustomization _anturaCustomization = null)
        {
            if (_anturaCustomization != null)
                CurrentAnturaCustomizations = _anturaCustomization;
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
            if (ProfileCompletion < 2)
                return true;
            else
                return false;
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
            if (_step < 0) return true;
            if (_step > 2) return false;
            if (ProfileCompletion == _step - 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Set firsts contact flag as passed for step specified.
        /// </summary>
        /// <param name="_step">The step (1 (default) or 2).</param>
        public void FirstContactPassed(int _step = 1)
        {
            switch (_step) {
                case 1:
                    ProfileCompletion = _step;
                    break;
                case 2:
                    ProfileCompletion = _step;
                    break;
            }

            Save();
        }

        /// <summary>
        /// Resets the player profile completion.
        /// </summary>
        public void ResetPlayerProfileCompletion()
        {
            ProfileCompletion = 0;
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
        public bool IsFirstTimeBook()
        {
            if (ProfileCompletion < 3)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Firsts the time book passed.
        /// </summary>
        public void FirstTimeBookPassed()
        {
            ProfileCompletion = 2;
            Save();
        }
        #endregion

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
            HasFinishedTheGame = _data.HasFinishedTheGame;
            HasFinishedTheGameWithAllStars = _data.HasFinishedTheGameWithAllStars;
            ProfileCompletion = _data.ProfileCompletion;
            TotalNumberOfBones = _data.TotalNumberOfBones;
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
            PlayerIconData returnData = new PlayerIconData() { Uuid = this.Uuid, AvatarId = this.AvatarId, Gender = this.Gender, Tint = this.Tint, IsDemoUser = this.IsDemoUser, HasFinishedTheGame = this.HasFinishedTheGame, HasFinishedTheGameWithAllStars = this.HasFinishedTheGameWithAllStars};
            return returnData;
        }
        #endregion

    }
}