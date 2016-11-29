using UnityEngine;
using System.Collections;

namespace EA4S.Db
{
    public class LocalizationDatabase : ScriptableObject
    {
        [SerializeField]
        public LocalizationTable table;
    }

}