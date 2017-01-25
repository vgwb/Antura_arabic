using UnityEngine;

namespace EA4S.Db
{
    /// <summary>
    /// Custom asset container for PlaySessionData. 
    /// </summary>
    public class PlaySessionDatabase : ScriptableObject
    {
        [SerializeField]
        public PlaySessionTable table;
    }

}