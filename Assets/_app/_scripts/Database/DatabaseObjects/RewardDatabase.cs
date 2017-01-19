using UnityEngine;

namespace EA4S.Db
{
    /// <summary>
    /// Custom asset container for RewardData. 
    /// </summary>
    public class RewardDatabase : ScriptableObject
    {
        [SerializeField]
        public RewardTable table;
    }

}