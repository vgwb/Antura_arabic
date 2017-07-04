namespace EA4S.Map
{
    public class PlaySessionState
    {
        public Database.PlaySessionData data;
        public float score;

        public PlaySessionState(Database.PlaySessionData _data, float _score)
        {
            this.data = _data;
            this.score = _score;
        }
    }
}