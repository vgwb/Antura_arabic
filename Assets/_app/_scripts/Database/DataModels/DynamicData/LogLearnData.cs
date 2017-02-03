using EA4S.Helpers;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Learning achievements obtained at a given timestamp. Logged at runtime.
    /// Table can be: Letter, Phrase, Word
    /// </summary>
    [System.Serializable]
    public class LogLearnData : IData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string AppSession { get; set; } 
        public int Timestamp { get; set; }

        public string PlaySession { get; set; }
        public MiniGameCode MiniGame { get; set; }
        public VocabularyDataType VocabularyDataType { get; set; }
        public string ElementId { get; set; }
        public float Score { get; set; } // -1.0 (bad)... 1.0 (perfect)

        public LogLearnData()
        {
        }

        public LogLearnData(string _AppSession, string _PlaySession, MiniGameCode _MiniGame, VocabularyDataType _dataType, string _elementId, float _score)
        {
            AppSession = _AppSession;
            PlaySession = _PlaySession;
            MiniGame = _MiniGame;
            VocabularyDataType = _dataType;
            ElementId = _elementId;
            Score = _score;
            Timestamp = GenericHelper.GetTimestampForNow();
        }

        public string GetId()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return string.Format("S{0},T{1},PS{2},MG{3},VDT{4},E{5},S{6}",
                AppSession,
                Timestamp,
                PlaySession,
                MiniGame,
                VocabularyDataType,
                ElementId,
                Score
                );
        }

    }
}