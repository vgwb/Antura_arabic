using System;

namespace EA4S.Db
{
    [Serializable]
    public class MiniGameData
    {
        public MiniGameCode Code;
        public string Id;
        public string Variation;
        public bool Available;
        public string Status;
        public string Parent;
        public string Description;
        public string Title_En;
        public string Title_Ar;
        public string Scene;

        public void InitMiniGameData(MiniGameCode code, string id, string title_ar, string englishTitle, string sceneName, bool available)
        {
            Code = code;
            Id = id;
            Title_Ar = title_ar;
            Title_En = englishTitle;
            Scene = sceneName;
            Available = available;
            Status = (available ? "active" : "");
        }

        public override string ToString()
        {
            return string.Format("[Minigame: id={0}, status={1},  title_en={2}, title_ar={3}]", Id, Status, Title_En, Title_Ar);
        }

        public string GetIconResourcePath()
        {
            return "Images/GameIcons/minigame_icon_" + Id;
        }

    }
}