using System;

namespace EA4S.Db
{
    /// <summary>
    /// Data defining a Minigame.
    /// One entry should exist for each minigame (or minigame variation).
    /// A Play Session contains one or more minigames that can be selected to play when reaching that play session.
    /// <seealso cref="PlaySessionData"/>
    /// </summary>
    [Serializable]
    public class MiniGameData : IData
    {
        // Source
        public string Title_En;
        public string Title_Ar;
        public MiniGameCode Code;
        public bool Available;
        public MiniGameDataType Type;
        public string Main;
        public string Variation;
        public string Description;
        public string Scene;

        public string GetId()
        {
            return Code.ToString();
        }

        public override string ToString()
        {
            return string.Format("[Minigame: id={0}, type={4}, available={1},  title_en={2}, title_ar={3}]", GetId(), Available, Title_En, Title_Ar, Type.ToString());
        }

        public string GetTitleSoundFilename()
        {
            return GetId() + "_Title";
        }

        public string GetIconResourcePath()
        {
            return "Images/GameIcons/minigame_Ico_" + Main;
        }

        public string GetBadgeIconResourcePath()
        {
            return "Images/GameIcons/minigame_BadgeIco_" + Variation;
        }
    }
}