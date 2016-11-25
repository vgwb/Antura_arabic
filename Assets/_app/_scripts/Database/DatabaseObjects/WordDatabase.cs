using UnityEngine;
using System.Collections;

namespace EA4S.Db
{
    public class WordDatabase : ScriptableObject
    {
        [SerializeField]
        public WordTable table;
    }

}