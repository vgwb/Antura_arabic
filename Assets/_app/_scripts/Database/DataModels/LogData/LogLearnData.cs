using SQLite;
using System;

namespace EA4S.Db
{

    /// <summary>
    /// Table can be: Letter, Phrase, Word
    /// </summary>

    [System.Serializable]
    public class LogLearnData : IData
    {
        public string Id { get; set; }
        public string Session { get; set; } // DailySession Id
        public int Timestamp { get; set; }

        public string PlaySession { get; set; }
        public MiniGameCode MiniGame { get; set; }
        public string TableName { get; set; } // word, letter, phrases (see DbTables enum)
        public string ElementId { get; set; }
        public float Score { get; set; } // -1.0 (bad)... 1.0 (perfect)

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},PS{2},MG{3},T{4},E{5},S{6}",
                Session,
                Timestamp,
                PlaySession,
                MiniGame,
                TableName,
                ElementId,
                Score
                );
        }

    }
}