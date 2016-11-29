using UnityEngine;
using System.Collections;

namespace EA4S.Db
{
    public class StageDatabase : ScriptableObject
    {
        [SerializeField]
        public StageTable table;
    }

}