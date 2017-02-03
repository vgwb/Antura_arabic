using UnityEngine;
using System;
using System.Collections.Generic;
using EA4S.Core;
using EA4S.Helpers;
using EA4S.Profile;

namespace EA4S.Database
{
    /// <summary>
    /// Entry point for the rest of the application to access database entries.
    /// This class is responsible for loading all data and provide access to both static (learning) and dynamic (logging) data. 
    /// </summary>
    public class DatabaseManager
    {
        public const string STATIC_DATABASE_NAME = "Database";
        public const string STATIC_DATABASE_NAME_TEST = STATIC_DATABASE_NAME + "_Test";

        // DB references
        private DatabaseObject staticDb;
        private DBService dynamicDb;

        public DatabaseObject StaticDatabase {
            get {
                return staticDb;
            }
        }

        public bool HasLoadedPlayerProfile()
        {
            return dynamicDb != null;
        }

        #region Player assignment

        public void CreateDatabaseForPlayer(PlayerProfileData playerProfileData)
        {
            SetPlayerProfile(playerProfileData.PlayerId);
            UpdatePlayerProfileData(playerProfileData);
        }

        public PlayerProfileData LoadDatabaseForPlayer(int playerId)
        {
            SetPlayerProfile(playerId);
            return GetPlayerProfileData();
        }

        #endregion

        public DatabaseManager(bool useTestDatabase)
        {
            // Only the static DB is available until the player profile is also assigned
            LoadStaticDB(useTestDatabase);
        }

        private void SetPlayerProfile(int playerProfileId)
        {
            // SAFE MODE: we need to make sure that the static db has some entires, otherwise there is something wrong
            if (staticDb.GetPlaySessionTable().GetDataCount() == 0)
            {
                throw new System.Exception("Database is empty, it was probably not setup correctly. Make sure it has been statically loaded by the management scene.");
            }

            // We load the selected player profile
            LoadDynamicDbForPlayerProfile(playerProfileId);
        }


        void LoadStaticDB(bool useTestDatabase)
        {
            var dbName = STATIC_DATABASE_NAME;
            if (useTestDatabase) {
                dbName = STATIC_DATABASE_NAME_TEST;
            }

            this.staticDb = DatabaseObject.LoadDB(dbName);
        }

        #region Profile

        private void LoadDynamicDbForPlayerProfile(int profileId)
        {
            dynamicDb = new DBService("EA4S_Database" + "_" + profileId + ".sqlite3", profileId);
        }

        public void UnloadCurrentProfile()
        {
            dynamicDb = null;
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


        #region Player Profile Data

        public void UpdatePlayerProfileData(PlayerProfileData playerProfileData)
        {
            Debug.LogError("upd -> " + playerProfileData.ToString());
            dynamicDb.InsertOrReplace(playerProfileData);
        }

        public PlayerProfileData GetPlayerProfileData()
        {
            var data = dynamicDb.FindPlayerProfileDataById(PlayerProfileData.UNIQUE_ID);
            if (data != null) Debug.LogError("get -> " + data.ToString());
            return data;
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
        //    return GenericHelper.GetRandom(letterslist);
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

        public List<T> GetAllDynamicData<T>() where T : IData, new()
        {
            return dynamicDb.FindAll<T>();
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

        public List<T> FindDataByQuery<T>(string query) where T : IData, new()
        {
            return dynamicDb.FindByQuery<T>(query);
        }

        public List<object> FindCustomDataByQuery(SQLite.TableMapping mapping, string query)
        {
            return dynamicDb.FindByQueryCustom(mapping, query);
        }
        #endregion


        #region Score

        public void UpdateVocabularyScoreData(VocabularyDataType dataType, string elementId, float score, int timestamp = -1)
        {
            VocabularyScoreData data = null;
            if (timestamp > 0) data = new VocabularyScoreData(elementId, dataType, score, timestamp);
            else data = new VocabularyScoreData(elementId, dataType, score);
            dynamicDb.InsertOrReplace(data);
        }

        public void UpdateJourneyScoreData(JourneyDataType dataType, string elementId, int score, int timestamp = -1)
        {
            JourneyScoreData data = null;
            if (timestamp > 0) data = new JourneyScoreData(elementId, dataType, score, timestamp);
            else data = new JourneyScoreData(elementId, dataType, score);
            dynamicDb.InsertOrReplace(data);
        }

        public void UpdateMinigameScoreData(string elementId, float totalPlayTime, int score, int timestamp = -1)
        {
            MinigameScoreData data = null;
            if (timestamp > 0) data = new MinigameScoreData(elementId, score, totalPlayTime, timestamp);
            else data = new MinigameScoreData(elementId, score, totalPlayTime);
            dynamicDb.InsertOrReplace(data);
        }
        #endregion

        #region Reward Unlock

        public List<RewardPackUnlockData> GetAllRewardPackUnlockData()
        {
            return GetAllDynamicData<RewardPackUnlockData>();
        }

        public void UpdateRewardPackUnlockData(RewardPackUnlockData updatedData)
        {
            dynamicDb.InsertOrReplace(updatedData);
        }

        #endregion
    }
}
