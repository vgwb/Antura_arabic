using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EA4S.Core;
using EA4S.Database;
using EA4S.Helpers;

namespace EA4S.Teacher
{
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

        public void LogMood(int mood)
        {
            // refactor: this should have a session like the rest of the logging methods
            float realMood = Mathf.InverseLerp(AppConstants.minimumMoodValue, AppConstants.maximumMoodValue, mood);
            var data = new LogMoodData(realMood);
            db.Insert(data);
        }

        public int SecondsFromLastMoodLog()
        {
            string query = string.Format("SELECT * FROM " + typeof(LogMoodData).Name);
            var logMoodData = db.FindDataByQuery<LogMoodData>(query).LastOrDefault();
            Debug.Log(GenericHelper.GetTimeSpanBetween(logMoodData.Timestamp, GenericHelper.GetTimestampForNow()));
            if (logMoodData != null) return (int)GenericHelper.GetTimeSpanBetween(logMoodData.Timestamp, GenericHelper.GetTimestampForNow()).TotalSeconds;
            return int.MaxValue;
        }

        #endregion

        #region Info

        public void LogInfo(string session, InfoEvent infoEvent, string parametersString = "")
        {
            if (!AppManager.I.DB.HasLoadedPlayerProfile()) {
                Debug.Log("No player profile DB to log to. Player profile is probably not set");
                return;
            }
            var data = new LogInfoData(session, infoEvent, parametersString);
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

        public void LogPlay(string session, string playSession, MiniGameCode miniGameCode, List<PlayResultParameters> resultsList)
        {
            // The teacher receives a score for each play skill the minigame deems worthy of analysis
            foreach (var result in resultsList) {
                var data = new LogPlayData(session, playSession, miniGameCode, result.playEvent, result.skill, result.score);
                db.Insert(data);
            }
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

        public void LogLearn(string session, string playSession, MiniGameCode miniGameCode, List<LearnResultParameters> resultsList)
        {
            var learnRules = GetLearnRules(miniGameCode);

            foreach (var result in resultsList) {
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

                var data = new LogLearnData(session, playSession, miniGameCode, result.dataType, result.elementId, score);
                db.Insert(data);

                // We also update the score for that data element
                // refactor: the magic number 5 should become a configuration parameter
                UpdateVocabularyScoreDataWithMovingAverage(result.dataType, result.elementId, score, 5);
            }
        } 

        // refactor: these rules should be moved out of the LogAI and be instead placed in the games' configuration, as they belong to the games 
        private MiniGameLearnRules GetLearnRules(MiniGameCode code)
        {
            MiniGameLearnRules rules = new MiniGameLearnRules();
            switch (code) {
                case MiniGameCode.Balloons_letter:  // @todo: set correct ones per each minigame
                    rules.voteLogic = MiniGameLearnRules.VoteLogic.Threshold;
                    rules.logicParameter = 0.5f;
                    break;

                default:
                    rules.voteLogic = MiniGameLearnRules.VoteLogic.SuccessRatio;
                    break;
            }
            return rules;
        }

        #endregion

        #region Journey Scores

        public void LogMiniGameScore(string session, string playSession, MiniGameCode miniGameCode, float totalPlayTime, int score)
        {
            if (AppConstants.VerboseLogging) Debug.Log("LogMiniGameScore " + miniGameCode + " / " + score);
            UpdateMinigameScoreDataWithMaximum(miniGameCode.ToString(), totalPlayTime, score);

            // We also log play skills related to that minigame, as read from MiniGameData
            var minigameData = db.GetMiniGameDataByCode(miniGameCode);
            List<PlayResultParameters> results = new List<PlayResultParameters>();
            foreach (var playSkill in minigameData.AffectedPlaySkills)
            {
                results.Add(new PlayResultParameters(PlayEvent.Skill, playSkill, score));
            }
            LogPlay(session, playSession, miniGameCode, results);
        }

        public void LogPlaySessionScore(string playSessionId, int score)
        {
            if (AppConstants.VerboseLogging) Debug.Log("LogPlaySessionScore " + playSessionId + " / " + score);
            UpdateJourneyScoreDataWithMaximum(JourneyDataType.PlaySession,(playSessionId).ToString(), score);
        }

        public void LogLearningBlockScore(int learningBlock, int score)
        {
            if (AppConstants.VerboseLogging) Debug.Log("LogLearningBlockScore " + learningBlock + " / " + score);
            UpdateJourneyScoreDataWithMaximum(JourneyDataType.LearningBlock, (learningBlock).ToString(), score);
        }

        #endregion

        #region Score Utilities

        private void UpdateMinigameScoreDataWithMaximum(string elementId, float playTime, int newScore)
        {
            string query = string.Format("SELECT * FROM " + typeof(MinigameScoreData).Name + " WHERE ElementId = '{0}'", elementId);
            var scoreDataList = db.FindDataByQuery<MinigameScoreData>(query);
            int previousMaxScore = 0;
            float previousTotalPlayTime = 0;
            if (scoreDataList.Count > 0)
            {
                previousMaxScore = scoreDataList[0].Score;
                previousTotalPlayTime = scoreDataList[0].TotalPlayTime;
            }
            float newTotalPlayTime = previousTotalPlayTime + playTime;
            int newMaxScore = Mathf.Max(previousMaxScore, newScore);
            db.UpdateMinigameScoreData(elementId, newTotalPlayTime, newMaxScore);
        }

        private void UpdateJourneyScoreDataWithMaximum(JourneyDataType dataType, string elementId, int newScore)
        {
            string query = string.Format("SELECT * FROM " + typeof(JourneyScoreData).Name + " WHERE JourneyDataType = '{0}' AND ElementId = '{1}'", (int)dataType, elementId);
            List<JourneyScoreData> scoreDataList = db.FindDataByQuery<JourneyScoreData>(query);
            int previousMaxScore = 0;
            if (scoreDataList.Count > 0) {
                previousMaxScore = scoreDataList[0].Score;
            }
            int newMaxScore = Mathf.Max(previousMaxScore, newScore);
            db.UpdateJourneyScoreData(dataType, elementId, newMaxScore);
        }

        private void UpdateVocabularyScoreDataWithMovingAverage(VocabularyDataType dataType, string elementId, float newScore, int movingAverageSpan)
        {
            string query = string.Format("SELECT * FROM " + typeof(VocabularyScoreData).Name +  " WHERE VocabularyDataType = '{0}' AND ElementId = '{1}'", (int)dataType, elementId);
            List<VocabularyScoreData> scoreDataList = db.FindDataByQuery<VocabularyScoreData>(query);
            float previousAverageScore = 0;
            if (scoreDataList.Count > 0) {
                previousAverageScore = scoreDataList[0].Score;
            }
            // @note: for the first movingAverageSpan values, this won't be accurate
            float newAverageScore = previousAverageScore - previousAverageScore / movingAverageSpan + newScore / movingAverageSpan;
            db.UpdateVocabularyScoreData(dataType, elementId, newAverageScore);
        }

        #endregion

    }

}