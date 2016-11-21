using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Db
{
    public enum DbTables
    {
        Letters = 1,
        Words = 2,
        Phrases = 3,

        MiniGames = 10,
        Stages = 11,
        LearningBlocks = 12,
        PlaySessions = 13,

        Localizations = 30,
        Rewards = 40
    }

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
        private LearningBlockTable learningBlockTable;
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
            var allValues = new List<T>(table.GetValuesTyped());
            var filtered = allValues.FindAll(predicate);
            return filtered;
        }

        public T GetById<T>(SerializableDataTable<T> table, string id) where T : IData
        {
            T value = (T)table.GetValue(id);
            if (value == null) {
                Debug.LogWarning("Cannot find id '" + id + "' in table " + table.GetType().Name);
                return default(T);
            }
            return value;
        }

        public bool HasById<T>(SerializableDataTable<T> table, string id) where T : IData
        {
            T value = (T)table.GetValue(id);
            if (value == null) return false;
            return true;
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
        public LearningBlockTable GetLearningBlockTable() { return this.learningBlockTable; }
        public RewardTable GetRewardTable() { return this.rewardTable; }
        public LocalizationTable GetLocalizationTable() { return this.localizationTable; }

        // @note: interface for common use using categories
        public IData GetData(DbTables tables, string id)
        {
            var table = GetTable(tables);
            return table.GetValue(id);
        }

        public IEnumerable<IDataTable> GetAllTables()
        {
            yield return GetTable(DbTables.Letters);
            yield return GetTable(DbTables.Words);
            yield return GetTable(DbTables.Phrases);
            yield return GetTable(DbTables.MiniGames);
            yield return GetTable(DbTables.Stages);
            yield return GetTable(DbTables.PlaySessions);
            yield return GetTable(DbTables.Rewards);
            yield return GetTable(DbTables.Localizations);
        }

        public IDataTable GetTable(DbTables tables)
        {
            IDataTable table = null;
            switch (tables) {
                case DbTables.Letters: table = letterTable; break;
                case DbTables.Words: table = wordTable; break;
                case DbTables.Phrases: table = phraseTable; break;
                case DbTables.MiniGames: table = minigameTable; break;
                case DbTables.Stages: table = stageTable; break;
                case DbTables.PlaySessions: table = playSessionTable; break;
                case DbTables.LearningBlocks: table = learningBlockTable; break;
                case DbTables.Rewards: table = rewardTable; break;
                case DbTables.Localizations: table = localizationTable; break;
                default:
                    throw new ArgumentOutOfRangeException("tables", tables, null);
            }
            return table;
        }

        #endregion

    }

}