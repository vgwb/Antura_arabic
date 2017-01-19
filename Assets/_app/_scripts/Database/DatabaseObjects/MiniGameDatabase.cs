using UnityEngine;

namespace EA4S.Db
{
    /// <summary>
    /// Custom asset container for MiniGameData. 
    /// </summary>
    public class MiniGameDatabase : ScriptableObject
    {
        [SerializeField]
        public MiniGameTable table;
    }

}