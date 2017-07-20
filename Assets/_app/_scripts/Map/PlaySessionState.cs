namespace Antura.Map
{
    public class PlaySessionState
    {
        public Database.PlaySessionData data;
        public int score;

        public PlaySessionState(Database.PlaySessionData _data, int _score)
        {
            this.data = _data;
            this.score = _score;
        }
    }
}