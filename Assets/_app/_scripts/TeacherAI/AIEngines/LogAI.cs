using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EA4S.Core;
using EA4S.Database;
using EA4S.Helpers;

namespace EA4S.Teacher
{
    public class LogPlaySessionScoreParams
    {
        public LogPlaySessionScoreParams(JourneyPosition pos, int score, float playTime)
        {
            Pos = pos;
            Score = score;
            PlayTime = playTime;
        }

        public JourneyPosition Pos { get; private set; }
        public int Score { get; private set; }
        public float PlayTime { get; private set; }
    }

    public class LogMiniGameScoreParams
    {
        public LogMiniGameScoreParams(JourneyPosition pos, MiniGameCode miniGameCode, int score, float playTime)
        {
            Pos = pos;
            MiniGameCode = miniGameCode;
            Score = score;
            PlayTime = playTime;
        }

        public JourneyPosition Pos { get; private set; }
        public MiniGameCode MiniGameCode { get; private set; }
        public int Score { get; private set; }
        public float PlayTime { get; private set; }
    }

    /// <summary>
    /// Entry point for logging information on play at runtime, filtered by the Teacher System.
    /// </summary>
    public class LogAI
    {
        // References
        DatabaseManager db;

        public LogAI(DatabaseManager db)
        {
            this.db = db;
        }

        #region Mood

        public void LogMood(string appSession, int mood)
        {
            // refactor: this should have a session like the rest of the logging methods
            float realMood = Mathf.InverseLerp(AppConstants.minimumMoodValue, AppConstants.maximumMoodValue, mood);
            var data = new LogMoodData(appSession, realMood);
            db.Insert(data);
        }

        public int SecondsFromLastMoodLog()
        {
            string query = string.Format("SELECT * FROM " + typeof(LogMoodData).Name);
            var logMoodData = db.FindDataByQuery<LogMoodData>(query).LastOrDefault();
            if (logMoodData != null) Debug.Log(GenericHelper.GetTimeSpanBetween(logMoodData.Timestamp, GenericHelper.GetTimestampForNow()));
            if (logMoodData != null) return (int)GenericHelper.GetTimeSpanBetween(logMoodData.Timestamp, GenericHelper.GetTimestampForNow()).TotalSeconds;
            return int.MaxValue;
        }

        #endregion

        #region Info

        public void LogInfo(string appSession, InfoEvent infoEvent, string parametersString = "")
        {
            if (!AppManager.I.DB.HasLoadedPlayerProfile()) {
                Debug.Log("No player profile DB to log to. Player profile is probably not set");
                return;
            }
            var data = new LogInfoData(appSession, infoEvent, parametersString);
            db.Insert(data);
        }

        #endregion

        #region Play

        /// <summary>
        /// Parameters for the results of a single minigame play session related to a specific skill.
        /// </summary>
        public struct PlayResultParameters
        {
            public PlayEvent playEvent;
            public PlaySkill skill;
            public float score;

            public PlayResultParameters(PlayEvent playEvent, PlaySkill skill, float score)
            {
                this.playEvent = playEvent;
                this.skill = skill;
                this.score = score;
            }
        }

        public void LogPlay(string appSession, JourneyPosition pos, MiniGameCode miniGameCode, List<PlayResultParameters> resultsList)
        {
            // The teacher receives a score for each play skill the minigame deems worthy of analysis
            List<LogPlayData> logDataList = new List<LogPlayData>();
            foreach (var result in resultsList) {
                var data = new LogPlayData(appSession, pos.ToStringId(), miniGameCode, result.playEvent, result.skill, result.score);
                logDataList.Add(data);
            }
            db.InsertAll(logDataList);
        }

        #endregion

        #region Learn

        /// <summary>
        /// General parameters used to define the learning result for each minigame instance
        /// </summary>
        public struct LearnResultParameters
        {
            public VocabularyDataType dataType;
            public string elementId;
            public int nCorrect;
            public int nWrong;
        }

