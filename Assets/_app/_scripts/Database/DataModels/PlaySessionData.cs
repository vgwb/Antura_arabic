using System;
using System.Collections.Generic;

namespace EA4S.Database
{

    /// <summary>
    /// Data defining a Play Session. 
    /// Used to define the learning journey progression.
    /// Learning Blocks contain one or more play sessions and end with an assessment.
    /// A Play Session contains one or more minigames that can be selected to play when reaching that play session.
    /// <seealso cref="StageData"/>
    /// <seealso cref="LearningBlockData"/>
    /// <seealso cref="MiniGameData"/>
    /// </summary>
    [Serializable]
    public class PlaySessionData : IData
    {
        public string Id;
        public int Stage;
        public int LearningBlock;
        public int PlaySession;
        public string Type;
        public PlaySessionDataOrder Order;
        public int NumberOfMinigames;
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