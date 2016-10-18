using System;
using System.Collections.Generic;

namespace EA4S.Db
{
    [Serializable]
    public class PlaySessionData : IData
    {
        public int Stage { get; set; }
        public int LearningBlock { get; set; }
        public int PlaySession { get; set; }
        public string Description { get; set; }
        public DidacticalFocus Focus { get; set; }
        public string[] Letters { get; set; }
        public string[] Words { get; set; }
        public string[] Words_previous { get; set; }
        public string[] Phrases { get; set; }
        public string[] Phrases_previous { get; set; }
        public AssessmentType AssessmentType { get; set; }
        public string AssessmentData { get; set; }
        public List<MiniGameInPlaySession> Minigames { get; set; }

        public string GetId()
        {
            return Stage + "." + LearningBlock + "." + PlaySession;
        }

        public override string ToString()
        {
            string output = "";
            output += string.Format("[PlaySession: S={0}, LB={1}, PS={2}, description={3}]", Stage, LearningBlock, PlaySession, Description);
            output += "\n MiniGames:";
            foreach(var minigame in Minigames)
            {
                if (minigame.Weight == 0) continue;
                output += "\n      " + minigame.MiniGame_Id + ": \t" + minigame.Weight;
            }
            return output;
        }
        
    }

    [Serializable]
    public struct MiniGameInPlaySession
    {
        public string MiniGame_Id;
        public int Weight;
    }

    public enum DidacticalFocus
    {
        Letters = 1,
        Shapes = 2,
        Words = 3
    }

}