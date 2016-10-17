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
        private MiniGameCode _miniGameCode;
        public MiniGameCode MiniGameCode {
            get { return _miniGameCode; }
            set { _miniGameCode = value; }
        }
        private bool _available;
        public bool Available {
            get { return _available; }
            set { _available = value; }
        }

        // NOTE: THESE ARE NEEDED ONLY WHEN GENERATING THE DATA!
       
        public bool ValidateData()
        {
            // Derived data 
            _available = Status == "active";

            try
            {
                MiniGameCode parsed_enum = (MiniGameCode)System.Enum.Parse(typeof(MiniGameCode), Id);
                this._miniGameCode = parsed_enum;
            }
            catch (ArgumentException e)
            {
                UnityEngine.Debug.LogError("MiniGameData: " + "field Id is '" + this.Parent + "', not available in the enum values.");
                return false;
            }

            // Validation
            try
            {
                MiniGameCode parsed_enum = (MiniGameCode)System.Enum.Parse(typeof(MiniGameCode), this.Parent);
            }
            catch (ArgumentException e)
            {
                UnityEngine.Debug.LogError("MiniGameData: " + "field Parent is '" + this.Parent+"', not available in the enum values.");
                return false;
            }

            return true;
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