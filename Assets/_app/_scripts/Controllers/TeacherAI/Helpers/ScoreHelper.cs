using System.Collections.Generic;
using System.Linq;
using EA4S.Db;

namespace EA4S.Teacher
{
    public class ScoreHelper
    {
        DatabaseManager dbManager;

        public ScoreHelper(DatabaseManager _dbManager)
        {
            dbManager = _dbManager;
        }


        public List<float> GetLatestScoresForMiniGame(MiniGameCode minigameCode, int nLastDays)
        {
            int fromTimestamp = GenericUtilities.GetRelativeTimestampFromNow(-nLastDays);
            string query = string.Format("SELECT * FROM LogPlayData WHERE MiniGame = '{0}' AND Timestamp < {1}",
                (int)minigameCode, fromTimestamp);
            List<LogPlayData> list = dbManager.FindLogPlayDataByQuery(query);
            List<float> scores = list.ConvertAll(x => x.Score);
            return scores;
        }

        public List<ScoreData> GetCurrentScoreForAllPlaySessions()
        {
            string query = string.Format("SELECT * FROM ScoreData WHERE TableName = 'PlaySessions' ORDER BY ElementId ");
            List<ScoreData> list = dbManager.FindScoreDataByQuery(query);
            return list;
        }

        public List<ScoreData> GetCurrentScoreForPlaySessionsOfStage(int stage)
        {
            // First, get all data given a stage
            List<PlaySessionData> eligiblePlaySessionData_list = this.dbManager.FindPlaySessionData(x => x.Stage == stage);
            List<string> eligiblePlaySessionData_id_list = eligiblePlaySessionData_list.ConvertAll(x => x.Id);

            // Then, get all scores
            string query = string.Format("SELECT * FROM ScoreData WHERE TableName = 'PlaySessions'");
            List<ScoreData> all_score_list = dbManager.FindScoreDataByQuery(query);

            // At last, filter by the given stage
            List<ScoreData> filtered_score_list = all_score_list.FindAll(x => eligiblePlaySessionData_id_list.Contains(x.ElementId));
            return filtered_score_list;
        }

        public List<ScoreData> GetLearningBlockScores(int stage, int learningBlock)
        {
            // First, get all data given a stage
            List<PlaySessionData> eligiblePlaySessionData_list = this.dbManager.FindPlaySessionData(x => x.Stage == stage && x.LearningBlock == learningBlock); // TODO: make this readily available!
            List<string> eligiblePlaySessionData_id_list = eligiblePlaySessionData_list.ConvertAll(x => x.Id);

            // Then, get all scores
            string query = string.Format("SELECT * FROM ScoreData WHERE TableName = 'PlaySessions'");
            List<ScoreData> all_score_list = dbManager.FindScoreDataByQuery(query);

            // At last, filter
            List<ScoreData> filtered_score_list = all_score_list.FindAll(x => eligiblePlaySessionData_id_list.Contains(x.ElementId));
            return filtered_score_list;
        }


        public List<ScoreData> GetCurrentScoreForLearningBlocksOfStage(int stage)
        {
            // First, get all data given a stage
            List<LearningBlockData> eligibleLearningBlockData_list = this.dbManager.FindLearningBlockData(x => x.Stage == stage);
            List<string> eligibleLearningBlockData_id_list = eligibleLearningBlockData_list.ConvertAll(x => x.Id);

            // Then, get all scores
            string query = string.Format("SELECT * FROM ScoreData WHERE TableName = 'LearningBlock'");
            List<ScoreData> all_score_list = dbManager.FindScoreDataByQuery(query);

            // At last, filter by the given stage
            List<ScoreData> filtered_score_list = all_score_list.FindAll(x => eligibleLearningBlockData_id_list.Contains(x.ElementId));
            return filtered_score_list;
        }
    }
}