        public void LogLearn(string appSession, JourneyPosition pos, MiniGameCode miniGameCode, List<LearnResultParameters> resultsList)
        {
            var learnRules = GetLearnRules(miniGameCode);

            // Retrieve previous scores
            string query = string.Format("SELECT * FROM " + typeof(VocabularyScoreData).Name);
            List<VocabularyScoreData> previousScoreDataList = db.FindDataByQuery<VocabularyScoreData>(query);

            // Prepare log data
            var logDataList = new List<LogLearnData>();
            var scoreDataList = new List<VocabularyScoreData>();
            foreach (var result in resultsList)
            {
                if (result.elementId == null)
                {
                    Debug.LogError("LogAI: Logging a result with a NULL elementId. Skipped.");
                    continue;
                }
                if (result.nCorrect == 0 && result.nWrong == 0)
                {
                    Debug.LogError("LogAI: Logging a result with no correct nor wrong hits. Skipped.");
                    continue;
                }

                float score = 0f;
                float successRatio = result.nCorrect * 1f / (result.nCorrect + result.nWrong);
                switch (learnRules.voteLogic) {
                    case MiniGameLearnRules.VoteLogic.Threshold:
                        // Uses a binary threshold
                        float threshold = learnRules.logicParameter;
                        score = successRatio > threshold ? 1f : -1f;
                        break;
                    case MiniGameLearnRules.VoteLogic.SuccessRatio:
                        // Uses directly the success ratio to drive the vote
                        score = Mathf.InverseLerp(-1f, 1f, successRatio);
                        break;
                }
                score *= learnRules.minigameImportanceWeight;
                score += learnRules.minigameVoteSkewOffset;

                var logData = new LogLearnData(appSession, pos.ToStringId(), miniGameCode, result.dataType, result.elementId, score);
                logDataList.Add(logData);

                // We also update the score for that data element
                var scoreData = GetVocabularyScoreDataWithMovingAverage(result.dataType, result.elementId, score, previousScoreDataList, ConfigAI.scoreMovingAverageWindow);
                scoreDataList.Add(scoreData);
            }

            db.InsertAll(logDataList);
            db.InsertOrReplaceAll(scoreDataList);
        } 

        // refactor: these rules should be moved out of the LogAI and be instead placed in the games' configuration, as they belong to the games 
        private MiniGameLearnRules GetLearnRules(MiniGameCode code)
        {
            MiniGameLearnRules rules = new MiniGameLearnRules();
            switch (code) {
                //case MiniGameCode.Balloons_letter:  // @todo: set correct ones per each minigame
                //    rules.voteLogic = MiniGameLearnRules.VoteLogic.Threshold;
                //    rules.logicParameter = 0.5f;
                //    break;

                default:
                    rules.voteLogic = MiniGameLearnRules.VoteLogic.SuccessRatio;
                    break;
            }
            return rules;
        }

        #endregion

        #region Journey Scores

        public void LogMiniGameScore(string appSession, JourneyPosition pos, MiniGameCode miniGameCode, int score, float playTime)
        {
            if (AppConstants.VerboseLogging) Debug.Log("LogMiniGameScore " + miniGameCode + " / " + score);

            // Log for history
            var data = new LogMinigameScoreData(appSession, pos, miniGameCode, score, playTime);
            db.Insert(data);

            // Retrieve previous scores
            string query = string.Format("SELECT * FROM " + typeof(MinigameScoreData).Name);
            List<MinigameScoreData> previousScoreDataList = db.FindDataByQuery<MinigameScoreData>(query);

            // Score update
            var scoreData = GetMinigameScoreDataWithMaximum(miniGameCode, playTime, score, previousScoreDataList);
            db.InsertOrReplace(scoreData);

            // We also log play skills related to that minigame, as read from MiniGameData
            var minigameData = db.GetMiniGameDataByCode(miniGameCode);
            List<PlayResultParameters> results = new List<PlayResultParameters>();
            foreach (var weightedPlaySkill in minigameData.AffectedPlaySkills)
            {
                results.Add(new PlayResultParameters(PlayEvent.Skill, weightedPlaySkill.Skill, score));
            }
            LogPlay(appSession, pos, miniGameCode, results);
        }

        public void LogMiniGameScores(string appSession, List<LogMiniGameScoreParams> logMiniGameScoreParams)
        {
            //if (AppConstants.VerboseLogging) Debug.Log("LogMiniGameScore " + logMiniGameScoreParams.MiniGameCode + " / " + logMiniGameScoreParams.Score);

            // Retrieve previous scores
            string query = string.Format("SELECT * FROM " + typeof(MinigameScoreData).Name);
            List<MinigameScoreData> previousScoreDataList = db.FindDataByQuery<MinigameScoreData>(query);

            var logDataList = new List<LogMinigameScoreData>();
            var scoreDataList = new List<MinigameScoreData>();
            foreach (var parameters in logMiniGameScoreParams)
            {
                // Log for history
                var logData = new LogMinigameScoreData(appSession, parameters.Pos, parameters.MiniGameCode, parameters.Score, parameters.PlayTime);
                logDataList.Add(logData);

                // Score update
                var scoreData = GetMinigameScoreDataWithMaximum(parameters.MiniGameCode, parameters.PlayTime, parameters.Score, previousScoreDataList);
                scoreDataList.Add(scoreData);

                // We also log play skills related to that minigame, as read from MiniGameData
                var minigameData = db.GetMiniGameDataByCode(parameters.MiniGameCode);
                List<PlayResultParameters> results = new List<PlayResultParameters>();
                foreach (var weightedPlaySkill in minigameData.AffectedPlaySkills)
                {
                    results.Add(new PlayResultParameters(PlayEvent.Skill, weightedPlaySkill.Skill, parameters.Score));
                }
                LogPlay(appSession, parameters.Pos, parameters.MiniGameCode, results);
            }

            db.InsertAll(logDataList);
            db.InsertOrReplaceAll(scoreDataList);
        }

