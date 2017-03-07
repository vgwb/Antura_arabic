using System;

namespace EA4S.Database
{
    /// <summary>
    /// Data defining a Learning Block. 
    /// Used to define the learning journey progression.
    /// A Stage contains multiple Learning Blocks.
    /// Learning Blocks contain one or more play sessions and end with an assessment.
    /// <seealso cref="StageData"/>
    /// <seealso cref="PlaySessionData"/>
    /// </summary>
    [Serializable]
    public class LearningBlockData : IData
    {
        public string Id;
        public int Stage;
        public int LearningBlock;
        public int NumberOfPlaySessions;
        public string Description_En;
        public string Description_Ar;
        public string Title_En;
        public string Title_Ar;
        //public string Reward;
        public LearningBlockDataFocus Focus;
        //        public string AssessmentData;

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            string output = "";
            output += string.Format("[LearningBlock: S={0}, LB={1}, description={2}]", Stage, LearningBlock, Description_En);
            return output;
        }

        public string GetTitleSoundFilename()
        {
            return "LB_" + Stage + "_" + LearningBlock.ToString("D2");
        }
    }

}