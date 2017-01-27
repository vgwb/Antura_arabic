using EA4S.Utilities;
using SQLite;

namespace EA4S.Database
{
    public enum VocabularyDataType
    {
        Letter,
        Word,
        Phrase
    }

    /// <summary>
    /// Summary score results relative to a vocabulary element. Updated at runtime.
    /// </summary>
    [System.Serializable]
    public class VocabularyScoreData : IData, IScoreData
    {
        public float GetScore()
        {
            return Score;
        }

        [PrimaryKey]
        public string Id { get; set; }

        // refactor: this should instead be an enum
        public VocabularyDataType VocabularyDataType { get; set; }
        public string ElementId { get; set; }

        public float Score { get; set; } // [-1.0,1.0] 
        public int LastAccessTimestamp { get; set; }

        public VocabularyScoreData()
        {

        }
        public VocabularyScoreData(string elementId, VocabularyDataType dataType, float score) :  this(elementId, dataType, score, GenericUtilities.GetTimestampForNow())
        {
        }
        public VocabularyScoreData(string elementId, VocabularyDataType dataType, float score, int timestamp)
        {
            ElementId = elementId;
            VocabularyDataType = dataType;
            Id = VocabularyDataType + "." + ElementId;
            Score = score;
            LastAccessTimestamp = timestamp;
        }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("T{0},E{1},S{2},T{3}",
                VocabularyDataType,
                ElementId,
                Score,
                LastAccessTimestamp
                );
        }

    }
}