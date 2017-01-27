using UnityEngine;

namespace EA4S.Database
{
    /// <summary>
    /// Custom asset container for WordData. 
    /// </summary>
    public class WordDatabase : ScriptableObject
    {
        [SerializeField]
        public WordTable table;
    }

}