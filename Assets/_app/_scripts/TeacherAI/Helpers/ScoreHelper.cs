using System.Collections.Generic;
using System.Linq;
using EA4S.Db;

namespace EA4S.Db
{
    #region Info Wrappers

    public class DataInfo<T> where T : IData
    {
        public T data = default(T);
        public float score = 0f;
        public bool unlocked = false;
    }

    public class LearningBlockInfo : DataInfo<LearningBlockData>
    {
        public List<PlaySessionInfo> playSessions = new List<PlaySessionInfo>();
    }

    public class PlaySessionInfo : DataInfo<PlaySessionData> { }
    public class MiniGameInfo : DataInfo<MiniGameData> { }
    public class WordInfo : DataInfo<WordData> { }
    public class LetterInfo : DataInfo<LetterData> { }
    public class PhraseInfo : DataInfo<PhraseData> { }

    #endregion
}

namespace EA4S.Teacher
{

    public class ScoreHelper
    {
        DatabaseManager dbManager;

        public ScoreHelper(DatabaseManager _dbManager)
        {
            dbManager = _dbManager;
        }


        #region Info getters

        public List<MiniGameInfo> GetAllMiniGameInfo()
        {
            return GetAllInfo<MiniGameData,MiniGameInfo>(DbTables.MiniGames);
        }

        public List<PlaySessionInfo> GetAllPlaySessionInfo()
        {
            return GetAllInfo<PlaySessionData, PlaySessionInfo>(DbTables.PlaySessions);
        }

        public List<LearningBlockInfo> GetAllLearningBlockInfo()
        {
            return GetAllInfo<LearningBlockData, LearningBlockInfo>(DbTables.LearningBlocks);
        }

        public List<LetterInfo> GetAllLetterInfo()
        {
            return GetAllInfo<LetterData, LetterInfo>(DbTables.Letters);
        }

        public List<WordInfo> GetAllWordInfo()
        {
            return GetAllInfo<WordData, WordInfo>(DbTables.Words);
        }

        public List<PhraseInfo> GetAllPhraseInfo()
        {
            return GetAllInfo<PhraseData, PhraseInfo>(DbTables.Phrases);
        }

        public List<I> GetAllInfo<D,I>(DbTables table) where I : DataInfo<D>, new() where D : IData
        {
            // Retrieve all data
            List<D> data_list = dbManager.GetAllData<D>(table);
            return GetAllInfo<D,I>(data_list, table);
        }

        public List<I> GetAllInfo<D, I>(List<D> data_list, DbTables table) where I : DataInfo<D>, new() where D : IData
        {
            var info_list = new List<I>();

            // Build info instances for the given data
            foreach (var data in data_list)
            {
                var info = new I();
                info.data = data;
                info_list.Add(info);
            }

            // Find available scores
            string query = string.Format("SELECT * FROM ScoreData WHERE TableName = '" + table.ToString() + "' ORDER BY ElementId ");
            List<ScoreData> scoredata_list = dbManager.FindScoreDataByQuery(query);
            for (int i = 0; i < info_list.Count; i++)
            {
                var info = info_list[i];
                var scoredata = scoredata_list.Find(x => x.ElementId == info.data.GetId());
                if (scoredata != null)
                {
                    info.score = scoredata.Score;
                    info.unlocked = true;
                } else
                {
                    info.score = 0; // 0 until unlocked
                    info.unlocked = false;
                }
            }

            return info_list;
        }

        #endregion

        #region Score getters

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

        public List<ScoreData> GetCurrentScoreForPlaySessionsOfLearningBlock(int stage, int learningBlock)
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

        #endregion


        #region List Helper

        /// <summary>
        /// TODO take count of the numberOfItems variable
        /// </summary>
        /// <returns>The average score.</returns>
        /// <param name="_scoreList">Score list.</param>
        /// <param name="numberOfItems">Number of items.</param>
        public float GetAverageScore(List<ScoreData> _scoreList, int numberOfItems = -1)
        {
            var average = 0f;

            foreach (var item in _scoreList)
            {
                average += item.Score;
            }

            return (average / _scoreList.Count);
        }

        #endregion

    }
}
