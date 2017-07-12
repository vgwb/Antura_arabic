using System;

namespace Antura.Profile
{
    /// <summary>
    /// Contains the data to generate a saved player profile (icon in the main menu)
    /// </summary>
    [Serializable]
    public struct PlayerIconData
    {
        public string Uuid;
        public int AvatarId;
        public PlayerGender Gender;
        public PlayerTint Tint;
        public bool IsDemoUser;
        public bool HasFinishedTheGame;
        public bool HasFinishedTheGameWithAllStars;

        public PlayerIconData(string _Uuid, int _AvatarId, PlayerGender _Gender, PlayerTint _Tint, bool _IsDemoUser,
            bool _HasFinishedTheGame, bool _HasFinishedTheGameWithAllStars)
        {
            Uuid = _Uuid;
            AvatarId = _AvatarId;
            Gender = _Gender;
            Tint = _Tint;
            IsDemoUser = _IsDemoUser;
            HasFinishedTheGame = _HasFinishedTheGame;
            HasFinishedTheGameWithAllStars = _HasFinishedTheGameWithAllStars;
        }
    }
}