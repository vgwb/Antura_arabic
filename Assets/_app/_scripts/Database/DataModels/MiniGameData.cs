using System;

namespace EA4S.Db
{
    [Serializable]
    public class MiniGameData : IData
    {
        // Source
        public string Id;
        public string Variation;
        public string Status;
        public string Parent;
        public string Description;
        public string Title_En;
        public string Title_Ar;
        public string Scene;
        public string TitleNew;
        public string Team;

        // Derived
        // TODO: these could be made less verbose, now that we control serialization using 'DataParsers'
        private MiniGameCode _miniGameCode;
        public MiniGameCode MiniGameCode {
            get { return _miniGameCode; }
            set { _miniGameCode = value; }
        }
        private bool _available;
        public bool Available {
            get { return Status == "active"; }
            set { _available = value; }
        }


        public override string ToString()
        {
            return string.Format("[Minigame: id={0}, status={1},  title_en={2}, title_ar={3}]", Id, Status, Title_En, Title_Ar);
        }

        public string GetIconResourcePath()
        {
            return "Images/GameIcons/minigame_icon_" + Id;
        }

        public string GetID()
        {
            return Id;
        }
    }
}