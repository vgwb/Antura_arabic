using SQLite;
using System;

namespace EA4S.Db
{
    public enum LogDataType
    {
        Info,
        Play,
        Learn,
        Mood,
        Assessment
    }

    [System.Serializable]
    public class LogInfoData : IData
    {
        public string Id { get; set; }
        public string Session { get; set; }
        public string Time { get; set; }
        public int PlayerID { get; set; }
        public string Context { get; set; }
        public string Action { get; set; }
        public float Score { get; set; }
        public string RawData { get; set; }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},P{2},T{3},PS{4},C{5},A{6}",
                Session,
                Time,
                PlayerID,
                Context,
                Action,
                Score,
                RawData
                );
        }

    }
}