using System;
using System.Collections.Generic;

namespace EA4S.Db
{
    public enum DidacticalFocus
    {
        Letters = 1,
        Shapes = 2,
        Words = 3,
        Phrases = 4
    }

    [Serializable]
    public class PlaySessionData : IData
    {
        public string Id;
        public int Stage;
        public int LearningBlock;
        public int PlaySession;
        public string Type;
        public string Description;
        public string IntroArabic;
        public DidacticalFocus Focus;
        public string[] Letters;
        public string[] Words;
        public string[] Words_previous;
        public string[] Phrases;
        public string[] Phrases_previous;
        public List<MiniGameInPlaySession> Minigames;

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            string output = "";
            output += string.Format("[PlaySession: S={0}, LB={1}, PS={2}, description={3}]", Stage, LearningBlock, PlaySession, Description);
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