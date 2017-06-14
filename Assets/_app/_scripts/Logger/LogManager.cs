using UnityEngine;
using EA4S.Utilities;
using EA4S.Helpers;
using System.Collections.Generic;

namespace EA4S.Core
{
    /// <summary>
    /// App Log Manager. Use this to log any event from app.
    /// </summary>
    public class LogManager
    {
        public static LogManager I;

        int _appSession;
        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public int AppSession {
            get { return _appSession; }
            private set { _appSession = value; }
        }

        public LogManager()
        {
            I = this;
            InitNewSession();
        }

        public void InitNewSession()
        {
            AppSession = GenericHelper.GetTimestampForNow();
            //Random.Range(10000000, 99999999).ToString();
        }

        #region Time Logging

        System.DateTime startPlaySessionDateTime;
        System.DateTime startMiniGameDateTime;
        System.DateTime endPlaySessionDateTime;
        System.DateTime endMiniGameDateTime;

        public void StartPlaySession()
        {
            startPlaySessionDateTime = System.DateTime.Now;
        }

        public void EndPlaySession()
        {
            endPlaySessionDateTime = System.DateTime.Now;
        }

        public void StartMiniGame()
        {
            startMiniGameDateTime = System.DateTime.Now;
        }

        public void EndMiniGame()
        {
            endMiniGameDateTime = System.DateTime.Now;
        }

        #endregion

        #region Proxy From Minigame log manager provider To App Log Intellingence

        protected internal void LogMinigameScore(string playSession, MiniGameCode miniGameCode, int score)
        {
            EndMiniGame();
            LogInfo(InfoEvent.GameEnd, JsonUtility.ToJson(new GameResultInfo() { Game = miniGameCode.ToString(), Result = score.ToString() }));

            float duration = (float)(endMiniGameDateTime - startMiniGameDateTime).TotalSeconds;
            //Debug.LogError("DURATION MG: " + duration);
            EA4S.AppManager.Instance.Teacher.logAI.LogMiniGameScore(AppSession, EA4S.AppManager.Instance.NavigationManager.NavData.CurrentPlayer.CurrentJourneyPosition, miniGameCode, score, duration);
        }

        struct GameResultInfo
        {
            public string Game;
            public string Result;
        }

        /// @note: deprecated (unless we re-add minigame direct logplay logging)
        protected internal void LogPlay(string playSession, MiniGameCode miniGameCode, List<Teacher.LogAI.PlayResultParameters> resultsList)
        {
            (EA4S.AppManager.Instance as EA4S.AppManager).Teacher.logAI.LogPlay(AppSession, (EA4S.AppManager.Instance as EA4S.AppManager).NavigationManager.NavData.CurrentPlayer.CurrentJourneyPosition, miniGameCode, resultsList);
        }

        protected internal void LogLearn(string playSession, MiniGameCode miniGameCode, List<Teacher.LogAI.LearnResultParameters> resultsList)
        {
            (EA4S.AppManager.Instance as EA4S.AppManager).Teacher.logAI.LogLearn(AppSession, (EA4S.AppManager.Instance as EA4S.AppManager).NavigationManager.NavData.CurrentPlayer.CurrentJourneyPosition, miniGameCode, resultsList);
        }

        #endregion

        #region public API        
        /// <summary>
        /// Logs the play session score.
        /// </summary>
        /// <param name="playSessionId">The play session identifier.</param>
        /// <param name="score">The score.</param>
        public void LogPlaySessionScore(string playSessionId, int score)
        {
            EndPlaySession();

            float duration = (float)(endPlaySessionDateTime - startPlaySessionDateTime).TotalSeconds;
            //Debug.LogError("DURATION PS: " + duration);
            (EA4S.AppManager.Instance as EA4S.AppManager).Teacher.logAI.LogPlaySessionScore(AppSession, (EA4S.AppManager.Instance as EA4S.AppManager).NavigationManager.NavData.CurrentPlayer.CurrentJourneyPosition, score, duration);
        }

        /// <summary>
        /// Logs the learning block score.
        /// </summary>
        /// <param name="learningBlock">The learning block.</param>
        /// <param name="score">The score.</param>
        public void LogLearningBlockScore(int learningBlock, int score)
        {
            (EA4S.AppManager.Instance as EA4S.AppManager).Teacher.logAI.LogLearningBlockScore(learningBlock, score);
        }

        /// <summary>
        /// Logs the generic information.
        /// </summary>
        /// <param name="infoEvent">The information event.</param>
        /// <param name="parametersString">The parameters string.</param>
        public void LogInfo(InfoEvent infoEvent, string parametersString = "")
        {
            (EA4S.AppManager.Instance as EA4S.AppManager).Teacher.logAI.LogInfo(AppSession, infoEvent, parametersString);
        }

        /// <summary>
        /// Logs the mood.
        /// </summary>
        /// <param name="mood">The mood.</param>
        public void LogMood(int mood)
        {
            (EA4S.AppManager.Instance as EA4S.AppManager).Teacher.logAI.LogMood(AppSession, mood);
        }

        public void StartApp()
        {
            LogInfo(InfoEvent.AppSessionStart, "{\"AppSession\":\"" + LogManager.I.AppSession + "\"}");
        }
        #endregion
    }
}