using Antura.Database;

namespace Antura.Map
{
    public class PlaySessionState
    {
        public PlaySessionData psData;
        public JourneyScoreData scoreData;

        public PlaySessionState(Database.PlaySessionData _psData, JourneyScoreData _scoreData)
        {
            this.psData = _psData;
            this.scoreData = _scoreData;
        }
    }
}