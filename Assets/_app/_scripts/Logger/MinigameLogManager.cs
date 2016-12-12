using UnityEngine;
using System.Collections.Generic;
using EA4S.Teacher;
using System;

namespace EA4S
{
    /// <summary>
    /// Concrete implementation of log manager to store log data on db.
    /// Use this to log from minigame.
    /// </summary>
    public class MinigameLogManager : ILogManager
    {

        #region Runtime variables
        public string session { get { return LogManager.I.Session; } }
        string minigameSession;
        MiniGameCode miniGameCode;
        List<ILivingLetterAnswerData> logLearnBuffer = new List<ILivingLetterAnswerData>();
        List<LogAI.PlayResultParameters> logPlayBuffer = new List<LogAI.PlayResultParameters>();
        #endregion

        #region Initializations

        public void InitLogSession()
        {
            //session = DateTime.Now.Ticks.ToString();
        }

        /// <summary>
        /// Initializes the single minigame gameplay log session.
        /// </summary>
        public void InitGameplayLogSession(MiniGameCode _minigameCode)
        {
            if (AppConstants.DebugLogInserts) Debug.Log("InitGameplayLogSession " + _minigameCode.ToString());
            miniGameCode = _minigameCode;
            minigameSession = DateTime.Now.Ticks.ToString();
            LogManager.I.LogInfo(InfoEvent.GameStart, miniGameCode.ToString());
        }

        #endregion

        #region API

        /// <summary>
        /// To be called to any action of player linked to learnig objective and with positive or negative vote.
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="_isPositiveResult"></param>
        public void OnAnswer(ILivingLetterData _data, bool _isPositiveResult)
        {
            if (AppConstants.DebugLogInserts) Debug.Log("pre-log OnAnswer " + _data.Id + " " + _isPositiveResult);
            ILivingLetterAnswerData newILivingLetterAnswerData = new ILivingLetterAnswerData();
            newILivingLetterAnswerData._data = _data;
            newILivingLetterAnswerData._isPositiveResult = _isPositiveResult;
            bufferizeLogLearnData(newILivingLetterAnswerData);
        }

        /// <summary>
        /// Called when minigame is finished.
        /// </summary>
        /// <param name="_valuation">The valuation.</param>
        public void OnMiniGameResult(int _valuation)
        {
            //MinigameResultData newGameplaySessionResultData = new MinigameResultData();
            //newGameplaySessionResultData._valuation = _valuation;
            flushLogLearn();
            flushLogPlay();
            LogManager.I.LogMinigameScore(miniGameCode, _valuation);
            LogManager.I.LogInfo(InfoEvent.GameEnd, JsonUtility.ToJson(new GameResultInfo() { Game = miniGameCode.ToString(), Result = _valuation.ToString() }));
        }

        /// <summary>
        /// Called when player perform a [gameplay skill action] action during gameplay. .
        /// </summary>
        /// <param name="_ability">The ability.</param>
        /// <param name="_score">The score.</param>
        public void OnGameplaySkillAction(PlaySkill _ability, float _score)
        {
            if (AppConstants.DebugLogInserts) Debug.Log("pre-log OnGameplaySkillAction " + _ability + " " + _score);
            bufferizeLogPlayData(new LogAI.PlayResultParameters() {
                playEvent = PlayEvent.Skill,
                skill = _ability,
                score = _score,
            });
        }

        /// <summary>
        /// Log a generic info data.
        /// </summary>
        /// <param name="_event">The event.</param>
        /// <param name="_data">The data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void LogInfoData(InfoEvent _event, string _data = "")
        {
            LogManager.I.LogInfo(_event, _data);

        }

        #endregion

        #region Gameplay        
        /// <summary>
        /// Bufferizes the log play data.
        /// </summary>
        /// <param name="_playResultParameters">The play result parameters.</param>
        void bufferizeLogPlayData(LogAI.PlayResultParameters _playResultParameters)
        {
            logPlayBuffer.Add(_playResultParameters);
        }
        /// <summary>
        /// Flushes the log play to app teacher log intellingence.
        /// </summary>
        void flushLogPlay()
        {
            LogManager.I.LogPlay(minigameSession, miniGameCode, logPlayBuffer);
        }
        #endregion

        #region Learn        
        /// <summary>
        /// Bufferizes the log learn data.
        /// </summary>
        /// <param name="_iLivingLetterAnswerData">The i living letter answer data.</param>
        void bufferizeLogLearnData(ILivingLetterAnswerData _iLivingLetterAnswerData)
        {
            logLearnBuffer.Add(_iLivingLetterAnswerData);
        }

        /// <summary>
        /// Flushes the log learn data to app teacher log intellingence.
        /// </summary>
        void flushLogLearn()
        {
            List<LogAI.LearnResultParameters> resultsList = new List<LogAI.LearnResultParameters>();
            ILivingLetterData actualData = null;
            LogAI.LearnResultParameters actualLearnResult = new LogAI.LearnResultParameters();

            foreach (var l in logLearnBuffer) {
                if (actualData != l._data) {
                    // Is a different learn objective 
                    actualData = l._data;
                    if (actualData != null) {
                        // save actualLearnResult to data log list to send, if exist...
                        resultsList.Add(actualLearnResult);
                        // ...and reset actualLearnResult for new learn objective with new properties
                        actualLearnResult = new LogAI.LearnResultParameters();
                        switch (l._data.DataType) {
                            case LivingLetterDataType.Letter:
                                actualLearnResult.table = Db.DbTables.Letters;
                                break;
                            case LivingLetterDataType.Word:
                                actualLearnResult.table = Db.DbTables.Words;
                                break;
                            case LivingLetterDataType.Image:
                                actualLearnResult.table = Db.DbTables.Words;
                                break;
                            default:
                                // data type not found. Make soft exception.
                                break;
                        }
                        actualLearnResult.elementId = l._data.Id;
                    }
                }
                // update learn objective log...
                if (l._isPositiveResult)
                    actualLearnResult.nCorrect++;
                else
                    actualLearnResult.nWrong++;
            }

            LogManager.I.LogLearn(minigameSession, miniGameCode, resultsList);

        }
        #endregion

        #region Journey Scores



        #endregion

        #region Mood
        // direct into API
        #endregion

        #region internal data structures and interfaces
        public interface iBufferizableLog
        {
            string CachedType { get; }
        }

        public struct ILivingLetterAnswerData : iBufferizableLog
        {
            public string CachedType { get { return "ILivingLetterAnswerData"; } }
            public ILivingLetterData _data;
            public bool _isPositiveResult;
        }

        public struct GameResultInfo
        {
            public string Game;
            public string Result;
        }
        #endregion
    }
}