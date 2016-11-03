using System;
using System.Collections.Generic;

namespace EA4S.Db
{

    [Serializable]
    public class PlaySessionData : IData
    {
        public string Id;
        public int Stage;
        public int LearningBlock;
        public int PlaySession;
        public string Type;
        public PlaySessionDataOrder Order;
        public List<MiniGameInPlaySession> Minigames;

        public string[] Letters;
        public string[] Words;
        public string[] Words_previous;
        public string[] Phrases;
        public string[] Phrases_previous;

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            string output = "";
            output += string.Format("[PlaySession: LB={0}, PS={1}]", Stage, LearningBlock, PlaySession);
            output += "\n MiniGames:";
            foreach (var minigame in Minigames) {
                if (minigame.Weight == 0) continue;
                output += "\n      " + minigame.MiniGameCode + ": \t" + minigame.Weight;
            }
            return output;
        }
    }

    [Serializable]
    public struct MiniGameInPlaySession
    {
        public MiniGameCode MiniGameCode;
        public int Weight;
    }


}