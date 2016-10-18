using System;

namespace EA4S.Db
{
    [Serializable]
    public class MiniGameData : IData
    {
        // Source
        public string Id { get; set; }
        public string Variation { get; set; }
        public string Status { get; set; }
        public string Parent { get; set; }
        public string Description { get; set; }
        public string Title_En { get; set; }
        public string Title_Ar { get; set; }
        public string Scene { get; set; }
        public string TitleNew { get; set; }
        public string Team { get; set; }

        // Derived
        //public MiniGameCode MiniGameCode;   // @note: we could just get rid of the Id and use this instead
        public bool Available;  // @note: derived values are risky as they are derived from above, use a property instead?

        public string GetId()
        {
            return Id;
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