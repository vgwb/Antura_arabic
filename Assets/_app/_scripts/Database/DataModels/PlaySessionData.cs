using SQLite;

namespace EA4S.Db
{
    public class PlaySessionData
    {
        [Indexed]
        public int stage { get; set; }
        [Indexed]
        public int learningblock { get; set; }
        [Indexed]
        public int playsession { get; set; }
        public string description { get; set; }

        public override string ToString()
        {
            return string.Format("[Playsession: S={0}, LB={1}, PS={2}, description={3}]", stage, learningblock, playsession, description);
        }
    }
}