using SQLite;

namespace EA4S.Db
{
    public class MiniGameData
    {
        [PrimaryKey, AutoIncrement]
        public string id { get; set; }
        public string status { get; set; }
        public string parent { get; set; }
        public string description { get; set; }
        public string title_en { get; set; }
        public string variation_en { get; set; }
        public string title_ar { get; set; }
        public string variation_ar { get; set; }
        public string scene { get; set; }

        public override string ToString()
        {
            return string.Format("[Minigame: id={0}, status={1},  title_en={2}, title_ar={3}]", id, status, title_en, title_ar);
        }
    }
}