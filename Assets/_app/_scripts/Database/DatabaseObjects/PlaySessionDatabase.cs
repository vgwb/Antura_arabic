using UnityEngine;
using System.Collections;

namespace EA4S.Db
{
    public class PlaySessionDatabase : ScriptableObject
    {
        [SerializeField]
        public PlaySessionTable table;
    }

}