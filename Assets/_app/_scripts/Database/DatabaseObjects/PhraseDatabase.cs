using UnityEngine;
using System.Collections;

namespace EA4S.Db
{
    public class PhraseDatabase : ScriptableObject
    {
        [SerializeField]
        public PhraseTable table;
    }

}