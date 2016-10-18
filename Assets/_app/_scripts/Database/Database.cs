using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S
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

        #region Common Use Queries

        public List<MiniGameData> FindAllActiveMinigames()
        {
            return FindAllMiniGameData((x) => (x.Available));
        }

        #endregion

        #region Specific Queries

        public List<MiniGameData> FindAllMiniGameData(Predicate<MiniGameData> predicate)
        {
           return FindAll<MiniGameData, MiniGameTable>(minigameTable, predicate);
        }
        public List<EA4S.Db.LetterData> FindAllLetterData(Predicate<EA4S.Db.LetterData> predicate)
        {
            return FindAll<EA4S.Db.LetterData, LetterTable>(letterTable, predicate);
        }
        public List<EA4S.Db.WordData> FindAllWordData(Predicate<EA4S.Db.WordData> predicate)
        {
            return FindAll<EA4S.Db.WordData, WordTable>(wordTable, predicate);
        }
        public List<PhraseData> FindAllPhraseData(Predicate<PhraseData> predicate)
        {
            return FindAll<PhraseData, PhraseTable>(phraseTable, predicate);
        }
        public List<PlaySessionData> FindAllPlaySessionData(Predicate<PlaySessionData> predicate)
        {
            return FindAll<PlaySessionData, PlaySessionTable>(playSessionTable, predicate);
        }
        public List<StageData> FindAllStageData(Predicate<StageData> predicate)
        {
            return FindAll<StageData, StageTable>(stageTable, predicate);
        }
        public List<AssessmentData> FindAllAssessmentData(Predicate<AssessmentData> predicate)
        {
            return FindAll<AssessmentData, AssessmentTable>(assessmentTable, predicate);
        }
        public List<LocalizationData> FindAllLocalizationData(Predicate<LocalizationData> predicate)
        {
            return FindAll<LocalizationData, LocalizationTable>(localizationTable, predicate);
        }
        public List<RewardData> FindAllRewardData(Predicate<RewardData> predicate)
        {
            return FindAll<RewardData, RewardTable>(rewardTable, predicate);
        }

        public List<MiniGameData> FindAllMiniGameData()
        {
            return new List<MiniGameData>(minigameTable.Values);
        }
        public List<EA4S.Db.LetterData> FindAllLetterData()
        {
            return new List<EA4S.Db.LetterData>(letterTable.Values);
        }
        public List<EA4S.Db.WordData> FindAllWordData()
        {
            return new List<EA4S.Db.WordData>(wordTable.Values);
        }
        public List<PhraseData> FindAllPhraseData()
        {
            return new List<PhraseData>(phraseTable.Values);
        }
        public List<PlaySessionData> FindAllPlaySessionData()
        {
            return new List<PlaySessionData>(playSessionTable.Values);
        }
        public List<StageData> FindAllStageData()
        {
            return new List<StageData>(stageTable.Values);
        }
        public List<AssessmentData> FindAllAssessmentData()
        {
            return new List<AssessmentData>(assessmentTable.Values);
        }
        public List<LocalizationData> FindAllLocalizationData()
        {
            return new List<LocalizationData>(localizationTable.Values);
        }
        public List<RewardData> FindAllRewardData()
        {
            return new List<RewardData>(rewardTable.Values);
        }


        public MiniGameData GetMiniGameDataById(string id)
        {
            return GetById<MiniGameData, MiniGameTable>(minigameTable, id);
        }
        public EA4S.Db.WordData GetWordDataById(string id)
        {
            return GetById<EA4S.Db.WordData, WordTable>(wordTable, id);
        }
        public EA4S.Db.LetterData GetLetterDataById(string id)
        {
            return GetById<EA4S.Db.LetterData, LetterTable>(letterTable, id);
        }
        public PhraseData GetPhraseDataById(string id)
        {
            return GetById<PhraseData, PhraseTable>(phraseTable, id);
        }
        public PlaySessionData GetPlaySessionDataById(string id)
        {
            return GetById<PlaySessionData, PlaySessionTable>(playSessionTable, id);
        }
        public StageData GetStageDataById(string id)
        {
            return GetById<StageData, StageTable>(stageTable, id);
        }
        public AssessmentData GetAssessmentDataById(string id)
        {
            return GetById<AssessmentData, AssessmentTable>(assessmentTable, id);
        }
        public LocalizationData GetLocalizationDataById(string id)
        {
            return GetById<LocalizationData, LocalizationTable>(localizationTable, id);
        }
        public RewardData GetRewardDataById(string id)
        {
            return GetById<RewardData, RewardTable>(rewardTable, id);
        }

        #endregion


        #region Access

        public List<T> FindAll<T, Ttable>(Ttable table, Predicate<T> predicate) where T : IData where Ttable : SerializableDataTable<T>
        {
            List<T> allValues = new List<T>(table.Values);
            List<T> filtered = allValues.FindAll(predicate);
            return filtered;
        }

        public T GetById<T, Ttable>(Ttable table, string id) where T : IData where Ttable : SerializableDataTable<T>
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