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

        public DatabaseManager(bool useTestDatabase, PlayerProfile playerProfile)
        {
            var staticDbNameToLoad = STATIC_DATABASE_NAME;
            if (useTestDatabase) {
                staticDbNameToLoad = STATIC_DATABASE_NAME_TEST;
            }
            staticDb = Resources.Load<Database>(staticDbNameToLoad);

            // SAFE MODE: we need to make sure that the static db has some entires, otherwise there is something wrong
            if (staticDb.GetPlaySessionTable().GetDataCount() == 0) {
                throw new System.Exception("Database is empty, it was probably not setup correctly. Make sure it has been statically loaded by the management scene.");
            }

            // We load the selected player profile
            LoadDynamicDbForPlayerProfile(playerProfile.Id);
        }

        #region Profile

        public void LoadDynamicDbForPlayerProfile(int profileId)
        {
            dynamicDb = new DBService("EA4S_Database" + "_" + profileId + ".bytes", profileId);
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

        public void Debug_UpdateScoreData(DbTables table, string elementId, float score, int timestamp)
        {
            ScoreData data = new ScoreData(elementId, table, score, timestamp);
            dynamicDb.InsertOrReplace(data);
        }


        #endregion


        #region Common Use Static Queries

        public List<MiniGameData> GetActiveMinigames()
        {
            return FindMiniGameData((x) => (x.Available && x.Type == MiniGameDataType.MiniGame));
        }

        public List<PlaySessionData> GetPlaySessionsOfLearningBlock(LearningBlockData lb)
        {
            return FindPlaySessionData(x => x.Stage == lb.Stage && x.LearningBlock == lb.LearningBlock);
        }

        #endregion

        #region Specific Static Queries

        public List<MiniGameData> GetAllMiniGameData()
        {
            return new List<MiniGameData>(staticDb.GetMiniGameTable().GetValuesTyped());
        }

        public List<MiniGameData> FindMiniGameData(Predicate<MiniGameData> predicate)
        {
            return staticDb.FindAll(staticDb.GetMiniGameTable(), predicate);
        }

        public List<LetterData> FindLetterData(Predicate<LetterData> predicate)
        {
            return staticDb.FindAll(staticDb.GetLetterTable(), predicate);
        }

        public List<LetterData> GetAllLetterData()
        {
            return FindLetterData((x) => (x.Kind == LetterDataKind.Letter));
            //return new List<EA4S.Db.LetterData>(db.GetLetterTable().Values);
        }

        public List<WordData> FindWordData(Predicate<WordData> predicate)
        {
            return staticDb.FindAll(staticDb.GetWordTable(), predicate);
        }

        public List<PhraseData> FindPhraseData(Predicate<PhraseData> predicate)
        {
            return staticDb.FindAll(staticDb.GetPhraseTable(), predicate);
        }

        public List<PlaySessionData> FindPlaySessionData(Predicate<PlaySessionData> predicate)
        {
            return staticDb.FindAll(staticDb.GetPlaySessionTable(), predicate);
        }

        public List<LearningBlockData> FindLearningBlockData(Predicate<LearningBlockData> predicate)
        {
            return staticDb.FindAll(staticDb.GetLearningBlockTable(), predicate);
        }

        public List<StageData> FindStageData(Predicate<StageData> predicate)
        {
            return staticDb.FindAll(staticDb.GetStageTable(), predicate);
        }

        public List<LocalizationData> FindLocalizationData(Predicate<LocalizationData> predicate)
        {
            return staticDb.FindAll(staticDb.GetLocalizationTable(), predicate);
        }

        public List<RewardData> FindRewardData(Predicate<RewardData> predicate)
        {
            return staticDb.FindAll(staticDb.GetRewardTable(), predicate);
        }

        public List<WordData> GetAllWordData()
        {
            return new List<WordData>(staticDb.GetWordTable().GetValuesTyped());
        }

        public List<PhraseData> GetAllPhraseData()
        {
            return new List<PhraseData>(staticDb.GetPhraseTable().GetValuesTyped());
        }

        public List<PlaySessionData> GetAllPlaySessionData()
        {
            return new List<PlaySessionData>(staticDb.GetPlaySessionTable().GetValuesTyped());
        }

        public List<LearningBlockData> GetAllLearningBlockData()
        {
            return new List<LearningBlockData>(staticDb.GetLearningBlockTable().GetValuesTyped());
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
            return staticDb.GetById(staticDb.GetMiniGameTable(), id);
        }

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

        public LetterData GetLetterDataById(string id)
        {
            return staticDb.GetById(staticDb.GetLetterTable(), id);
        }

        /*public LetterData GetLetterDataByRandom()
        {
            var letterslist = GetAllLetterData();
            return GenericUtilities.GetRandom(letterslist);
        }*/

        public PhraseData GetPhraseDataById(string id)
        {
            return staticDb.GetById(staticDb.GetPhraseTable(), id);
        }

        public PlaySessionData GetPlaySessionDataById(string id)
        {
            return staticDb.GetById(staticDb.GetPlaySessionTable(), id);
        }

        public LearningBlockData GetLearningBlockDataById(string id)
        {
            return staticDb.GetById(staticDb.GetLearningBlockTable(), id);
        }

        public StageData GetStageDataById(string id)
        {
            return staticDb.GetById(staticDb.GetStageTable(), id);
        }

        public LocalizationData GetLocalizationDataById(string id)
        {
            var locData = staticDb.GetById(staticDb.GetLocalizationTable(), id);
            if (locData != null) {
                return locData;
            }
            return new LocalizationData { Id = id, Arabic = ("MISSING " + id), English = ("MISSING " + id) };
        }

        public RewardData GetRewardDataById(string id)
        {
            return staticDb.GetById(staticDb.GetRewardTable(), id);
        }

        #endregion



    }
}
