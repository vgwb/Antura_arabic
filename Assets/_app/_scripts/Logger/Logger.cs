using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

namespace EA4S_deprecated
{
    public static class LoggerEA4S
    {

        static string Filename = "AnturaLog.csv";
        public static string SessionID = "0000000000";
        static string buffer = string.Empty;

        public static void Log(string _area, string _context, string _action, string _data)
        {
            LogData data = new LogData() {
                SessionId = SessionID,
                TimeInSeconds = DateTime.Now.Ticks.ToString(),
                Timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss"),
                Area = _area,
                Context = _context,
                Action = _action,
                RawData = _data,
            };
            buffer += data;
        }

        public static void Save()
        {
            if (!File.Exists(getPath())) {
                FileStream fs = File.Create(getPath());
                fs.Close();
                // First row
                File.WriteAllText(getPath(), new LogData() {
                    SessionId = "SessionId",
                    TimeInSeconds = "Ticks",
                    Timestamp = "Timestamp",
                    Area = "Area",
                    Context = "Context",
                    Action = "Action",
                    RawData = "RawData"
                }.ToString() + buffer);
            } else {
                File.AppendAllText(getPath(), buffer);
            }

            //string tmpBuffer = File.ReadAllText(getPath());
            //tmpBuffer += buffer;
            //File.WriteAllText(getPath(), tmpBuffer);



            ////Debug.Log(buffer);
            buffer = string.Empty;
        }

        // Following method is used to retrive the relative path as device platform
        private static string getPath()
        {
#if UNITY_EDITOR
            //return @"c:\tmp\" + Filename;
            return Application.persistentDataPath + "/" + Filename;
#elif UNITY_ANDROID
            return Application.persistentDataPath +"/"+ Filename;
#elif UNITY_IPHONE
            return Application.persistentDataPath+"/"+Filename;
#else
            return Application.dataPath +"/"+Filename;
#endif
        }
    }

    [Serializable]
    public class LogData
    {
        public string SessionId = string.Empty;
        public string TimeInSeconds = string.Empty;
        public string Timestamp = string.Empty;
        public string Area = string.Empty;
        public string Context = string.Empty;
        public string Action = string.Empty;
        public string RawData = string.Empty;


        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6}" + Environment.NewLine,
                SessionId,
                TimeInSeconds,
                Timestamp,
                Area,
                Context,
                Action,
                RawData
                );
        }
    }
}