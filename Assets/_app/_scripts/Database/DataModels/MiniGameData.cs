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
        public MiniGameCode Code;
        public bool Available;
        public MiniGameType Type;
        public string Main;
        public string Variation;
        //public string Status; // deprecated
        //public string Parent; ; // deprecated
        public string Description;
        public string Title_En;
        public string Title_Ar;
        public string Scene;

        // Derived

        public string GetId()
        {
            return Code.ToString();
        }

        public override string ToString()
        {
            return string.Format("[Minigame: id={0}, type={4}, available={1},  title_en={2}, title_ar={3}]", GetId(), Available, Title_En, Title_Ar, Type.ToString());
        }

        public string GetIconResourcePath()
        {

//            return "Images/GameIcons/minigame_icon_" + Id;
            return "Images/GameIcons/minigameIco_" + Main;
        }

    }
}