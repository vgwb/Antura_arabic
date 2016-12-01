/*
Database is maintained by Stefano Cecere
we are using Mysqlite from https://github.com/codecoding/SQLite4Unity3d 
and engine from https://github.com/praeclarum/sqlite-net
*/

using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using SQLite;
using System.Linq.Expressions;
using System;

namespace EA4S.Db
{
    public class DBService
    {
        SQLiteConnection _connection;

        public DBService(string DatabaseName, int profileId)
        {
            var dbPath = string.Format(@"{0}/{1}", Application.persistentDataPath, DatabaseName);

            // Try to open an existing DB connection, or create a new DB if it does not exist already
            try {
                _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite);
            } catch {
                _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
                RegenerateDatabase(profileId);
            }

            // Check that the DB version is correct, otherwise recreate the tables
            GenerateTable<DatabaseInfoData>(true, false); // Makes sure that the database info data table exists
            var info = _connection.Find<DatabaseInfoData>(1);
            if (info == null || info.Version != AppConstants.DbSchemeVersion) {
                var lastVersion = info != null ? info.Version : "NONE";
                Debug.LogWarning("SQL database is outdated. Recreating it (from " + lastVersion + " to " + AppConstants.DbSchemeVersion + ")");
                RegenerateDatabase(profileId);
            }
            //Debug.Log("Database ready. Version " + dbPath + "v:" +info.Version);
        }

        #region Creation

        private void RegenerateDatabase(int profileId)
        {
            RecreateAllTables();

            _connection.Insert(new DatabaseInfoData(AppConstants.DbSchemeVersion, profileId));
        }

        public void GenerateTables(bool create, bool drop)
        {
            // @note: define the DB structure here
            GenerateTable<DatabaseInfoData>(create, drop);
            GenerateTable<LogInfoData>(create, drop);
            GenerateTable<LogLearnData>(create, drop);
            GenerateTable<LogMoodData>(create, drop);
            GenerateTable<LogPlayData>(create, drop);
            GenerateTable<ScoreData>(create, drop);
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
            _connection.Insert(data);
        }

        public void InsertOrReplace<T>(T data) where T : IData, new()
        {
            _connection.InsertOrReplace(data);
        }

        public void InsertAll<T>(System.Collections.IEnumerable objects) where T : IData, new()
        {
            _connection.InsertAll(objects);
        }

        #endregion

        #region Find (simple queries)

        // Get one entry by ID
        public LogInfoData FindLogInfoDataById(string target_id)
        {
            return _connection.Table<LogInfoData>().Where((x) => (x.Id.Equals(target_id))).FirstOrDefault();
        }

        // @note: this cannot be used as the current SQLite implementation does not support Parameter expression nodes in LINQ
        // Get one entry by ID
        public T FindById<T>(string target_id) where T : IData, new()
        {
            return _connection.Table<T>().Where((x) => (x.GetId().Equals(target_id))).FirstOrDefault();
        }

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