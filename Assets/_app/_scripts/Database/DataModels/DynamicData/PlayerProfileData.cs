using Antura.Core;
using Antura.Helpers;
using Antura.Profile;
using SQLite;

namespace Antura.Database
{
    /// <summary>
    /// Serialized information about the player. Used by the Player Profile.
    /// </summary>
    [System.Serializable]
    public class PlayerProfileData : IData, IDataEditable
    {
        public const string UNIQUE_ID = "1";

        /// <summary>
        /// Primary key for the database.
        /// Unique, as there is only one row for this table.
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// Timestamp of creation of the profile data.
        /// </summary>
        public int Timestamp { get; set; }


        #region PlayerIconData

        /// <summary>
        /// Unique identifier for the player.
        /// Also used as the name of the database file.
        /// Part of PlayerIconData.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// ID of the avatar icon for this player.
        /// Part of PlayerIconData.
        /// </summary>
        public int AvatarId { get; set; }

        /// <summary>
        /// Gender of the player.
        /// Part of PlayerIconData.
        /// </summary>
        public PlayerGender Gender { get; set; }

        /// <summary>
        /// Tint of the player icon.
        /// Part of PlayerIconData.
        /// </summary>
        public PlayerTint Tint { get; set; }

        /// <summary>
        /// Is this player a demo user?
        /// Demo users have all the game unlocked.
        /// Part of PlayerIconData.
        /// </summary>
        public bool IsDemoUser { get; set; }

        /// <summary>
        /// Has the player completed the whole journey
        /// Used only for the player icons in the Home scene.
        /// Part of PlayerIconData.
        /// </summary>
        public bool JourneyCompleted { get; set; }

        /// <summary>
        /// general total final overall score
        /// Used only for the player icons in the Home scene.
        /// Part of PlayerIconData.
        /// </summary>
        public float TotalScore { get; set; }

        #endregion

        #region Details

        /// <summary>
        /// Age of the player, as selected during profile creation.
        /// </summary>
        public int Age { get; set; }

        #endregion

        #region Progression

        /// <summary>
        /// State of completion for the player profile.
        /// Can be 0,1,2,3. See PlayerProfile for further details.
        /// </summary>
        public ProfileCompletionState ProfileCompletion { get; set; }

        /// <summary>
        /// Maximum journey position: stage reached.
        /// </summary>
        public int MaxStage { get; set; }

        /// <summary>
        /// Maximum journey position: learning block reached.
        /// </summary>
        public int MaxLearningBlock { get; set; }

        /// <summary>
        /// Maximum journey position: play session reached.
        /// </summary>
        public int MaxPlaySession { get; set; }


        /// <summary>
        /// Current journey position: play session reached.
        /// </summary>
        public int CurrentStage { get; set; }

        /// <summary>
        /// Current journey position: learning block reached.
        /// </summary>
        public int CurrentLearningBlock { get; set; }

        /// <summary>
        /// Current journey position: play session reached.
        /// </summary>
        public int CurrentPlaySession { get; set; }

        #endregion

        #region Rewards

        /// <summary>
        /// Total bones collected.
        /// </summary>
        public int TotalBones { get; set; }

        /// <summary>
        /// JSON data for the current customization set on Antura.
        /// </summary>
        public string CurrentAnturaCustomization { get; set; }

        /// <summary>
        /// Number of consecutive days of playing
        /// </summary>
        // TODO: we need to handle this too, but this requires a regeneration (or a migration) of existing databases
        public int ComboPlayDays;// { get; set; }

        /// <summary>
        /// JSON data for the current shop unlocked state.
        /// </summary>
        // TODO: we need to handle this too, but this requires a regeneration (or a migration) of existing databases
        public string CurrentShopStateJSON; // {get; set;}

        #endregion

        #region Additional Data

        /// <summary>
        /// JSON-serialized additional data, may be added as needed.
        /// </summary>
        public string AdditionalData { get; set; }

        #endregion


        public PlayerProfileData()
        {
        }

        public PlayerProfileData(PlayerIconData iconData, int age, int totalBones, ProfileCompletionState profileCompletion, string currentAnturaCustomization = null)
        {
            Id = UNIQUE_ID;  // Only one record
            Age = age;
            SetPlayerIconData(iconData);
            ProfileCompletion = profileCompletion;
            TotalBones = totalBones;
            SetMaxJourneyPosition(JourneyPosition.InitialJourneyPosition);
            SetCurrentJourneyPosition(JourneyPosition.InitialJourneyPosition);
            Timestamp = GenericHelper.GetTimestampForNow();
            CurrentAnturaCustomization = currentAnturaCustomization;
            ComboPlayDays = 0;
            CurrentShopStateJSON = null;
        }

        public bool HasFinishedTheGameWithAllStars()
        {
            return (TotalScore >= 0.999f);
        }

        public void SetPlayerIconData(PlayerIconData iconData)
        {
            Uuid = iconData.Uuid;
            AvatarId = iconData.AvatarId;
            Gender = iconData.Gender;
            Tint = iconData.Tint;
            IsDemoUser = iconData.IsDemoUser;
            JourneyCompleted = iconData.HasFinishedTheGame;
            TotalScore = (iconData.HasFinishedTheGameWithAllStars ? 1f : 0f);
        }

        public PlayerIconData GetPlayerIconData()
        {
            return new PlayerIconData(Uuid, AvatarId, Gender, Tint, IsDemoUser, JourneyCompleted, HasFinishedTheGameWithAllStars());
        }

        #region Journey Position

        public void SetMaxJourneyPosition(JourneyPosition pos)
        {
            MaxStage = pos.Stage;
            MaxLearningBlock = pos.LearningBlock;
            MaxPlaySession = pos.PlaySession;
        }

        public void SetCurrentJourneyPosition(JourneyPosition pos)
        {
            CurrentStage = pos.Stage;
            CurrentLearningBlock = pos.LearningBlock;
            CurrentPlaySession = pos.PlaySession;
        }

        public JourneyPosition GetMaxJourneyPosition()
        {
            return new JourneyPosition(MaxStage, MaxLearningBlock, MaxPlaySession);
        }

        public JourneyPosition GetCurrentJourneyPosition()
        {
            return new JourneyPosition(CurrentStage, CurrentLearningBlock, CurrentPlaySession);
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
            return string.Format("ID{0},U{1},Ts{2}, MaxJ({3}.{4}.{5}), CurrentJ({6}.{7}.{8}), ProfCompl{9}, JourneyCompleted{10}, Score{11}",
                Id,
                Uuid,
                Timestamp,

                MaxStage,
                MaxLearningBlock,
                MaxPlaySession,

                CurrentStage,
                CurrentLearningBlock,
                CurrentPlaySession,

                ProfileCompletion,
                JourneyCompleted,
                TotalScore
            );
        }

        #endregion

    }
}