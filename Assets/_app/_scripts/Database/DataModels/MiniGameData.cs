using System;

namespace EA4S.Db
{
    public enum MiniGameType
    {
        MiniGame = 1,
        Assessment = 2
    }

    [Serializable]
    public class MiniGameData : IData
    {
        // Source
        public string Id;
        public MiniGameType Type;
        public string Variation;
        public string Status;
        public string Parent;
        public string Description;
        public string Title_En;
        public string Title_Ar;
        public string Scene;

        // Derived
        //public MiniGameCode MiniGameCode;   // @note: we could just get rid of the Id and use this instead
        public bool Available;  // @note: derived values are risky as they are derived from above, use a property instead?

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("[Minigame: id={0}, type={4}, status={1},  title_en={2}, title_ar={3}]", Id, Status, Title_En, Title_Ar, Type.ToString());
        }

        public string GetIconResourcePath()
        {
//            return "Images/GameIcons/minigame_icon_" + Id;
            return "Images/GameIcons/minigameIco_" + Id;
        }

    }
}