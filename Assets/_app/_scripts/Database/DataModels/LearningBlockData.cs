using System;
using System.Collections.Generic;

namespace EA4S.Db
{

    [Serializable]
    public class LearningBlockData : IData
    {
        public string Id;
        public int Stage;
        public int LearningBlock;
        public int NumberOfPlaySessions;
        public string Description;
        public string Title_En;
        public string Title_Ar;
        public string Reward;
        public LearningBlockDataFocus Focus;
        //        public string AssessmentData;

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            string output = "";
            output += string.Format("[LearningBlock: S={0}, LB={1}, description={2}]", Stage, LearningBlock, Description);
            return output;
        }

        public string GetTitleSoundFilename()
        {
            return "LB_" + Stage + "_" + LearningBlock.ToString("D2");
        }
    }

}