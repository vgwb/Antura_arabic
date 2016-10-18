using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Db
{
    // @note: we use these serialized tables for faster access
    public class Database : ScriptableObject
    {
        [SerializeField]
        private MiniGameTable minigameTable;
        [SerializeField]
        private LetterTable letterTable;
        [SerializeField]
        private WordTable wordTable;
        [SerializeField]
        private PlaySessionTable playSessionTable;
        [SerializeField]
        private AssessmentTable assessmentTable;
        [SerializeField]
        private LocalizationTable localizationTable;
        [SerializeField]
        private PhraseTable phraseTable;
        [SerializeField]
        private StageTable stageTable;
        [SerializeField]
        private RewardTable rewardTable;


        #region Access

        public List<T> FindAll<T>(SerializableDataTable<T> table, Predicate<T> predicate) where T : IData 
        {
            List<T> allValues = new List<T>(table.Values);
            List<T> filtered = allValues.FindAll(predicate);
            return filtered;
        }

        public T GetById<T>(SerializableDataTable<T> table, string id) where T : IData 
        {
            if (!table.ContainsKey(id))
            {
                Debug.LogWarning("Cannot find id '" + id + "' in talbe " + table.GetType().Name);
                return default(T);
            }
            return table[id];
        }

        public IEnumerable<List<IData>> GetAllData()
        {
            foreach (var table in GetAllTables())
                yield return table.GetList();
        }

        public LetterTable GetLetterTable() { return this.letterTable; }
        public WordTable GetWordTable() { return this.wordTable; }
        public PhraseTable GetPhraseTable() { return this.phraseTable; }
        public MiniGameTable GetMiniGameTable() { return this.minigameTable; }
        public StageTable GetStageTable() { return this.stageTable; }
        public PlaySessionTable GetPlaySessionTable() { return this.playSessionTable; }
        public AssessmentTable GetAssessmentTable() { return this.assessmentTable; }
        public RewardTable GetRewardTable() { return this.rewardTable; }
        public LocalizationTable GetLocalizationTable() { return this.localizationTable; }

        // @note: interface for common use using categories
        public IData GetData(DatabaseCategory category, string id)
        {
            var table = GetTable(category);
            return table.GetValue(id);
        }

        public IEnumerable<IDataTable> GetAllTables()
        {
            yield return GetTable(DatabaseCategory.Letters);
            yield return GetTable(DatabaseCategory.Words);
            yield return GetTable(DatabaseCategory.Phrases);
            yield return GetTable(DatabaseCategory.MiniGames);
            yield return GetTable(DatabaseCategory.Stages);
            yield return GetTable(DatabaseCategory.PlaySessions);
            yield return GetTable(DatabaseCategory.Assessments);
            yield return GetTable(DatabaseCategory.Rewards);
            yield return GetTable(DatabaseCategory.Localizations);
        }

        private IDataTable GetTable(DatabaseCategory category)
        {
            IDataTable table = null;
            switch (category)
            {
                case DatabaseCategory.Letters: table = this.letterTable; break;
                case DatabaseCategory.Words: table = this.wordTable; break;
                case DatabaseCategory.Phrases: table = this.phraseTable; break;
                case DatabaseCategory.MiniGames: table = this.minigameTable; break;
                case DatabaseCategory.Stages: table = this.stageTable; break;
                case DatabaseCategory.PlaySessions: table = this.playSessionTable; break;
                case DatabaseCategory.Assessments: table = this.assessmentTable; break;
                case DatabaseCategory.Rewards: table = this.rewardTable; break;
                case DatabaseCategory.Localizations: table = this.localizationTable; break;
            }
            return table;
        }

        #endregion

    }

    public enum DatabaseCategory
    {
        Letters = 1,
        Words = 2,
        Phrases = 3,

        MiniGames = 10,
        Stages = 11,
        PlaySessions = 12,

        Assessments = 20,
        Localizations = 30,
        Rewards = 40
    }

}