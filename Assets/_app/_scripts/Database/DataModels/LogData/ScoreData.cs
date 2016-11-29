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
        [PrimaryKey]
        public string Id { get; set; }  // @note: with SQLite .Net we cannot have composite keys, so I created one

        public string TableName { get; set; }
        public string ElementId { get; set; }

        public float Score { get; set; } // [-1.0,1.0] for Letters, Words, Phrases. [1,3] for MiniGame, PlaySession, LearningBlock
        public int LastAccessTimestamp { get; set; }

        public ScoreData()
        {

        }
        public ScoreData(string elementId, DbTables table, float score) : this(elementId, table, score, GenericUtilities.GetTimestampForNow())
        {
        }
        public ScoreData(string elementId, DbTables table, float score, int timestamp)
        {
            this.ElementId = elementId;
            this.TableName = table.ToString();
            this.Id = TableName + "." + ElementId;
            this.Score = score;
            this.LastAccessTimestamp = timestamp;
        }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("T{0},E{1},S{2},T{3}",
                TableName,
                ElementId,
                Score,
                LastAccessTimestamp
                );
        }

    }
}