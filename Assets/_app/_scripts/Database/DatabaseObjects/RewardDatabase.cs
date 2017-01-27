using UnityEngine;

namespace EA4S.Database
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