using UnityEngine;

namespace EA4S.Db
{
    /// <summary>
    /// Custom asset container for LocalizationData. 
    /// </summary>
    public class LocalizationDatabase : ScriptableObject
    {
        [SerializeField]
        public LocalizationTable table;
    }

}