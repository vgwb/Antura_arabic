using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Db
{
    // @note: we use these serialized tables for faster access
    public class Database : ScriptableObject
    {
        [SerializeField]
        public MiniGameTable minigameTable;
        [SerializeField]
        public LetterTable letterTable;
        [SerializeField]
        public WordTable wordTable;
        [SerializeField]
        public PlaySessionTable playSessionTable;
        [SerializeField]
        public AssessmentTable assessmentTable;
        [SerializeField]
        public LocalizationTable localizationTable;
        [SerializeField]
        public PhraseTable phraseTable;
        [SerializeField]
        public StageTable stageTable;
        [SerializeField]
        public RewardTable rewardTable;
    }
}