using UnityEngine;
using System.Collections;

namespace EA4S.Db
{
    public class MiniGameDatabase : ScriptableObject
    {
        [SerializeField]
        public MiniGameTable table;
    }

}