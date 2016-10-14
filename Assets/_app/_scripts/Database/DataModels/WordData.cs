using SQLite;

namespace EA4S.Db
{
    public class WordData
    {
        [PrimaryKey, AutoIncrement]
        public string Id { get; set; }
        public string Kind { get; set; }
        public string Category { get; set; }
        public string Stage { get; set; }
        public string English { get; set; }
        public string Word { get; set; }
        public string Letters { get; set; }
        public string Transliteration { get; set; }
        public string DifficultyLevel { get; set; }
        public string NumberOfLetters { get; set; }
        public string Group { get; set; }
    }
}