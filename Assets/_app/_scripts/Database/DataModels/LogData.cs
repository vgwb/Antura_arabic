using SQLite;
using System;

namespace EA4S.Db
{
    public class LogData
    {
        public string Session = string.Empty;
        public string Time = string.Empty;
        public string Area = string.Empty;
        public string Context = string.Empty;
        public string Action = string.Empty;
        public string RawData = string.Empty;

        public override string ToString()
        {
            return string.Format("{0},{1},{3},{4},{5},{6}" + Environment.NewLine,
                Session,
                Time,
                Area,
                Context,
                Action,
                RawData
                );
        }
    }
}