using SQLite;
using System;

namespace EA4S.Db
{

    [System.Serializable]
    public class LogScoresData : IData
    {
        public string Id { get; set; }
        public string Session { get; set; }
        public string Time { get; set; }
        public int PlayerID { get; set; }
        public string Table { get; set; }
        public string Element { get; set; }
        public float Score { get; set; }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},P{2},T{3},PS{4},C{5}",
                Session,
                Time,
                PlayerID,
                Table,
                Element,
                Score
                );
        }

    }
}