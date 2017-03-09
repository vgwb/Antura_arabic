using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using SQLite;
using System.Linq.Expressions;
using System;
using System.IO;
using EA4S.Core;

namespace EA4S.Database
{
    /// <summary>
    /// Service that connects to SQLite.
    ///  we are using Mysqlite from https://github.com/codecoding/SQLite4Unity3d 
    /// and engine from https://github.com/praeclarum/sqlite-net
    /// </summary>
    public class DBService
    {
        SQLiteConnection _connection;

        public DBService(string playerUuid)
        {
            var databaseName = "Antura_Player_" + playerUuid + ".sqlite3";
            var dirPath = string.Format(@"{0}/{1}", Application.persistentDataPath, "players");
            var dbPath = string.Format(@"{0}/{1}/{2}", Application.persistentDataPath, "players", databaseName);

            if (!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }

            // Try to open an existing DB connection, or create a new DB if it does not exist already
            try {
                _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite);
            } catch {
                _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
                RegenerateDatabase();
            }

            // Check that the DB version is correct, otherwise recreate the tables
            GenerateTable<DatabaseInfoData>(true, false); // Makes sure that the database info data table exists
            var info = _connection.Find<DatabaseInfoData>(1);
            if (info == null || info.DynamicDbVersion != AppConstants.DynamicDbSchemeVersion) {
                var lastVersion = info != null ? info.DynamicDbVersion : "NONE";
                Debug.LogWarning("SQL database for player " + playerUuid + " is outdated. Recreating it (from " + lastVersion + " to " + AppConstants.DynamicDbSchemeVersion + ")");
                RegenerateDatabase();
            }

            //Debug.Log("Database ready with UUID " + playerUuid + "   Version: " + (info != null ? info.DynamicDbVersion : "NONE"));
        }

        public void ForceFileDeletion()
        {
            Debug.LogError("MySQL File deletion not yet implemented.");
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

        private void GenerateTable<T>(bool create, bool drop)
        {
            if (drop) _connection.DropTable<T>();
            if (create) _connection.CreateTable<T>();
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

        public PlayerProfileData FindPlayerProfileDataById(string target_id)
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
        public List<T> FindByQuery<T>(string customQuery) where T : IData, new()
        {
            return _connection.Query<T>(customQuery);
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

        #endregion

    }
}