using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
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
        public string Session {
            get { return _session; }
            private set { _session = value; }
        }

        /// <summary>
        /// The minigame log manager concrete instance.
        /// </summary>
        public ILogManager MinigameLogManager = new MinigameLogManager();

        public LogManager()
        {

            I = this;
        }

        public void InitNewSession()
        {
            Session = Random.Range(10000000, 99999999).ToString();
        }

        #region Proxy From Minigame log manager provider To App Log Intellingence

        protected internal void LogMinigameScore(MiniGameCode miniGameCode, float score)
        {
            AppManager.I.Teacher.logAI.LogMiniGameScore(miniGameCode, score);
        }

        protected internal void LogPlay(string playSession, MiniGameCode miniGameCode, List<Teacher.LogAI.PlayResultParameters> resultsList)
        {
            AppManager.I.Teacher.logAI.LogPlay(Session, playSession, miniGameCode, resultsList);
        }

        protected internal void LogLearn(string playSession, MiniGameCode miniGameCode, List<Teacher.LogAI.LearnResultParameters> resultsList)
        {
            AppManager.I.Teacher.logAI.LogLearn(Session, playSession, miniGameCode, resultsList);
        }

        #endregion

        #region public API        
        /// <summary>
        /// Logs the play session score.
        /// </summary>
        /// <param name="playSessionId">The play session identifier.</param>
        /// <param name="score">The score.</param>
        public void LogPlaySessionScore(string playSessionId, float score)
        {
            AppManager.I.Teacher.logAI.LogPlaySessionScore(playSessionId, score);
        }

        /// <summary>
        /// Logs the learning block score.
        /// </summary>
        /// <param name="learningBlock">The learning block.</param>
        /// <param name="score">The score.</param>
        public void LogLearningBlockScore(int learningBlock, float score)
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
            AppManager.I.Teacher.logAI.LogInfo(Session, infoEvent, parametersString);
        }

        /// <summary>
        /// Logs the mood.
        /// </summary>
        /// <param name="mood">The mood.</param>
        public void LogMood(int mood)
        {
            AppManager.I.Teacher.logAI.LogMood(mood);
        }

        public void StartApp()
        {
            LogInfo(InfoEvent.AppStarted);
        }
        #endregion
    }
}