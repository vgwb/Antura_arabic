using System;
using UnityEngine.UI;
using ModularFramework.Core;
using ModularFramework.Modules;
using UnityEngine;
using System.Collections.Generic;

namespace EA4S
{

    [Serializable]
    public class PlayerProfile : IPlayerProfile
    {
        public string Key { get; set; }
        public int Id;
        public int AvatarId;
        public int Age;
        public string Name;

        //int to track first visit
        //First contact (ProfileCompletion = 1 & 2)
        //BookVisited: ProfileCompletion = 3
        public int ProfileCompletion = 0;

        // Mood (1 to 5 indicators)
        public float MainMood = 3f;
        public float Impatient = 3f;
        public float Impulsive = 3f;
        public float Genius = 3f;
        public float Bored = 3f;
        public float Collector = 3f;
        public float Frustrated = 3f;

        // PlaySkills
        public float Precision;
        public float Reaction;
        public float Memory;
        public float Logic;
        public float Rhythm;
        public float Musicality;
        public float Sight;

        public string MoodLastVisit;

        public JourneyPosition MaxJourneyPosition = new JourneyPosition(1, 1, 1);
        public JourneyPosition CurrentJourneyPosition = new JourneyPosition(1, 1, 1);
        [NonSerialized]
        public int CurrentMiniGameInPlaySession;

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

        public void DeleteThisProfile() { }

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

        public Sprite GetAvatar()
        {
            return Resources.Load<Sprite>(AppConstants.AvatarsResourcesDir + AvatarId);
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
            AppManager.I.Player.CurrentJourneyPosition = _journeyPosition;
            if (_save)
                Save();

        }

        /// <summary>
        /// Sets the maximum journey position and save to profile.
        /// @note: check valid data before insert.
        /// </summary>
        /// <param name="newJourneyPosition">The journey position.</param>
        /// <param name="_save">if set to <c>true</c> [save] profile at the end.</param>
        public void SetMaxJourneyPosition(JourneyPosition newJourneyPosition, bool _save = true)
        {
            if (AppManager.I.Player.MaxJourneyPosition.isMinor(newJourneyPosition)) {
                AppManager.I.Player.ResetPlaySessionMinigame();
                AppManager.I.Player.MaxJourneyPosition = new JourneyPosition(newJourneyPosition.Stage, newJourneyPosition.LearningBlock, newJourneyPosition.PlaySession);
                AppManager.I.Player.CurrentJourneyPosition = new JourneyPosition(newJourneyPosition.Stage, newJourneyPosition.LearningBlock, newJourneyPosition.PlaySession);
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
            AppManager.I.Player.MaxJourneyPosition = new JourneyPosition(1, 1, 1);
            AppManager.I.Player.CurrentJourneyPosition = new JourneyPosition(1, 1, 1);
            if (_save) {
                Save();
            }
        }
        #endregion

        #region Antura Customization                
        /// <summary>
        /// The rewards unlocked
        /// </summary>
        public List<RewardPack> RewardsUnlocked = new List<RewardPack>();

        /// <summary>
        /// Adds the reward unlocked.
        /// </summary>
        /// <param name="_rewardPack">The reward pack.</param>
        public void AddRewardUnlocked(RewardPack _rewardPack)
        {
            RewardsUnlocked.Add(_rewardPack);
            Save();
        }

        /// <summary>
        /// The current antura customizations
        /// </summary>
        public AnturaCustomization CurrentAnturaCustomizations = new AnturaCustomization();

        /// <summary>
        /// Saves the customization.
        /// </summary>
        /// <param name="_anturaCustomization">The antura customization.</param>
        public void SaveCustomization(AnturaCustomization _anturaCustomization)
        {
            CurrentAnturaCustomizations = _anturaCustomization;
            Save();
        }

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

        #region Current minigame in playsession        
        /// <summary>
        /// Nexts the play session minigame.
        /// </summary>
        public void NextPlaySessionMinigame()
        {
            CurrentMiniGameInPlaySession++;
        }
        /// <summary>
        /// Resets position in play session minigame.
        /// </summary>
        public void ResetPlaySessionMinigame()
        {
            CurrentMiniGameInPlaySession = 0;
        }
        #endregion

        #endregion


    }
}