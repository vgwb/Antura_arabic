using UnityEngine;
using System;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S
{
    public class DatabaseManager
    {
        public const string STATIC_DATABASE_NAME = "Database";
        public const string STATIC_DATABASE_NAME_TEST = STATIC_DATABASE_NAME + "_Test";

        // DB references
        private Database staticDb;
        private DBService dynamicDb;

        public Database StaticDatabase {
            get {
                return staticDb;
            }
        }

        public DatabaseManager(bool useTestDatabase, PlayerProfile playerProfile)
        {
            LoadStaticDB(useTestDatabase);

            // SAFE MODE: we need to make sure that the static db has some entires, otherwise there is something wrong
            if (staticDb.GetPlaySessionTable().GetDataCount() == 0) {
                throw new System.Exception("Database is empty, it was probably not setup correctly. Make sure it has been statically loaded by the management scene.");
            }

            // We load the selected player profile
            LoadDynamicDbForPlayerProfile(playerProfile.Id);
        }

        void LoadStaticDB(bool useTestDatabase)
        {
            var dbName = STATIC_DATABASE_NAME;
            if (useTestDatabase) {
                dbName = STATIC_DATABASE_NAME_TEST;
            }

            this.staticDb = Database.LoadDB(dbName);
        }

        #region Profile

        public void LoadDynamicDbForPlayerProfile(int profileId)
        {
            dynamicDb = new DBService("EA4S_Database" + "_" + profileId + ".sqlite3", profileId);
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

        #region Utilities

        // Utilities
        public string GetTableName<T>()
        {
            return dynamicDb.GetTableName<T>();
        }

        public void Insert<T>(T data) where T : IData, new()
        {
            dynamicDb.Insert(data);
            if (AppConstants.DebugLogInserts)
                Debug.Log("DB Insert: " + data);
        }

        public void InsertOrReplace<T>(T data) where T : IData, new()
        {
            dynamicDb.InsertOrReplace(data);
            if (AppConstants.DebugLogInserts)
                Debug.Log("DB InsertOrReplace: " + data);
        }
        #endregion

        #region Letter
        public LetterData GetLetterDataById(string id)
        {
            return staticDb.GetById(staticDb.GetLetterTable(), id);
        }

        public List<LetterData> FindLetterData(Predicate<LetterData> predicate)
        {
            return staticDb.FindAll(staticDb.GetLetterTable(), predicate);
        }

        public List<LetterData> GetAllLetterData()
        {
            return FindLetterData((x) => (x.Kind == LetterDataKind.Letter));
        }

        //public LetterData GetLetterDataByRandom()
        //{
        //    var letterslist = GetAllLetterData();
        //    return GenericUtilities.GetRandom(letterslist);
        //}
        #endregion

        #region Word
        public WordData GetWordDataById(string id)
        {
            return staticDb.GetById(staticDb.GetWordTable(), id);
        }

        public WordData GetWordDataByRandom()
        {
            // TODO now locked to body parts for retrocompatibility
            var wordslist = FindWordData((x) => (x.Category == WordDataCategory.BodyPart));
            return wordslist.GetRandom();
        }

        public List<WordData> GetAllWordData()
        {
            return new List<WordData>(staticDb.GetWordTable().GetValuesTyped());
        }

        public List<WordData> FindWordData(Predicate<WordData> predicate)
        {
            return staticDb.FindAll(staticDb.GetWordTable(), predicate);
        }

        public List<WordData> FindWordDataByCategory(WordDataCategory wordCategory)
        {
            return staticDb.FindAll(staticDb.GetWordTable(), (x) => (x.Category == wordCategory));
        }
        #endregion

        #region MiniGame
        public MiniGameData GetMiniGameDataByCode(MiniGameCode code)
        {
            return GetMiniGameDataById(code.ToString());
        }

        private MiniGameData GetMiniGameDataById(string id)
        {
            return staticDb.GetById(staticDb.GetMiniGameTable(), id);
        }

        public List<MiniGameData> GetActiveMinigames()
        {
            return FindMiniGameData((x) => (x.Available && x.Type == MiniGameDataType.MiniGame));
        }

        public List<MiniGameData> GetAllMiniGameData()
        {
            return new List<MiniGameData>(staticDb.GetMiniGameTable().GetValuesTyped());
        }

        public List<MiniGameData> FindMiniGameData(Predicate<MiniGameData> predicate)
        {
            return staticDb.FindAll(staticDb.GetMiniGameTable(), predicate);
        }
        #endregion

        #region LearningBlock
        public LearningBlockData GetLearningBlockDataById(string id)
        {
            return staticDb.GetById(staticDb.GetLearningBlockTable(), id);
        }

        public List<LearningBlockData> FindLearningBlockData(Predicate<LearningBlockData> predicate)
        {
            return staticDb.FindAll(staticDb.GetLearningBlockTable(), predicate);
        }

        public List<LearningBlockData> GetAllLearningBlockData()
        {
            return new List<LearningBlockData>(staticDb.GetLearningBlockTable().GetValuesTyped());
        }

        // @note: new generic-only data getter, should be used instead of all the above ones
        public List<T> GetAllData<T>(DbTables table) where T : IData
        {
            return staticDb.GetAll<T>((SerializableDataTable<T>)staticDb.GetTable(table));
        }

        #endregion

        #region PlaySession
        public bool HasPlaySessionDataById(string id)
        {
            return staticDb.HasById(staticDb.GetPlaySessionTable(), id);
        }

        public PlaySessionData GetPlaySessionDataById(string id)
        {
            return staticDb.GetById(staticDb.GetPlaySessionTable(), id);
        }

        public List<PlaySessionData> FindPlaySessionData(Predicate<PlaySessionData> predicate)
        {
            return staticDb.FindAll(staticDb.GetPlaySessionTable(), predicate);
        }

        public List<PlaySessionData> GetPlaySessionsOfLearningBlock(LearningBlockData lb)
        {
            return FindPlaySessionData(x => x.Stage == lb.Stage && x.LearningBlock == lb.LearningBlock);
        }

        public List<PlaySessionData> GetAllPlaySessionData()
        {
            return new List<PlaySessionData>(staticDb.GetPlaySessionTable().GetValuesTyped());
        }
        #endregion

        #region Phrase
        public PhraseData GetPhraseDataById(string id)
        {
            return staticDb.GetById(staticDb.GetPhraseTable(), id);
        }

        public List<PhraseData> FindPhraseData(Predicate<PhraseData> predicate)
        {
            return staticDb.FindAll(staticDb.GetPhraseTable(), predicate);
        }

        public List<PhraseData> GetAllPhraseData()
        {
            return new List<PhraseData>(staticDb.GetPhraseTable().GetValuesTyped());
        }
        #endregion

        #region Localization
        public LocalizationData GetLocalizationDataById(string id)
        {
            var locData = staticDb.GetById(staticDb.GetLocalizationTable(), id);
            if (locData != null) {
                return locData;
            }
            return new LocalizationData { Id = id, Arabic = ("MISSING " + id), English = ("MISSING " + id), AudioFile = "" };
        }

        public List<LocalizationData> GetAllLocalizationData()
        {
            return new List<LocalizationData>(staticDb.GetLocalizationTable().GetValuesTyped());
        }

        public List<LocalizationData> FindLocalizationData(Predicate<LocalizationData> predicate)
        {
            return staticDb.FindAll(staticDb.GetLocalizationTable(), predicate);
        }
        #endregion

        #region Stage
        public StageData GetStageDataById(string id)
        {
            return staticDb.GetById(staticDb.GetStageTable(), id);
        }

        public List<StageData> GetAllStageData()
        {
            return new List<StageData>(staticDb.GetStageTable().GetValuesTyped());
        }

        public List<StageData> FindStageData(Predicate<StageData> predicate)
        {
            return staticDb.FindAll(staticDb.GetStageTable(), predicate);
        }
        #endregion

        #region Reward
        public RewardData GetRewardDataById(string id)
        {
            return staticDb.GetById(staticDb.GetRewardTable(), id);
        }

        public List<RewardData> GetAllRewardData()
        {
            return new List<RewardData>(staticDb.GetRewardTable().GetValuesTyped());
        }

        public List<RewardData> FindRewardData(Predicate<RewardData> predicate)
        {
            return staticDb.FindAll(staticDb.GetRewardTable(), predicate);
        }
        #endregion

        #region Log
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
        #endregion


        #region Score
        public void UpdateScoreData(DbTables table, string elementId, float score)
        {
            ScoreData data = new ScoreData(elementId, table, score);
            dynamicDb.InsertOrReplace(data);
        }

        public void Debug_UpdateScoreData(DbTables table, string elementId, float score, int timestamp)
        {
            ScoreData data = new ScoreData(elementId, table, score, timestamp);
            dynamicDb.InsertOrReplace(data);
        }
        #endregion
    }
}
