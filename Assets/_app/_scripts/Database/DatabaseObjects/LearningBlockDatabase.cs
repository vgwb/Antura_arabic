using UnityEngine;

namespace EA4S.Database
{
    /// <summary>
    /// Custom asset container for LearningBlockData. 
    /// </summary>
    public class LearningBlockDatabase : ScriptableObject
    {
        [SerializeField]
        public LearningBlockTable table;
    }

}