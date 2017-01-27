using EA4S.Core;
using EA4S.Utilities;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Serialized information about the player. Used by the Player Profile.
    /// </summary>
    [System.Serializable]
    public class PlayerProfileData : IData
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string PlayerKey { get; set; }
        public int PlayerId { get; set; }
        public int AvatarId { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public int ProfileCompletion { get; set; }
        public int TotalNumberOfBones { get; set; }
        public int CreationTimestamp { get; set; }
        public int MaxJourneyPosition_Stage { get; set; }
        public int MaxJourneyPosition_LearningBlock { get; set; }
        public int MaxJourneyPosition_PlaySession { get; set; }
        public int CurrentJourneyPosition_Stage { get; set; }
        public int CurrentJourneyPosition_LearningBlock { get; set; }
        public int CurrentJourneyPosition_PlaySession { get; set; }

        public PlayerProfileData()
        {
        }

        public PlayerProfileData(string _PlayerKey, int _PlayerId, int _AvatarId, 
            int _Age, string _Name, int _TotalNumberOfBones)
        {
            Id = 1;  // Only one record
            PlayerKey = _PlayerKey;
            PlayerId = _PlayerId;
            AvatarId = _AvatarId;
            Age = _Age;
            Name = _Name;
            ProfileCompletion = 0;
            TotalNumberOfBones = _TotalNumberOfBones;
            MaxJourneyPosition_Stage = JourneyPosition.InitialJourneyPosition.Stage;
            MaxJourneyPosition_LearningBlock = JourneyPosition.InitialJourneyPosition.LearningBlock;
            MaxJourneyPosition_PlaySession = JourneyPosition.InitialJourneyPosition.PlaySession;
            CurrentJourneyPosition_Stage = JourneyPosition.InitialJourneyPosition.Stage;
            CurrentJourneyPosition_LearningBlock = JourneyPosition.InitialJourneyPosition.LearningBlock;
            CurrentJourneyPosition_PlaySession = JourneyPosition.InitialJourneyPosition.PlaySession;
            CreationTimestamp = GenericUtilities.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return string.Format("ID{0},P{1},Ts{2}",
                Id,
                PlayerId,
                CreationTimestamp
            );
        }

    }
}