using System;

namespace EA4S.Profile
{
    [Serializable]
    public struct PlayerIcon
    {
        public string Uuid;
        public int AvatarId;
        public PlayerGender Gender;
        public PlayerTint Tint;

        public PlayerIcon(string _Uuid, int _AvatarId, PlayerGender _Gender, PlayerTint _Tint)
        {
            Uuid = _Uuid;
            AvatarId = _AvatarId;
            Gender = _Gender;
            Tint = _Tint;
        }
    }
}