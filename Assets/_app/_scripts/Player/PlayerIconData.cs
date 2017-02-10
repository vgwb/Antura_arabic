using System;

namespace EA4S.Profile
{
    /// <summary>
    /// this data contains the data to generate a saved player profile
    /// </summary>
    [Serializable]
    public struct PlayerIconData
    {
        public string Uuid;
        public int AvatarId;
        public PlayerGender Gender;
        public PlayerTint Tint;
        public bool IsDemoUser;

        public PlayerIconData(string _Uuid, int _AvatarId, PlayerGender _Gender, PlayerTint _Tint, bool _IsDemoUser)
        {
            Uuid = _Uuid;
            AvatarId = _AvatarId;
            Gender = _Gender;
            Tint = _Tint;
            IsDemoUser = _IsDemoUser;
        }
    }
}