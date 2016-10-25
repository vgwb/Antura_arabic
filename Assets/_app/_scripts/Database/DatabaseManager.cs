using UnityEngine;
using System;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S
{
    public class DatabaseManager
    {
        public const string STATIC_DATABASE_NAME = "EA4S.Database";
        public const string STATIC_DATABASE_NAME_TEST = STATIC_DATABASE_NAME + "_Test";

        // DB references
        private readonly Database staticDb;
        private DBService dynamicDb;

        // Profile
        //bool dbLoaded;

        public DatabaseManager(bool useTestDatabase)
        {
            var staticDbNameToLoad = STATIC_DATABASE_NAME;
            if (useTestDatabase) {
                staticDbNameToLoad = STATIC_DATABASE_NAME_TEST;
            }
            staticDb = Resources.Load<Database>(staticDbNameToLoad);

            // SAFE MODE: we load the profileId '1' for now to make everything work
            LoadDynamicDb(1);
        }

        #region Profile

        public void LoadDynamicDb(int profileId)
        {
            dynamicDb = new DBService("EA4S_Database" + "_" + profileId + ".bytes");
            //dbLoaded = true;
        }

        public void UnloadCurrentProfile()
        {
            dynamicDb = null;
            //dbLoaded = false;
        }

        public void CreateProfile()
        {
            dynamicDb.CreateAllTables();
        }

        public void RecreateProfile()
        {
            dynamicDb.RecreateAllTables();
        }

        public void DropProfile()
        {
            dynamicDb.DropAllTables();
        }

        #endregion

        #region Specific Dynamic Queries

        // Find all
        public List<LogInfoData> GetAllLogInfoData()
        {
            return dynamicDb.FindAll<LogInfoData>();
        }

        public List<LogMoodData> GetAllLogMoodData()
        {
            return dynamicDb.FindAll<LogMoodData>();
        }

        public List<LogLearnData> GetAllLogLearnData()
        {
            return dynamicDb.FindAll<LogLearnData>();
        }

        public List<LogPlayData> GetAllLogPlayData()
        {
            return dynamicDb.FindAll<LogPlayData>();
        }

        public List<ScoreData> GetAllScoreData()
        {
            return dynamicDb.FindAll<ScoreData>();
        }

        // Find all (expression)
        public List<LogInfoData> FindLogInfoData(System.Linq.Expressions.Expression<Func<LogInfoData, bool>> expression)
        {
            return dynamicDb.FindAll(expression);
        }

        // Get by id
        public LogInfoData GetLogInfoDataById(string id)
        {
            return dynamicDb.FindLogInfoDataById(id);
        }

        // Query
        public List<LogInfoData> FindLogInfoDataByQuery(string query)
        {
            return dynamicDb.FindByQuery<LogInfoData>(query);
        }

        public List<LogLearnData> FindLogLearnDataByQuery(string query)
        {
            return dynamicDb.FindByQuery<LogLearnData>(query);
        }

        public List<LogMoodData> FindLogMoodDataByQuery(string query)
        {
            return dynamicDb.FindByQuery<LogMoodData>(query);
        }

        public List<LogPlayData> FindLogPlayDataByQuery(string query)
        {
            return dynamicDb.FindByQuery<LogPlayData>(query);
        }

        public List<ScoreData> FindScoreDataByQuery(string query)
        {
            return dynamicDb.FindByQuery<ScoreData>(query);
        }

        public List<object> FindCustomDataByQuery(SQLite.TableMapping mapping, string query)
        {
            return dynamicDb.FindByQueryCustom(mapping, query);
        }

        // Utilities
        public string GetTableName<T>()
        {
            return dynamicDb.GetTableName<T>();
        }

        #endregion


        #region Specific Dynamic Inserts and Updates

        public void Insert<T>(T data) where T : IData, new()
        {
            dynamicDb.Insert(data);
        }

        public void InsertOrReplace<T>(T data) where T : IData, new()
        {
            dynamicDb.InsertOrReplace(data);
        }

        public void UpdateScoreData(DbTables table, string elementId, float score)
        {
            ScoreData data = new ScoreData(elementId, table, score);
            dynamicDb.InsertOrReplace(data);
        }
        public void UpdateScoreData(DbTables table, string elementId, float score, int timestamp)
        {
            ScoreData data = new ScoreData(elementId, table, score, timestamp);
            dynamicDb.InsertOrReplace(data);
        }


        #endregion


        #region Common Use Static Queries

        public List<MiniGameData> GetActiveMinigames()
        {
            return FindMiniGameData((x) => (x.Available && x.Type == MiniGameType.MiniGame));
        }

        #endregion

        #region Specific Static Queries

        public List<MiniGameData> GetAllMiniGameData()
        {
            return new List<MiniGameData>(staticDb.GetMiniGameTable().GetValuesTyped());
        }

        public List<MiniGameData> FindMiniGameData(Predicate<MiniGameData> predicate)
        {
            return staticDb.FindAll<MiniGameData>(staticDb.GetMiniGameTable(), predicate);
        }

        public List<EA4S.Db.LetterData> FindLetterData(Predicate<LetterData> predicate)
        {
            return staticDb.FindAll<EA4S.Db.LetterData>(staticDb.GetLetterTable(), predicate);
        }

        public List<EA4S.Db.LetterData> GetAllLetterData()
        {
            return FindLetterData((x) => (x.Kind == LetterKind.Letter));
            //return new List<EA4S.Db.LetterData>(db.GetLetterTable().Values);
        }

        public List<EA4S.Db.WordData> FindWordData(Predicate<EA4S.Db.WordData> predicate)
        {
            return staticDb.FindAll<EA4S.Db.WordData>(staticDb.GetWordTable(), predicate);
        }

        public List<PhraseData> FindPhraseData(Predicate<PhraseData> predicate)
        {
            return staticDb.FindAll<PhraseData>(staticDb.GetPhraseTable(), predicate);
        }

        public List<PlaySessionData> FindPlaySessionData(Predicate<PlaySessionData> predicate)
        {
            return staticDb.FindAll<PlaySessionData>(staticDb.GetPlaySessionTable(), predicate);
        }

        public List<StageData> FindStageData(Predicate<StageData> predicate)
        {
            return staticDb.FindAll<StageData>(staticDb.GetStageTable(), predicate);
        }

        public List<LocalizationData> FindLocalizationData(Predicate<LocalizationData> predicate)
        {
            return staticDb.FindAll<LocalizationData>(staticDb.GetLocalizationTable(), predicate);
        }

        public List<RewardData> FindRewardData(Predicate<RewardData> predicate)
        {
            return staticDb.FindAll<RewardData>(staticDb.GetRewardTable(), predicate);
        }

        public List<EA4S.Db.WordData> GetAllWordData()
        {
            return new List<EA4S.Db.WordData>(staticDb.GetWordTable().GetValuesTyped());
        }

        public List<PhraseData> GetAllPhraseData()
        {
            return new List<PhraseData>(staticDb.GetPhraseTable().GetValuesTyped());
        }

        public List<PlaySessionData> GetAllPlaySessionData()
        {
            return new List<PlaySessionData>(staticDb.GetPlaySessionTable().GetValuesTyped());
        }

        public List<StageData> GetAllStageData()
        {
            return new List<StageData>(staticDb.GetStageTable().GetValuesTyped());
        }

        public List<LocalizationData> GetAllLocalizationData()
        {
            return new List<LocalizationData>(staticDb.GetLocalizationTable().GetValuesTyped());
        }

        public List<RewardData> GetAllRewardData()
        {
            return new List<RewardData>(staticDb.GetRewardTable().GetValuesTyped());
        }

        public MiniGameData GetMiniGameDataByCode(MiniGameCode code)
        {
            return GetMiniGameDataById(code.ToString());
        }

        private MiniGameData GetMiniGameDataById(string id)
        {
            return staticDb.GetById<MiniGameData>(staticDb.GetMiniGameTable(), id);
        }

        public WordData GetWordDataById(string id)
        {
            return staticDb.GetById<WordData>(staticDb.GetWordTable(), id);
        }

        public WordData GetWordDataByRandom()
        {
            // TODO now locked to body parts for retrocompatibility
            var wordslist = FindWordData((x) => (x.Category == WordCategory.BodyPart));
            return GenericUtilities.GetRandom(wordslist);
        }

        public LetterData GetLetterDataById(string id)
        {
            return staticDb.GetById<LetterData>(staticDb.GetLetterTable(), id);
        }

        public LetterData GetLetterDataByRandom()
        {
            var letterslist = GetAllLetterData();
            return GenericUtilities.GetRandom(letterslist);
        }

        public PhraseData GetPhraseDataById(string id)
        {
            return staticDb.GetById<PhraseData>(staticDb.GetPhraseTable(), id);
        }

        public PlaySessionData GetPlaySessionDataById(string id)
        {
            return staticDb.GetById<PlaySessionData>(staticDb.GetPlaySessionTable(), id);
        }

        public StageData GetStageDataById(string id)
        {
            return staticDb.GetById<StageData>(staticDb.GetStageTable(), id);
        }

        public LocalizationData GetLocalizationDataById(string id)
        {
            return staticDb.GetById<LocalizationData>(staticDb.GetLocalizationTable(), id);
        }

        public RewardData GetRewardDataById(string id)
        {
            return staticDb.GetById<RewardData>(staticDb.GetRewardTable(), id);
        }

        #endregion



    }
}
