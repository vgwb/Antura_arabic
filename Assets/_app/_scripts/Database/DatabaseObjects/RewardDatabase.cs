using UnityEngine;
using System.Collections;

namespace EA4S.Db
{
    public class RewardDatabase : ScriptableObject
    {
        [SerializeField]
        public RewardTable table;
    }

}