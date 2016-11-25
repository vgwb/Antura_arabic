using UnityEngine;
using System.Collections;

namespace EA4S.Db
{
    public class LearningBlockDatabase : ScriptableObject
    {
        [SerializeField]
        public LearningBlockTable table;
    }

}