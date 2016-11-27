using UnityEngine;
using System.Collections;

namespace EA4S.Db
{
    public class LetterDatabase : ScriptableObject
    {
        [SerializeField]
        public LetterTable table;
    }

}