using UnityEngine;

namespace EA4S.Database
{
    /// <summary>
    /// Custom asset container for PhraseData. 
    /// </summary>
    public class PhraseDatabase : ScriptableObject
    {
        [SerializeField]
        public PhraseTable table;
    }

}