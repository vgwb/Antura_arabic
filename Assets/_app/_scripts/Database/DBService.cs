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
using EA4S.Db;

public class DBService
{
    SQLiteConnection _connection;

    public DBService(string DatabaseName)
    {

#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/Database/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/Database/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/Database/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/Database/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);
#endif
        Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Database final PATH: " + dbPath);
    }


    public IEnumerable<MiniGameData> GetMinigames()
    {
        return _connection.Table<MiniGameData>().Where(x => x.status == "active");
    }

    public IEnumerable<PlaySessionData> GetPlaySessions(int stage)
    {
        return _connection.Table<PlaySessionData>().Where(x => x.stage == stage);
    }


    //public Person CreatePerson()
    //{
    //    var p = new Person {
    //        Name = "Johnny",
    //        Surname = "Mnemonic",
    //        Age = 21
    //    };
    //    _connection.Insert(p);
    //    return p;
    //}
}
