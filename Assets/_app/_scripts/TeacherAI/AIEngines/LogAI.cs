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
        public static bool UNLOCK_AT_PLAYSESSION_END = true;

        // References
        DatabaseManager db;

        public LogAI(DatabaseManager db)
        {
            this.db = db;
        }

        #region Mood

        public void LogMood(int appSession, int mood)
        {
            // refactor: this should have a session like the rest of the logging methods
            float realMood = Mathf.InverseLerp(AppConstants.minimumMoodValue, AppConstants.maximumMoodValue, mood);
            var data = new LogMoodData(appSession, realMood);
            db.Insert(data);
        }

        public int SecondsFromLastMoodLog()
        {
            string query = string.Format("SELECT * FROM " + typeof(LogMoodData).Name);
            var logMoodData = db.Query<LogMoodData>(query).LastOrDefault();
            //if (logMoodData != null) Debug.Log(GenericHelper.GetTimeSpanBetween(logMoodData.Timestamp, GenericHelper.GetTimestampForNow()));
            if (logMoodData != null) return (int)GenericHelper.GetTimeSpanBetween(logMoodData.Timestamp, GenericHelper.GetTimestampForNow()).TotalSeconds;
            return int.MaxValue;
        }

        #endregion

        #region Info

        public void LogInfo(int appSession, InfoEvent infoEvent, string parametersString = "")
        {
            if (!AppManager.Instance.DB.HasLoadedPlayerProfile()) {
                Debug.Log("No player profile DB to log to. Player profile is probably not set");
                return;
            }
            var data = new LogInfoData(appSession, infoEvent, AppManager.Instance.NavigationManager.GetCurrentScene(), parametersString);
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

        public void LogPlay(int appSession, JourneyPosition pos, MiniGameCode miniGameCode, List<PlayResultParameters> resultsList)
        {
            // The teacher receives a score for each play skill the minigame deems worthy of analysis
            List<LogPlayData> logDataList = new List<LogPlayData>();
            foreach (var result in resultsList) {
                var data = new LogPlayData(appSession, pos, miniGameCode, result.playEvent, result.skill, result.score);
                logDataList.Add(data);
            }
            db.InsertAll(logDataList);
        }

        #endregion

        #region Learn

        /// <summary>
        /// General parameters used to define the learning result for each minigame instance
        /// </summary>
        public class LearnResultParameters
        {
            public VocabularyDataType dataType;
            public string elementId;
            public int nCorrect;
            public int nWrong;

            public override string ToString()
            {
                return dataType + " - " + elementId + " " + nCorrect + "/" + nWrong;
            }
        }

        public void UnlockVocabularyDataForJourneyPosition(JourneyPosition pos)
        {
            if (!UNLOCK_AT_PLAYSESSION_END) return;

            string query = string.Format("SELECT * FROM " + typeof(VocabularyScoreData).Name);
            List<VocabularyScoreData> scoreDataList = db.Query<VocabularyScoreData>(query);

            var currentPSContents = AppManager.Instance.Teacher.VocabularyAi.GetContentsAtJourneyPosition(pos);
            var letters = currentPSContents.GetHashSet<LetterData>();
            var words = currentPSContents.GetHashSet<WordData>();
            var phrases = currentPSContents.GetHashSet<PhraseData>();

            foreach (var d in letters) {
                var scoreData = scoreDataList.Find(x => x.ElementId == d.Id && x.VocabularyDataType == VocabularyDataType.Letter);
                if (scoreData == null) {
                    scoreData = new VocabularyScoreData(d.Id, VocabularyDataType.Letter, 0, false);
                    scoreDataList.Add(scoreData);
                }
                scoreData.Unlocked = true;
            }

            foreach (var d in words)
            {
                var scoreData = scoreDataList.Find(x => x.ElementId == d.Id && x.VocabularyDataType == VocabularyDataType.Word);
                if (scoreData == null) {
                    scoreData = new VocabularyScoreData(d.Id, VocabularyDataType.Word, 0, false);
                    scoreDataList.Add(scoreData);
                }
                scoreData.Unlocked = true;
            }

            foreach (var d in phrases)
            {
                var scoreData = scoreDataList.Find(x => x.ElementId == d.Id && x.VocabularyDataType == VocabularyDataType.Phrase);
                if (scoreData == null) {
                    scoreData = new VocabularyScoreData(d.Id, VocabularyDataType.Phrase, 0, false);
                    scoreDataList.Add(scoreData);
                }
                scoreData.Unlocked = true;
            }

            db.InsertOrReplaceAll(scoreDataList);
        }

        public void LogLearn(int appSession, JourneyPosition pos, MiniGameCode miniGameCode, List<LearnResultParameters> resultsList)
        {
            var currentJourneyContents = AppManager.Instance.Teacher.VocabularyAi.CurrentJourneyContents;
            if (currentJourneyContents == null) return; // No logging if we do not have contents (for example through a direct Play)

            var learnRules = GetLearnRules(miniGameCode);

            // Retrieve previous scores
            string query = string.Format("SELECT * FROM " + typeof(VocabularyScoreData).Name);
            List<VocabularyScoreData> previousScoreDataList = db.Query<VocabularyScoreData>(query);

            // Prepare log data
            var logDataList = new List<LogVocabularyScoreData>();
            var scoreDataList = new List<VocabularyScoreData>();
            foreach (var result in resultsList) {
                if (result.elementId == null) {
                    Debug.LogError("LogAI: Logging a result with a NULL elementId. Skipped.");
                    continue;
                }
                if (result.nCorrect == 0 && result.nWrong == 0) {
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

                var logData = new LogVocabularyScoreData(appSession, pos, miniGameCode, result.dataType, result.elementId, score);
                logDataList.Add(logData);

                // We also update the score for that data element
                var scoreData = GetVocabularyScoreDataWithMovingAverage(result.dataType, result.elementId, score, previousScoreDataList, ConfigAI.scoreMovingAverageWindow);
                scoreDataList.Add(scoreData);

                // Check whether the vocabulary data was in the journey (and can thus be unlocked)
                if (!UNLOCK_AT_PLAYSESSION_END)
                {
                    if (!scoreData.Unlocked)
                    {
                        IVocabularyData data = null;
                        bool containedInJourney = false;
                        switch (result.dataType)
                        {
                            case VocabularyDataType.Letter:
                                data = AppManager.Instance.DB.GetLetterDataById(result.elementId);
                                containedInJourney = currentJourneyContents.Contains(data as LetterData);
                                break;
                            case VocabularyDataType.Word:
                                data = AppManager.Instance.DB.GetWordDataById(result.elementId);
                                containedInJourney = currentJourneyContents.Contains(data as WordData);
                                break;
                            case VocabularyDataType.Phrase:
                                data = AppManager.Instance.DB.GetPhraseDataById(result.elementId);
                                containedInJourney = currentJourneyContents.Contains(data as PhraseData);
                                break;
                        }

                        if (containedInJourney)
                        {
                            scoreData.Unlocked = true;
                        }
                    }
                }
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

        public void LogMiniGameScore(int appSession, JourneyPosition pos, MiniGameCode miniGameCode, int score, float playTime)
        {
            if (AppConstants.VerboseLogging) Debug.Log("LogMiniGameScore " + miniGameCode + " / " + score);

            // Log for history
            var data = new LogMiniGameScoreData(appSession, pos, miniGameCode, score, playTime);
            db.Insert(data);

            // Retrieve previous scores
            string query = string.Format("SELECT * FROM " + typeof(MiniGameScoreData).Name);
            List<MiniGameScoreData> previousScoreDataList = db.Query<MiniGameScoreData>(query);

            // Score update
            var scoreData = GetMinigameScoreDataWithMaximum(miniGameCode, playTime, score, previousScoreDataList);
            db.InsertOrReplace(scoreData);

            // We also log play skills related to that minigame, as read from MiniGameData
            var minigameData = db.GetMiniGameDataByCode(miniGameCode);
            List<PlayResultParameters> results = new List<PlayResultParameters>();
            float normalizedScore = Mathf.InverseLerp(AppConstants.minimumMinigameScore, AppConstants.maximumMinigameScore, score);
            foreach (var weightedPlaySkill in minigameData.AffectedPlaySkills) {
                results.Add(new PlayResultParameters(PlayEvent.Skill, weightedPlaySkill.Skill, normalizedScore));
            }
            LogPlay(appSession, pos, miniGameCode, results);
        }

        public void LogMiniGameScores(int appSession, List<LogMiniGameScoreParams> logMiniGameScoreParams)
        {
            //if (AppConstants.VerboseLogging) Debug.Log("LogMiniGameScore " + logMiniGameScoreParams.MiniGameCode + " / " + logMiniGameScoreParams.Score);

            // Retrieve previous scores
            string query = string.Format("SELECT * FROM " + typeof(MiniGameScoreData).Name);
            List<MiniGameScoreData> previousScoreDataList = db.Query<MiniGameScoreData>(query);

            var logDataList = new List<LogMiniGameScoreData>();
            var scoreDataList = new List<MiniGameScoreData>();
            foreach (var parameters in logMiniGameScoreParams) {
                // Log for history
                var logData = new LogMiniGameScoreData(appSession, parameters.Pos, parameters.MiniGameCode, parameters.Score, parameters.PlayTime);
                logDataList.Add(logData);

                // Score update
                var scoreData = GetMinigameScoreDataWithMaximum(parameters.MiniGameCode, parameters.PlayTime, parameters.Score, previousScoreDataList);
                scoreDataList.Add(scoreData);

                // We also log play skills related to that minigame, as read from MiniGameData
                var minigameData = db.GetMiniGameDataByCode(parameters.MiniGameCode);
                List<PlayResultParameters> results = new List<PlayResultParameters>();
                foreach (var weightedPlaySkill in minigameData.AffectedPlaySkills) {
                    results.Add(new PlayResultParameters(PlayEvent.Skill, weightedPlaySkill.Skill, parameters.Score));
                }
                LogPlay(appSession, parameters.Pos, parameters.MiniGameCode, results);
            }

            db.InsertAll(logDataList);
            db.InsertOrReplaceAll(scoreDataList);
        }

        public void LogPlaySessionScore(int appSession, JourneyPosition pos, int score, float playTime)
        {
            if (AppConstants.VerboseLogging) Debug.Log("LogPlaySessionScore " + pos.ToStringId() + " / " + score);

            // Log for history
            var data = new LogPlaySessionScoreData(appSession, pos, score, playTime);
            db.Insert(data);

            // Retrieve previous scores
            string query = string.Format("SELECT * FROM " + typeof(JourneyScoreData).Name);
            List<JourneyScoreData> previousScoreDataList = db.Query<JourneyScoreData>(query);

            // Score update
            var scoreData = GetJourneyScoreDataWithMaximum(JourneyDataType.PlaySession, pos.ToStringId(), score, previousScoreDataList);
            db.InsertOrReplace(scoreData);
        }

        public void LogPlaySessionScores(int appSession, List<LogPlaySessionScoreParams> logPlaySessionScoreParamsList)
        {
            // Retrieve previous scores
            string query = string.Format("SELECT * FROM " + typeof(JourneyScoreData).Name);
            List<JourneyScoreData> previousScoreDataList = db.Query<JourneyScoreData>(query);

            var logDataList = new List<LogPlaySessionScoreData>();
            var scoreDataList = new List<JourneyScoreData>();
            foreach (var parameters in logPlaySessionScoreParamsList) {
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

        private MiniGameScoreData GetMinigameScoreDataWithMaximum(MiniGameCode miniGameCode, float playTime, int newStars, List<MiniGameScoreData> scoreDataList)
        {
            int previousMaxStars = 0;
            float previousTotalPlayTime = 0;
            var scoreData = scoreDataList.Find(x => x.MiniGameCode == miniGameCode);
            if (scoreData != null) {
                previousMaxStars = scoreData.Stars;
                previousTotalPlayTime = scoreData.TotalPlayTime;
            }

            float newTotalPlayTime = previousTotalPlayTime + playTime;
            int newMaxStars = Mathf.Max(previousMaxStars, newStars);
            return new MiniGameScoreData(miniGameCode, newMaxStars, newTotalPlayTime);
        }

        private JourneyScoreData GetJourneyScoreDataWithMaximum(JourneyDataType dataType, string elementId, int newStars, List<JourneyScoreData> scoreDataList)
        {
            int previousMaxStars = 0;
            var scoreData = scoreDataList.Find(x => x.ElementId == elementId && x.JourneyDataType == dataType);
            if (scoreData != null) {
                previousMaxStars = scoreData.Stars;
            }

            int newMaxStars = Mathf.Max(previousMaxStars, newStars);
            return new JourneyScoreData(elementId, dataType, newMaxStars);
        }

        private VocabularyScoreData GetVocabularyScoreDataWithMovingAverage(VocabularyDataType dataType, string elementId, float newScore, List<VocabularyScoreData> scoreDataList, int movingAverageSpan)
        {
            float previousAverageScore = 0;
            bool previousUnlocked = false;
            var scoreData = scoreDataList.Find(x => x.ElementId == elementId && x.VocabularyDataType == dataType);
            if (scoreData != null) {
                previousAverageScore = scoreData.Score;
                previousUnlocked = scoreData.Unlocked;
            }

            // @note: for the first movingAverageSpan values, this won't be accurate
            float newAverageScore = previousAverageScore - previousAverageScore / movingAverageSpan + newScore / movingAverageSpan;
            return new VocabularyScoreData(elementId, dataType, newAverageScore, previousUnlocked);
        }

        #endregion

    }

}