using UnityEngine;
using System.Collections.Generic;

namespace EA4S.Core
{
    /// <summary>
    /// App Log Manager. Use this to log any event from app.
    /// </summary>
    public class LogManager
    {
        public static LogManager I;

        private string _session = "";
        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public string AppSession {
            get { return _session; }
            private set { _session = value; }
        }

        public LogManager()
        {

            I = this;
        }

        public void InitNewSession()
        {
            // refactor: should be a sequential number
            AppSession = Random.Range(10000000, 99999999).ToString();
        }

        #region Proxy From Minigame log manager provider To App Log Intellingence

        protected internal void LogMinigameScore(string playSession, MiniGameCode miniGameCode, int score)
        {
            // @todo: the minigame play time should be passed here
            AppManager.I.Teacher.logAI.LogMiniGameScore(AppSession, AppManager.I.NavigationManager.NavData.CurrentPlayer.CurrentJourneyPosition, miniGameCode, score, 0f);
        }

        /// @note: deprecated (unless we re-add minigame direct logplay logging)
        protected internal void LogPlay(string playSession, MiniGameCode miniGameCode, List<Teacher.LogAI.PlayResultParameters> resultsList)
        {
            AppManager.I.Teacher.logAI.LogPlay(AppSession, AppManager.I.NavigationManager.NavData.CurrentPlayer.CurrentJourneyPosition, miniGameCode, resultsList);
        }

        protected internal void LogLearn(string playSession, MiniGameCode miniGameCode, List<Teacher.LogAI.LearnResultParameters> resultsList)
        {
            AppManager.I.Teacher.logAI.LogLearn(AppSession, AppManager.I.NavigationManager.NavData.CurrentPlayer.CurrentJourneyPosition, miniGameCode, resultsList);
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
            // @todo: the complete play session play time should be passed here
            AppManager.I.Teacher.logAI.LogPlaySessionScore(AppSession, AppManager.I.NavigationManager.NavData.CurrentPlayer.CurrentJourneyPosition, score, 0f);
        }

        /// <summary>
        /// Logs the learning block score.
        /// </summary>
        /// <param name="learningBlock">The learning block.</param>
        /// <param name="score">The score.</param>
        public void LogLearningBlockScore(int learningBlock, int score)
        {
            AppManager.I.Teacher.logAI.LogLearningBlockScore(learningBlock, score);
        }

        /// <summary>
        /// Logs the generic information.
        /// </summary>
        /// <param name="infoEvent">The information event.</param>
        /// <param name="parametersString">The parameters string.</param>
        public void LogInfo(InfoEvent infoEvent, string parametersString = "")
        {
            AppManager.I.Teacher.logAI.LogInfo(AppSession, infoEvent, parametersString);
        }

        /// <summary>
        /// Logs the mood.
        /// </summary>
        /// <param name="mood">The mood.</param>
        public void LogMood(int mood)
        {
            AppManager.I.Teacher.logAI.LogMood(AppSession, mood);
        }

        public void StartApp()
        {
            LogInfo(InfoEvent.AppStarted);
        }
        #endregion
    }
}