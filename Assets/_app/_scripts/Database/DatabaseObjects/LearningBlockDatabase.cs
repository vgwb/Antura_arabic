using UnityEngine;

namespace EA4S.Db
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