        public void LogPlaySessionScore(string appSession, JourneyPosition pos, int score, float playTime)
        {
            if (AppConstants.VerboseLogging) Debug.Log("LogPlaySessionScore " + pos.ToStringId() + " / " + score);

            // Log for history
            var data = new LogPlaySessionScoreData(appSession, pos, score, playTime);
            db.Insert(data);

            // Retrieve previous scores
            string query = string.Format("SELECT * FROM " + typeof(JourneyScoreData).Name);
            List<JourneyScoreData> previousScoreDataList = db.FindDataByQuery<JourneyScoreData>(query);

            // Score update
            var scoreData = GetJourneyScoreDataWithMaximum(JourneyDataType.PlaySession, pos.ToStringId(), score, previousScoreDataList);
            db.InsertOrReplace(scoreData);
        }

        public void LogPlaySessionScores(string appSession, List<LogPlaySessionScoreParams> logPlaySessionScoreParamsList)
        {
            // Retrieve previous scores
            string query = string.Format("SELECT * FROM " + typeof(JourneyScoreData).Name);
            List<JourneyScoreData> previousScoreDataList = db.FindDataByQuery<JourneyScoreData>(query);

            var logDataList = new List<LogPlaySessionScoreData>();
            var scoreDataList = new List<JourneyScoreData>();
            foreach (var parameters in logPlaySessionScoreParamsList)
            {
                // Log for history
                var logData = new LogPlaySessionScoreData(appSession, parameters.Pos, parameters.Score, parameters.PlayTime);
                logDataList.Add(logData);

                // Score update
                var scoreData = GetJourneyScoreDataWithMaximum(JourneyDataType.PlaySession, parameters.Pos.ToStringId(), parameters.Score, previousScoreDataList);
                scoreDataList.Add(scoreData);
            }

            db.InsertAll(logDataList);
            db.InsertOrReplaceAll(scoreDataList);
        }

        public void LogLearningBlockScore(int learningBlock, int score)
        {
            throw new System.Exception("Scoring for Learning Block has not been implemented.");
            //if (AppConstants.VerboseLogging) Debug.Log("LogLearningBlockScore " + learningBlock + " / " + score);
            //GetJourneyScoreDataWithMaximum(JourneyDataType.LearningBlock, (learningBlock).ToString(), score);
        }

        #endregion

        #region Score Utilities

        private MinigameScoreData GetMinigameScoreDataWithMaximum(MiniGameCode miniGameCode, float playTime, int newScore, List<MinigameScoreData> scoreDataList)
        {
            int previousMaxScore = 0;
            float previousTotalPlayTime = 0;
            var scoreData = scoreDataList.Find(x => x.MiniGameCode == miniGameCode);
            if (scoreData != null)
            {
                previousMaxScore = scoreData.Score;
                previousTotalPlayTime = scoreData.TotalPlayTime;
            }

            float newTotalPlayTime = previousTotalPlayTime + playTime;
            int newMaxScore = Mathf.Max(previousMaxScore, newScore);
            return new MinigameScoreData(miniGameCode, newMaxScore, newTotalPlayTime);
        }

        private JourneyScoreData GetJourneyScoreDataWithMaximum(JourneyDataType dataType, string elementId, int newScore, List<JourneyScoreData> scoreDataList)
        {
            int previousMaxScore = 0;
            var scoreData = scoreDataList.Find(x => x.ElementId == elementId && x.JourneyDataType == dataType);
            if (scoreData != null)
            {
                previousMaxScore = scoreData.Score;
            }

            int newMaxScore = Mathf.Max(previousMaxScore, newScore);
            return new JourneyScoreData(elementId, dataType, newMaxScore);
        }

        private VocabularyScoreData GetVocabularyScoreDataWithMovingAverage(VocabularyDataType dataType, string elementId, float newScore, List<VocabularyScoreData> scoreDataList, int movingAverageSpan)
        {
            float previousAverageScore = 0;
            var scoreData = scoreDataList.Find(x => x.ElementId == elementId && x.VocabularyDataType == dataType);
            if (scoreData != null) {
                previousAverageScore = scoreData.Score;
            }

            // @note: for the first movingAverageSpan values, this won't be accurate
            float newAverageScore = previousAverageScore - previousAverageScore / movingAverageSpan + newScore / movingAverageSpan;
            return new VocabularyScoreData(elementId, dataType, newAverageScore);
        }

        #endregion

    }

}