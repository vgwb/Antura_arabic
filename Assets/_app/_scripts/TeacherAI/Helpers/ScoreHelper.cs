using System.Collections.Generic;
using EA4S.Database;
using EA4S.Utilities;

// @todo: refactor this to separate JourneyInfo from VocabularyInfo (different rules)
namespace EA4S.Database
{
    #region Info Wrappers

    /// <summary>
    /// Pairs the data related to a specific type T with its score and unlock state.
    /// </summary>
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

    /// <summary>
    /// Utilities that help in retrieving and updating score values for learning and progression data.
    /// </summary>
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
            string query = string.Format("SELECT * FROM JourneyScoreData WHERE JourneyDataType = '" + table.ToString() + "' ORDER BY ElementId ");
            List<JourneyScoreData> scoredata_list = dbManager.FindDataByQuery<JourneyScoreData>(query);
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

        public List<JourneyScoreData> GetCurrentScoreForAllPlaySessions()
        {
            string query = string.Format("SELECT * FROM JourneyScoreData WHERE JourneyDataType = '{0}'  ORDER BY ElementId", JourneyDataType.PlaySession);
            List<JourneyScoreData> list = dbManager.FindDataByQuery<JourneyScoreData>(query);
            return list;
        }

        public List<JourneyScoreData> GetCurrentScoreForPlaySessionsOfStage(int stage)
        {
            // First, get all data given a stage
            List<PlaySessionData> eligiblePlaySessionData_list = this.dbManager.FindPlaySessionData(x => x.Stage == stage);
            List<string> eligiblePlaySessionData_id_list = eligiblePlaySessionData_list.ConvertAll(x => x.Id);

            // Then, get all scores
            string query = string.Format("SELECT * FROM JourneyScoreData WHERE JourneyDataType = '{0}'", JourneyDataType.PlaySession);
            List <JourneyScoreData> all_score_list = dbManager.FindDataByQuery<JourneyScoreData>(query);

            // At last, filter by the given stage
            List<JourneyScoreData> filtered_score_list = all_score_list.FindAll(x => eligiblePlaySessionData_id_list.Contains(x.ElementId));
            return filtered_score_list;
        }

        public List<JourneyScoreData> GetCurrentScoreForPlaySessionsOfLearningBlock(int stage, int learningBlock)
        {
            // First, get all data given a stage
            List<PlaySessionData> eligiblePlaySessionData_list = this.dbManager.FindPlaySessionData(x => x.Stage == stage && x.LearningBlock == learningBlock); // TODO: make this readily available!
            List<string> eligiblePlaySessionData_id_list = eligiblePlaySessionData_list.ConvertAll(x => x.Id);

            // Then, get all scores
            string query = string.Format("SELECT * FROM JourneyScoreData WHERE JourneyDataType = '{0}'", JourneyDataType.PlaySession);
            List<JourneyScoreData> all_score_list = dbManager.FindDataByQuery<JourneyScoreData>(query);

            // At last, filter
            List<JourneyScoreData> filtered_score_list = all_score_list.FindAll(x => eligiblePlaySessionData_id_list.Contains(x.ElementId));
            return filtered_score_list;
        }

        public List<JourneyScoreData> GetCurrentScoreForLearningBlocksOfStage(int stage)
        {
            // First, get all data given a stage
            List<LearningBlockData> eligibleLearningBlockData_list = this.dbManager.FindLearningBlockData(x => x.Stage == stage);
            List<string> eligibleLearningBlockData_id_list = eligibleLearningBlockData_list.ConvertAll(x => x.Id);

            // Then, get all scores
            string query = string.Format("SELECT * FROM JourneyScoreData WHERE JourneyDataType= '{0}'", JourneyDataType.LearningBlock);
            List<JourneyScoreData> all_score_list = dbManager.FindDataByQuery<JourneyScoreData>(query);

            // At last, filter by the given stage
            List<JourneyScoreData> filtered_score_list = all_score_list.FindAll(x => eligibleLearningBlockData_id_list.Contains(x.ElementId));
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
        public float GetAverageScore(List<JourneyScoreData> _scoreList, int numberOfItems = -1)
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
