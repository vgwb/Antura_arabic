using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S
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


        public List<MiniGameData> GetActiveMinigames()
        {
            Debug.Log("GetActiveMinigames " + minigameTable.Count);
            var GameList = new List<MiniGameData>();
            foreach (var minigame in minigameTable.Values) {
                if (minigame.Available) {
                    GameList.Add(minigame);
                }
            }
            Debug.Log("GetActiveMinigames Active" + GameList.Count);
            return GameList;
        }
    }
}