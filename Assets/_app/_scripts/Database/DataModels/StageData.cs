using System;

namespace EA4S.Database
{
    /// <summary>
    /// Data defining a Stage. 
    /// Defines the learning journey progression.
    /// A Stage contains multiple Learning Blocks.
    /// Each Stage is shown as a specific sub-map in the Map scene.
    /// <seealso cref="LearningBlockData"/>
    /// </summary>
    [Serializable]
    public class StageData : IData
    {
        public string Id;
        public string Title_En;
        public string Title_Ar;
        public string Description;

        public override string ToString()
        {
            return Id + ": " + Title_En;
        }

        public string GetId()
        {
            return Id;
        }
    }
}