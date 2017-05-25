using UnityEngine;
using System;
using System.Collections.Generic;
using EA4S.Core;
using EA4S.Helpers;
using EA4S.Teacher;

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

        public List<Type> staticDataTypes = new List<Type>()
        {
            typeof(StageData),
            typeof(PlaySessionData),
            typeof(LearningBlockData),
            typeof(MiniGameData),
            typeof(LetterData),
            typeof(WordData),
            typeof(PhraseData),
            typeof(LocalizationData),
            typeof(RewardData)
        };

        public List<Type> dynamicDataTypes = new List<Type>()
        {
            typeof(VocabularyScoreData),
            typeof(JourneyScoreData),
            typeof(MiniGameScoreData),
            typeof(LogInfoData),
            typeof(LogMoodData),
            typeof(LogMiniGameScoreData),
            typeof(LogPlaySessionScoreData),
            typeof(LogVocabularyScoreData),
            typeof(DatabaseInfoData),
            typeof(PlayerProfileData),
            typeof(RewardPackUnlockData)
        };

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
            SetPlayerProfile(playerProfileData.Uuid);
            UpdatePlayerProfileData(playerProfileData);
        }

        public PlayerProfileData LoadDatabaseForPlayer(string playerUuid)
        {
            SetPlayerProfile(playerUuid);
            return GetPlayerProfileData();
        }

        #endregion

        public DatabaseManager(bool useTestDatabase)
        {
            // Only the static DB is available until the player profile is also assigned
            LoadStaticDB(useTestDatabase);
        }

        private void SetPlayerProfile(string playerUuid)
        {
            // SAFE MODE: we need to make sure that the static db has some entires, otherwise there is something wrong
            if (staticDb.GetPlaySessionTable().GetDataCount() == 0) {
                throw new System.Exception("Database is empty, it was probably not setup correctly. Make sure it has been statically loaded by the management scene.");
            }

            // We load the selected player profile
            LoadDynamicDbForPlayerProfile(playerUuid);
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

        private void LoadDynamicDbForPlayerProfile(string playerUuid)
        {
            dynamicDb = new DBService(playerUuid);
        }

        public void UnloadCurrentProfile()
        {
            dynamicDb = null;
        }

        public void DeleteCurrentProfile()
        {
            dynamicDb.ForceFileDeletion();
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
            dynamicDb.InsertOrReplace(playerProfileData);
        }

        public PlayerProfileData GetPlayerProfileData()
        {
            var data = dynamicDb.FindPlayerProfileDataById(PlayerProfileData.UNIQUE_ID);
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
        }

        public void InsertOrReplace<T>(T data) where T : IData, new()
        {
            dynamicDb.InsertOrReplace(data);
        }

        public void InsertAll<T>(IEnumerable<T> objects) where T : IData, new()
        {
            dynamicDb.InsertAll<T>(objects);
        }

        public void InsertOrReplaceAll<T>(IEnumerable<T> objects) where T : IData, new()
        {
            dynamicDb.InsertOrReplaceAll<T>(objects);
        }

        public void DeleteAll<T>() where T : IData, new()
        {
            dynamicDb.DeleteAll<T>();
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
            return new List<LetterData>(staticDb.GetLetterTable().GetValuesTyped());
        }

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

        public List<LogVocabularyScoreData> GetAllVocabularyScoreData()
        {
            return dynamicDb.FindAll<LogVocabularyScoreData>();
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
            return dynamicDb.Query<LogInfoData>(query);
        }

        public List<LogVocabularyScoreData> FindLogVocabularyScoreDataByQuery(string query)
        {
            return dynamicDb.Query<LogVocabularyScoreData>(query);
        }

        public List<LogMoodData> FindLogMoodDataByQuery(string query)
        {
            return dynamicDb.Query<LogMoodData>(query);
        }

        public List<LogPlayData> FindLogPlayDataByQuery(string query)
        {
            return dynamicDb.Query<LogPlayData>(query);
        }

        public List<T> Query<T>(string query) where T : IData, new()
        {
            return dynamicDb.Query<T>(query);
        }
        public List<object> Query(Type t, string query)
        {
            return dynamicDb.Query(t, query);
        }

        public List<object> FindCustomDataByQuery(SQLite.TableMapping mapping, string query)
        {
            return dynamicDb.FindByQueryCustom(mapping, query);
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

        public void UpdateRewardPackUnlockDataAll(List<RewardPackUnlockData> updatedDataList)
        {
            dynamicDb.InsertOrReplaceAll(updatedDataList);
        }

        #endregion

        public bool ExportDatabaseOfPlayer(string selectedPlayerId)
        {
            string playerUuid = selectedPlayerId;

            // Create a new service for the copied database
            // This will copy the current database
            var exportDbService = new DBService(playerUuid, true);
            exportDbService.GenerateStaticExportTables();

            foreach (var type in dynamicDataTypes)
            {
                PopulateUUID(type, playerUuid);
            }

            // Copy in the Static DB contents
            exportDbService.InsertAll(StaticDatabase.GetStageTable().GetValuesTyped());
            exportDbService.InsertAll(StaticDatabase.GetPlaySessionTable().GetValuesTyped());
            exportDbService.InsertAll(StaticDatabase.GetLearningBlockTable().GetValuesTyped());
            exportDbService.InsertAll(StaticDatabase.GetMiniGameTable().GetValuesTyped());
            exportDbService.InsertAll(StaticDatabase.GetLetterTable().GetValuesTyped());
            exportDbService.InsertAll(StaticDatabase.GetWordTable().GetValuesTyped());
            exportDbService.InsertAll(StaticDatabase.GetPhraseTable().GetValuesTyped());
            exportDbService.InsertAll(StaticDatabase.GetLocalizationTable().GetValuesTyped());
            exportDbService.InsertAll(StaticDatabase.GetRewardTable().GetValuesTyped());

            // Copy enums in too
            exportDbService.ExportEnum<AppScene>();
            exportDbService.ExportEnum<JourneyDataType>();
            exportDbService.ExportEnum<LearningBlockDataFocus>();
            exportDbService.ExportEnum<LetterDataKind>();
            exportDbService.ExportEnum<LetterDataSunMoon>();
            exportDbService.ExportEnum<LetterDataType>();
            exportDbService.ExportEnum<LocalizationDataId>();
            exportDbService.ExportEnum<MiniGameDataType>();
            exportDbService.ExportEnum<PhraseDataCategory>();
            exportDbService.ExportEnum<InfoEvent>();
            exportDbService.ExportEnum<PlaySkill>();
            exportDbService.ExportEnum<PlayEvent>();
            exportDbService.ExportEnum<PlaySessionDataOrder>();
            exportDbService.ExportEnum<RewardDataCategory>();
            exportDbService.ExportEnum<VocabularyDataGender>();
            exportDbService.ExportEnum<VocabularyDataType>();
            exportDbService.ExportEnum<WordDataArticle>();
            exportDbService.ExportEnum<WordDataCategory>();
            exportDbService.ExportEnum<WordDataForm>();
            exportDbService.ExportEnum<WordDataKind>();

            return true;
        }

        void PopulateUUID(Type t, string playerUuid)
        {
            string query = "UPDATE " + t.Name + " SET Uuid = \"" + playerUuid + "\"";
            Debug.Log(query);
            Query(t, query);
        }

    }
}
