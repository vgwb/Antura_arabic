using UnityEngine;
using System;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S
{
    public class DatabaseManager
    {
        // DB references
        private Database db;
        private DBService dbService;

        // Profile
        bool profileLoaded = false;

        public DatabaseManager()
        {
            db = Resources.Load<Database>("EA4S.Database");

            // SAFE MODE: we load the profileId '1' for now to make everything work
            LoadProfile(1);
        }

        #region Profile

        public void LoadProfile(int profileId)
        {
            this.dbService = new DBService("EA4S_Database" + "_" + profileId + ".bytes");
            profileLoaded = true;
        }

        public void UnloadCurrentProfile()
        {
            this.dbService = null;
            profileLoaded = false;
        }

        public void CreateProfile()
        {
            this.dbService.CreateAllTables();
        }

        public void RecreateProfile()
        {
            this.dbService.RecreateAllTables();
        }

        public void DropProfile()
        {
            this.dbService.DropAllTables();
        }

        #endregion


        #region Specific Runtime Queries

        // Find all
        public List<LogInfoData> FindAllLogInfoData()
        {
            return dbService.FindAll<LogInfoData>();
        }

        // Find all (expression)
        public List<LogInfoData> FindAllLogInfoData(System.Linq.Expressions.Expression<Func<LogInfoData, bool>> expression)
        {
            return dbService.FindAll(expression);
        }

        // Get by id
        public LogInfoData GetLogInfoDataById(string id)
        {
            return dbService.FindLogInfoDataById(id);
        }

        // Query
        public List<LogInfoData> FindLogInfoDataByQuery(string query)
        {
            return dbService.FindByQuery<LogInfoData>(query);
        }

        public List<LogLearnData> FindLogLearnDataByQuery(string query)
        {
            return dbService.FindByQuery<LogLearnData>(query);
        }

        public List<LogMoodData> FindLogMoodDataByQuery(string query)
        {
            return dbService.FindByQuery<LogMoodData>(query);
        }

        public List<LogPlayData> FindLogPlayDataByQuery(string query)
        {
            return dbService.FindByQuery<LogPlayData>(query);
        }

        public List<LogScoreData> FindLogScoreDataByQuery(string query)
        {
            return dbService.FindByQuery<LogScoreData>(query);
        }

        public List<object> FindCustomDataByQuery(SQLite.TableMapping mapping, string query)
        {
            return dbService.FindByQueryCustom(mapping, query);
        }

        // Utilities
        public string GetTableName<T>()
        {
            return dbService.GetTableName<T>();
        }

        #endregion


        #region Specific Runtime Inserts

        // Insert
        public void InsertLogInfoData(LogInfoData data)
        {
            dbService.Insert(data);
        }

        public void InsertLogLearnData(LogLearnData data)
        {
            dbService.Insert(data);
        }

        public void InsertLogMoodData(LogMoodData data)
        {
            dbService.Insert(data);
        }

        public void InsertLogPlayData(LogPlayData data)
        {
            dbService.Insert(data);
        }

        public void InsertLogScoreData(LogScoreData data)
        {
            dbService.Insert(data);
        }

        #endregion


        #region Common Use Static Queries

        public List<MiniGameData> FindAllActiveMinigames()
        {
            return FindAllMiniGameData((x) => (x.Available && x.Type == MiniGameType.MiniGame));
        }

        #endregion

        #region Specific Static Queries

        public List<MiniGameData> FindAllMiniGameData(Predicate<MiniGameData> predicate)
        {
            return db.FindAll<MiniGameData>(db.GetMiniGameTable(), predicate);
        }

        public List<EA4S.Db.LetterData> FindAllLetterData(Predicate<EA4S.Db.LetterData> predicate)
        {
            return db.FindAll<EA4S.Db.LetterData>(db.GetLetterTable(), predicate);
        }

        public List<EA4S.Db.LetterData> FindAllLetterData()
        {
            return FindAllLetterData((x) => (x.Kind == "letter"));
            //return new List<EA4S.Db.LetterData>(db.GetLetterTable().Values);
        }

        public List<EA4S.Db.WordData> FindAllWordData(Predicate<EA4S.Db.WordData> predicate)
        {
            return db.FindAll<EA4S.Db.WordData>(db.GetWordTable(), predicate);
        }

        public List<PhraseData> FindAllPhraseData(Predicate<PhraseData> predicate)
        {
            return db.FindAll<PhraseData>(db.GetPhraseTable(), predicate);
        }

        public List<PlaySessionData> FindAllPlaySessionData(Predicate<PlaySessionData> predicate)
        {
            return db.FindAll<PlaySessionData>(db.GetPlaySessionTable(), predicate);
        }

        public List<StageData> FindAllStageData(Predicate<StageData> predicate)
        {
            return db.FindAll<StageData>(db.GetStageTable(), predicate);
        }

        public List<LocalizationData> FindAllLocalizationData(Predicate<LocalizationData> predicate)
        {
            return db.FindAll<LocalizationData>(db.GetLocalizationTable(), predicate);
        }

        public List<RewardData> FindAllRewardData(Predicate<RewardData> predicate)
        {
            return db.FindAll<RewardData>(db.GetRewardTable(), predicate);
        }

        public List<MiniGameData> FindAllMiniGameData()
        {
            return new List<MiniGameData>(db.GetMiniGameTable().Values);
        }

        public List<EA4S.Db.WordData> FindAllWordData()
        {
            return new List<EA4S.Db.WordData>(db.GetWordTable().Values);
        }

        public List<PhraseData> FindAllPhraseData()
        {
            return new List<PhraseData>(db.GetPhraseTable().Values);
        }

        public List<PlaySessionData> FindAllPlaySessionData()
        {
            return new List<PlaySessionData>(db.GetPlaySessionTable().Values);
        }

        public List<StageData> FindAllStageData()
        {
            return new List<StageData>(db.GetStageTable().Values);
        }

        public List<LocalizationData> FindAllLocalizationData()
        {
            return new List<LocalizationData>(db.GetLocalizationTable().Values);
        }

        public List<RewardData> FindAllRewardData()
        {
            return new List<RewardData>(db.GetRewardTable().Values);
        }

        public MiniGameData GetMiniGameDataByCode(MiniGameCode code)
        {
            return GetMiniGameDataById(code.ToString());
        }

        private MiniGameData GetMiniGameDataById(string id)
        {
            return db.GetById<MiniGameData>(db.GetMiniGameTable(), id);
        }

        public EA4S.Db.WordData GetWordDataById(string id)
        {
            return db.GetById<EA4S.Db.WordData>(db.GetWordTable(), id);
        }

        public EA4S.Db.WordData GetWordDataByRandom()
        {
            // TODO now locked to body parts for retrocompatibility
            var wordslist = FindAllWordData((x) => (x.Category == "body_parts"));
            return GenericUtilites.GetRandom(wordslist);
        }

        public EA4S.Db.LetterData GetLetterDataById(string id)
        {
            return db.GetById<EA4S.Db.LetterData>(db.GetLetterTable(), id);
        }

        public EA4S.Db.LetterData GetLetterDataByRandom()
        {
            var letterslist = FindAllLetterData();
            return GenericUtilites.GetRandom(letterslist);
        }

        public PhraseData GetPhraseDataById(string id)
        {
            return db.GetById<PhraseData>(db.GetPhraseTable(), id);
        }

        public PlaySessionData GetPlaySessionDataById(string id)
        {
            return db.GetById<PlaySessionData>(db.GetPlaySessionTable(), id);
        }

        public StageData GetStageDataById(string id)
        {
            return db.GetById<StageData>(db.GetStageTable(), id);
        }

        public LocalizationData GetLocalizationDataById(string id)
        {
            return db.GetById<LocalizationData>(db.GetLocalizationTable(), id);
        }

        public RewardData GetRewardDataById(string id)
        {
            return db.GetById<RewardData>(db.GetRewardTable(), id);
        }

        #endregion



    }
}
