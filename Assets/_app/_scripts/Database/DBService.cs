using EA4S.Core;
using UnityEngine;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;

namespace EA4S.Database
{
    /// <summary>
    /// Service that connects to SQLite.
    /// we are using Mysqlite from https://github.com/codecoding/SQLite4Unity3d 
    /// and engine from https://github.com/praeclarum/sqlite-net
    /// </summary>
    public class DBService
    {
        #region Paths

        public static string GetDatabaseFilePath(string fileName, string dirName)
        {
            return string.Format(@"{0}/{1}/{2}", Application.persistentDataPath, dirName, fileName);
        }

        public static string GetDatabaseDirectoryPath(string dirName)
        {
            return string.Format(@"{0}/{1}", Application.persistentDataPath, dirName);
        }

        #endregion

        #region Factory Methods

        public static DBService OpenFromFileName(bool createIfNotFound, string fileName, string dirName = AppConstants.DBPlayersFolder)
        {
            var dirPath = GetDatabaseDirectoryPath(dirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            var dbPath = GetDatabaseFilePath(fileName, dirName);
            return new DBService(createIfNotFound, dbPath);
        }

        public static DBService OpenFromFilePath(bool createIfNotFound, string filePath)
        {
            return new DBService(createIfNotFound, filePath);
        }

        public static DBService OpenFromPlayerUUID(bool createIfNotFound, string playerUuid, string fileName = "", string dirName = AppConstants.DBPlayersFolder)
        {
            if (fileName == "") fileName = AppConstants.GetPlayerDatabaseFilename(playerUuid);
            var dirPath = GetDatabaseDirectoryPath(dirName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            var dbPath = GetDatabaseFilePath(fileName, dirName);
            return new DBService(createIfNotFound, dbPath);
        }

        public static DBService ExportAndOpenFromPlayerUUID(string playerUuid, string fileName = "", string dirName = AppConstants.DBPlayersFolder)
        {
            if (fileName == "") fileName = AppConstants.GetPlayerDatabaseFilename(playerUuid);
            ExportFromPlayerUUID(playerUuid, fileName, dirName);
            return OpenFromPlayerUUID(false, playerUuid, AppConstants.GetPlayerDatabaseFilenameForExport(playerUuid), AppConstants.DbExportFolder);
        }

        public static void ExportFromPlayerUUID(string playerUuid, string fileName, string dirName)
        {
            var dbPath = GetDatabaseFilePath(fileName, dirName);

            if (!File.Exists(dbPath))
            {
                Debug.LogError("Could not find database for export at path: " + dbPath);
                return;
            }

            var dirNameExport = AppConstants.DbExportFolder;
            var dirPathExport = GetDatabaseDirectoryPath(dirNameExport);
            if (!Directory.Exists(dirPathExport))
            {
                Directory.CreateDirectory(dirPathExport);
            }

            var dbNameExport = AppConstants.GetPlayerDatabaseFilenameForExport(playerUuid);
            var dbPathExport = GetDatabaseFilePath(dbNameExport, dirNameExport);

            File.Copy(dbPath, dbPathExport);
        }

        #endregion

        SQLiteConnection _connection;

        private DBService(bool createIfNotFound, string dbPath)
        {
            //Debug.Log("Opening DBService at " + dbPath);

            _connection = null;
            // Try to open an existing DB connection, or create a new DB if it does not exist already
            try {
                _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite);
            } catch {
                if (createIfNotFound)
                {
                    _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
                    RegenerateDatabase();
                }
                else
                {
                    Debug.LogWarning("Could not find database at: " + dbPath);
                }
            }

            if (_connection != null)
            {
                // Check that the DB version is correct, otherwise recreate the tables
                GenerateTable<DatabaseInfoData>(true, false); // Makes sure that the database info data table exists
                var info = _connection.Find<DatabaseInfoData>(1);
                if (info == null || info.DynamicDbVersion != AppConstants.DynamicDbSchemeVersion)
                {
                    var lastVersion = info != null ? info.DynamicDbVersion : "NONE";
                    Debug.LogWarning("SQL database at path " + dbPath + " is outdated. Recreating it (from " +
                                     lastVersion + " to " + AppConstants.DynamicDbSchemeVersion + ")");
                    RegenerateDatabase();
                }
                //Debug.Log("Database ready at path " + dbPath + "   Version: " + (info != null ? info.DynamicDbVersion : "NONE"));
            }

        }

        #region Creation

        private void RegenerateDatabase()
        {
            RecreateAllTables();

            _connection.Insert(new DatabaseInfoData(AppConstants.DynamicDbSchemeVersion, AppConstants.StaticDbSchemeVersion));
        }

        public void GenerateTables(bool create, bool drop)
        {
            //Debug.Log(("GENERATING TABLES: " + create + " " + drop));

            // @note: define the DB structure here
            GenerateTable<DatabaseInfoData>(create, drop);
            GenerateTable<PlayerProfileData>(create, drop);
            GenerateTable<VocabularyScoreData>(create, drop);
            GenerateTable<MiniGameScoreData>(create, drop);
            GenerateTable<JourneyScoreData>(create, drop);
            GenerateTable<RewardPackUnlockData>(create, drop);

            GenerateTable<LogInfoData>(create, drop);
            GenerateTable<LogVocabularyScoreData>(create, drop);
            GenerateTable<LogMoodData>(create, drop);
            GenerateTable<LogPlayData>(create, drop);
            GenerateTable<LogMiniGameScoreData>(create, drop);
            GenerateTable<LogPlaySessionScoreData>(create, drop);
        }

        private void GenerateTable<T>(bool create, bool drop, string customTableName = "")
        {
            if (drop) _connection.DropTable<T>();
            if (create) _connection.CreateTable<T>(customTableName: customTableName);
        }

        public void CreateAllTables()
        {
            GenerateTables(true, false);
        }
        public void DropAllTables()
        {
            GenerateTables(false, true);
        }
        public void RecreateAllTables()
        {
            GenerateTables(true, true);
        }

        #endregion

        #region Deletion

        public void ForceFileDeletion()
        {
            Debug.LogWarning("Deleting database at path " + _connection.DatabasePath);
            File.Delete(_connection.DatabasePath);
        }

        #endregion

        #region Insert

        public void Insert<T>(T data) where T : IData, new()
        {
            if (AppConstants.DebugLogInserts)
                Debug.Log("DB Insert: " + data);
            _connection.Insert(data);
        }

        public void InsertOrReplace<T>(T data) where T : IData, new()
        {
            if (AppConstants.DebugLogInserts)
                Debug.Log("DB Insert: " + data);
            _connection.InsertOrReplace(data);
        }

        public void InsertAll<T>(IEnumerable<T> objects) where T : IData, new()
        {
            if (AppConstants.DebugLogInserts)
                foreach (var obj in objects)
                    Debug.Log("DB Insert: " + obj);
            _connection.InsertAll(objects);
        }

        public void InsertAllObjects(IEnumerable objects) 
        {
            if (AppConstants.DebugLogInserts)
                foreach (var obj in objects)
                    Debug.Log("DB Insert: " + obj);
            _connection.InsertAll(objects);
        }

        public void InsertOrReplaceAll<T>(IEnumerable<T> objects) where T : IData, new()
        {
            if (AppConstants.DebugLogInserts)
                foreach (var obj in objects)
                    Debug.Log("DB Insert: " + obj);
            _connection.InsertAll(objects, "OR REPLACE");
        }

        public void DeleteAll<T>() where T : IData, new()
        {
            _connection.DeleteAll<T>();
        }

        #endregion

        #region Find (simple queries)

        // Get one entry by ID
        public LogInfoData FindLogInfoDataById(string target_id)
        {
            return _connection.Table<LogInfoData>().Where((x) => (x.Id.Equals(target_id))).FirstOrDefault();
        }

        public PlayerProfileData GetPlayerProfileData()
        {
            return FindPlayerProfileDataById(PlayerProfileData.UNIQUE_ID);
        }

        private PlayerProfileData FindPlayerProfileDataById(string target_id)
        {
            return _connection.Table<PlayerProfileData>().Where((x) => (x.Id.Equals(target_id))).FirstOrDefault();
        }

        // @note: this cannot be used as the current SQLite implementation does not support Parameter expression nodes in LINQ
        // Get one entry by ID
        /*public T FindById<T>(string target_id) where T : IData, new()
        {
            return _connection.Table<T>().Where((x) => (x.GetId().Equals(target_id))).FirstOrDefault();
        }*/

        // select * from (Ttable)
        public List<T> FindAll<T>() where T : IData, new()
        {
            return new List<T>(_connection.Table<T>());
        }

        // select * from (Ttable) where (expression)
        public List<T> FindAll<T>(Expression<Func<T, bool>> expression) where T : IData, new()
        {
            return new List<T>(_connection.Table<T>().Where(expression));
        }

        // (query) from (Ttable)
        public List<T> Query<T>(string customQuery) where T : IData, new()
        {
            return _connection.Query<T>(customQuery);
        }

        public List<object> Query(Type t, string customQuery)
        {
            return _connection.Query(_connection.GetMapping(t), customQuery);
        }

        public string GetTableName<T>()
        {
            return _connection.GetMapping<T>().TableName;
        }

        // (query) from (Ttable) with a custom result
        public List<object> FindByQueryCustom(TableMapping mapping, string customQuery)
        {
            return _connection.Query(mapping, customQuery);
        }

        // entry point for custom queries
        public TableQuery<T> CreateQuery<T>() where T : IData, new()
        {
            return _connection.Table<T>();
        }

        /*public List<T> Query<T>(string query) where T : IData, new()
        {
            return _connection.Query<T>();
        }
        var query = string.Format("drop table if exists \"{0}\"", map.TableName);
        */

        public void CloseConnection()
        {
            _connection.Close();
        }

        #endregion

        #region Export

        public void GenerateStaticExportTables()
        {
            GenerateTable<StageData>(true, false, customTableName: "static_" + typeof(StageData).Name);
            GenerateTable<PlaySessionData>(true, false, customTableName: "static_" + typeof(PlaySessionData).Name);
            GenerateTable<LearningBlockData>(true, false, customTableName: "static_" + typeof(LearningBlockData).Name);
            GenerateTable<MiniGameData>(true, false, customTableName: "static_" + typeof(MiniGameData).Name);
            GenerateTable<LetterData>(true, false, customTableName: "static_" + typeof(LetterData).Name);
            GenerateTable<WordData>(true, false, customTableName: "static_" + typeof(WordData).Name);
            GenerateTable<PhraseData>(true, false, customTableName: "static_" + typeof(PhraseData).Name);
            GenerateTable<LocalizationData>(true, false, customTableName: "static_" + typeof(LocalizationData).Name);
            GenerateTable<RewardData>(true, false, customTableName: "static_" + typeof(RewardData).Name);
        }

        public void ExportEnum<T>() where T : struct, IConvertible
        {
            this.GenerateTable<EnumContainerData<T>>(true, false, customTableName: "enum_" + typeof(T).Name);
            this.InsertAll(CreateEnumContainerData<T>());
        }

        private IEnumerable<EnumContainerData<T>> CreateEnumContainerData<T>() where T : struct, IConvertible
        {
            foreach (T value in Enum.GetValues(typeof(T))) {
                var data = new EnumContainerData<T>();
                data.Set(value);
                yield return data;
            }
        }

        public class EnumContainerData<T> : IData where T : struct, IConvertible
        {
            [PrimaryKey]
            public int Value
            {
                get;
                set;
            }

            public string Name { get; set; }

            public EnumContainerData()
            {

            }

            public void Set(T enumValue)
            {
                if (!typeof(T).IsEnum) {
                    throw new ArgumentException("T must be an enumerated type");
                }

                Name = enumValue.ToString(CultureInfo.InvariantCulture);
                Value = Convert.ToInt32(enumValue);
            }

            public string GetId()
            {
                return Value.ToString();
            }
        }
        #endregion
    }

}