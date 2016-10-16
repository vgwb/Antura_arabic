using System;
using System.Collections.Generic;

namespace EA4S.Db
{
    [Serializable]
    public class PlaySessionData
    {
        public int Stage;
        public int LearningBlock;
        public int PlaySession;
        public string Description;
        public String AssessmentType;
        public String AssessmentData;

        // to be marsed during import 
        public String[] Words;
        public String[] Words_previous;
        public String[] Phrases;
        public String[] Phrases_previous;
        public List<MiniGameInPlaysession> Minigames;

        public override string ToString()
        {
            return string.Format("[Playsession: S={0}, LB={1}, PS={2}, description={3}]", Stage, LearningBlock, PlaySession, Description);
        }
    }

    [Serializable]
    public struct MiniGameInPlaysession
    {
        public MiniGameCode Code;
        public int Weight;
    }

}