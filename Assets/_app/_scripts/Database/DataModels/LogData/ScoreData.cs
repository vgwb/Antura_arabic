using SQLite;
using System;

namespace EA4S.Db
{
    /// <summary>
    /// summaries of values Table -> ElementId
    /// Table can be: Letter, MiniGame, Phrase, PlaySession, Stage, Reward, Word
    /// Keys:Table and ElementId
    /// </summary>

    [System.Serializable]
    public class ScoreData : IData
    {
        public string Table { get; set; } //key
        public string ElementId { get; set; } // key

        public int PlayerID { get; set; }
        public float Score { get; set; } // 0 ... 1.0, or 1f,2f,3f per PlaySession

        public string GetId()
        {
            return Table+"."+ElementId;
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},P{2},T{3},PS{4},C{5}",
                PlayerID,
                Table,
                ElementId,
                Score
                );
        }

    }
}