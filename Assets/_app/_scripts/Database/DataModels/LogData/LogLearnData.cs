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
        public int PlayerID { get; set; }

        public string PlaySession { get; set; }
        public MiniGameCode MiniGame { get; set; }
        public string Table { get; set; } // word, letter, phrases
        public string ElementId { get; set; }
        public float Score { get; set; } // -1.0 (bad)... 1.0 (perfect)

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},P{2},PS{3},MG{4},T{5},E{6},S{7}",
                Session,
                Timestamp,
                PlayerID,
                PlaySession,
                MiniGame,
                Table,
                ElementId,
                Score
                );
        }

    }
}