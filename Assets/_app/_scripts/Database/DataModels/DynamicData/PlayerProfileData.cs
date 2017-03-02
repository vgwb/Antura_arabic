using EA4S.Core;
using EA4S.Helpers;
using EA4S.Profile;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Serialized information about the player. Used by the Player Profile.
    /// </summary>
    [System.Serializable]
    public class PlayerProfileData : IData
    {
        public const string UNIQUE_ID = "1";

        [PrimaryKey]
        public string Id { get; set; }

        // Player Icon Data
        public string Uuid { get; set; }
        public int AvatarId { get; set; }
        public PlayerGender Gender { get; set; }
        public PlayerTint Tint { get; set; }
        public bool IsDemoUser { get; set; }

        /// <summary>
        /// Has the player completed the whole game?
        /// Used only for the player icons in the Home scene.
        /// </summary>
        public bool HasFinishedTheGame { get; set; }

        /// <summary>
        /// Has the player completed the whole game AND earned full score in all play sessions?
        /// Used only for the player icons in the Home scene.
        /// </summary>
        public bool HasFinishedTheGameWithAllStars { get; set; }

        public int Age { get; set; }

        /// <summary>
        /// State of completion for the player profile.
        /// Can be 0,1,2,3. See PlayerProfile for further details.
        /// </summary>
        public int ProfileCompletion { get; set; }

        public int MaxStage { get; set; }
        public int MaxLearningBlock { get; set; }
        public int MaxPlaySession { get; set; }

        public int CurrentStage { get; set; }
        public int CurrentLearningBlock { get; set; }
        public int CurrentPlaySession { get; set; }

        public int TotalBones { get; set; }


        /// <summary>
        /// JSON data for the current customization set on Antura.
        /// </summary>
        public string CurrentAnturaCustomization { get; set; }

        /// <summary>
        /// JSON-serialized additional data, may be added as needed.
        /// </summary>
        public string AdditionalJsonData { get; set; }  

        public int CreationTimestamp { get; set; }

        public PlayerProfileData()
        {
        }

        public PlayerProfileData(PlayerIconData _IconData, int _Age, int totalBones, int _ProfileCompletion, string _AnturaCustomization = null)
        {
            Id = UNIQUE_ID;  // Only one record
            Age = _Age;
            //Name = ""; // not requested at the moment
            SetPlayerIconData(_IconData);
            ProfileCompletion = _ProfileCompletion;
            TotalBones = totalBones;
            SetMaxJourneyPosition(JourneyPosition.InitialJourneyPosition);
            SetCurrentJourneyPosition(JourneyPosition.InitialJourneyPosition);
            CreationTimestamp = GenericHelper.GetTimestampForNow();
            CurrentAnturaCustomization = _AnturaCustomization;
        }

        
        public void SetPlayerIconData(PlayerIconData data)
        {
            Uuid = data.Uuid;
            AvatarId = data.AvatarId;
            Gender = data.Gender;
            Tint = data.Tint;
            IsDemoUser = data.IsDemoUser;
            HasFinishedTheGame = data.HasFinishedTheGame;
            HasFinishedTheGameWithAllStars = data.HasFinishedTheGameWithAllStars;
        }
        
        public PlayerIconData GetPlayerIconData()
        {
            return new PlayerIconData(Uuid, AvatarId, Gender, Tint, IsDemoUser, HasFinishedTheGame, HasFinishedTheGameWithAllStars);
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

        public override string ToString()
        {
            return string.Format("ID{0},U{1},Ts{2}, MaxJ({3}.{4}.{5}), CurrentJ({6}.{7}.{8}), ProfCompl{9},",
                Id,
                Uuid,
                CreationTimestamp,

                MaxStage,
                MaxLearningBlock,
                MaxPlaySession,

                CurrentStage,
                CurrentLearningBlock,
                CurrentPlaySession,

                ProfileCompletion
            );
        }

        #endregion

    }
}