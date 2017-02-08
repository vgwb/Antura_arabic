using EA4S.Core;
using EA4S.Helpers;
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

        public string Guid { get; set; }

        public string PlayerKey { get; set; }
        public int PlayerId { get; set; }

        public int AvatarId { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public PlayerGender Gender { get; set; }
        public PlayerTint Tint { get; set; }
        public int ProfileCompletion { get; set; }
        public int TotalNumberOfBones { get; set; }
        public int CreationTimestamp { get; set; }
        public int MaxJourneyPosition_Stage { get; set; }
        public int MaxJourneyPosition_LearningBlock { get; set; }
        public int MaxJourneyPosition_PlaySession { get; set; }
        public int CurrentJourneyPosition_Stage { get; set; }
        public int CurrentJourneyPosition_LearningBlock { get; set; }
        public int CurrentJourneyPosition_PlaySession { get; set; }
        public string CurrentAnturaCustomization { get; set; }

        public PlayerProfileData()
        {
        }

        public PlayerProfileData(string _guid, string _PlayerKey, int _PlayerId, int _AvatarId, int _Age, PlayerGender _Gender, PlayerTint _Tint, int _TotalNumberOfBones, int _ProfileCompletion, string _AnturaCustomization = null)
        {
            Id = UNIQUE_ID;  // Only one record
            Guid = _guid;
            PlayerKey = _PlayerKey;
            PlayerId = _PlayerId;
            AvatarId = _AvatarId;
            Age = _Age;
            Name = ""; // not compiled at the moment
            Gender = _Gender;
            Tint = _Tint;
            ProfileCompletion = _ProfileCompletion;
            TotalNumberOfBones = _TotalNumberOfBones;
            SetMaxJourneyPosition(JourneyPosition.InitialJourneyPosition);
            SetCurrentJourneyPosition(JourneyPosition.InitialJourneyPosition);
            CreationTimestamp = GenericHelper.GetTimestampForNow();
            CurrentAnturaCustomization = _AnturaCustomization;
        }

        #region Journey Position

        public void SetMaxJourneyPosition(JourneyPosition pos)
        {
            MaxJourneyPosition_Stage = pos.Stage;
            MaxJourneyPosition_LearningBlock = pos.LearningBlock;
            MaxJourneyPosition_PlaySession = pos.PlaySession;
        }

        public void SetCurrentJourneyPosition(JourneyPosition pos)
        {
            CurrentJourneyPosition_Stage = pos.Stage;
            CurrentJourneyPosition_LearningBlock = pos.LearningBlock;
            CurrentJourneyPosition_PlaySession = pos.PlaySession;
        }

        public JourneyPosition GetMaxJourneyPosition()
        {
            return new JourneyPosition(MaxJourneyPosition_Stage, MaxJourneyPosition_LearningBlock, MaxJourneyPosition_PlaySession);
        }

        public JourneyPosition GetCurrentJourneyPosition()
        {
            return new JourneyPosition(CurrentJourneyPosition_Stage, CurrentJourneyPosition_LearningBlock, CurrentJourneyPosition_PlaySession);
        }

        #endregion

        #region Database API

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("ID{0},P{1},Ts{2}, MaxJ({3}.{4}.{5}), CurrentJ({6}.{7}.{8}), ProfCompl{9},",
                Id,
                PlayerId,
                CreationTimestamp,

                MaxJourneyPosition_Stage,
                MaxJourneyPosition_LearningBlock,
                MaxJourneyPosition_PlaySession,

                CurrentJourneyPosition_Stage,
                CurrentJourneyPosition_LearningBlock,
                CurrentJourneyPosition_PlaySession,

                ProfileCompletion
            );
        }

        #endregion

    }
}