using UnityEngine;
using System.Collections.Generic;
using EA4S.Core;
using EA4S.MinigamesAPI;
using EA4S.Teacher;

namespace EA4S.MinigamesCommon
{
    /// <summary>
    /// Concrete implementation of the log manager, accessible to minigames.
    /// </summary>
    public class MinigamesLogManager : ILogManager
    {
        #region Runtime variables
        string sessionName;
        MiniGameCode miniGameCode;

        List<ILivingLetterAnswerData> logLearnBuffer = new List<ILivingLetterAnswerData>();
        List<LogAI.PlayResultParameters> logPlayBuffer = new List<LogAI.PlayResultParameters>();
        #endregion

        #region Initialization

        public MinigamesLogManager(MiniGameCode miniGameCode, string sessionName)
        {
            this.miniGameCode = miniGameCode;
            this.sessionName = sessionName;
        }

        #endregion

        #region API

        /// <summary>
        /// To be called to any action of player linked to learnig objective and with positive or negative vote.
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="_isPositiveResult"></param>
        public void OnAnswered(ILivingLetterData _data, bool _isPositiveResult)
        {
            if (AppConstants.DebugLogInserts) Debug.Log("pre-log OnAnswer " + _data.Id + " " + _isPositiveResult);
            ILivingLetterAnswerData newILivingLetterAnswerData = new ILivingLetterAnswerData();
            newILivingLetterAnswerData._data = _data;
            newILivingLetterAnswerData._isPositiveResult = _isPositiveResult;
            BufferizeLogLearnData(newILivingLetterAnswerData);
        }

        /// <summary>
        /// Called when minigame is finished.
        /// </summary>
        /// <param name="_valuation">The valuation.</param>
        public void OnGameEnded(int _valuation)
        {
            FlushLogLearn();
            //FlushLogPlay();   // Unless minigames can directly log play skills, this is not needed
            LogManager.I.LogMinigameScore(sessionName, miniGameCode, _valuation);
       }

        /*
        /// <summary>
        /// Called when player perform a [gameplay skill action] action during gameplay. .
        /// </summary>
        /// <param name="_ability">The ability.</param>
        /// <param name="_score">The score.</param>
        public void OnGameplaySkillAction(PlaySkill _ability, float _score)
        {
            if (AppConstants.DebugLogInserts) Debug.Log("pre-log OnGameplaySkillAction " + _ability + " " + _score);
            BufferizeLogPlayData(new LogAI.PlayResultParameters() {
                playEvent = PlayEvent.Skill,
                skill = _ability,
                score = _score,
            });
        }
        */

        #endregion

        #region Gameplay        
        /// <summary>
        /// Bufferizes the log play data.
        /// @note: deprecated (unless we re-add minigame direct logplay logging)
        /// </summary>
        /// <param name="_playResultParameters">The play result parameters.</param>
        void BufferizeLogPlayData(LogAI.PlayResultParameters _playResultParameters)
        {
            logPlayBuffer.Add(_playResultParameters);
        }
        /// <summary>
        /// Flushes the log play to app teacher log intellingence.
        /// @note: deprecated (unless we re-add minigame direct logplay logging)
        /// /// </summary>
        void FlushLogPlay()
        {
            LogManager.I.LogPlay(sessionName, miniGameCode, logPlayBuffer);
        }
        #endregion

        #region Learn        
        /// <summary>
        /// Bufferizes the log learn data.
        /// </summary>
        /// <param name="_iLivingLetterAnswerData">The i living letter answer data.</param>
        void BufferizeLogLearnData(ILivingLetterAnswerData _iLivingLetterAnswerData)
        {
            logLearnBuffer.Add(_iLivingLetterAnswerData);
        }

        /// <summary>
        /// Flushes the log learn data to app teacher log intellingence.
        /// </summary>
        void FlushLogLearn()
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
                                actualLearnResult.dataType = Database.VocabularyDataType.Letter;
                                break;
                            case LivingLetterDataType.Word:
                                actualLearnResult.dataType = Database.VocabularyDataType.Word;
                                break;
                            case LivingLetterDataType.Image:
                                actualLearnResult.dataType = Database.VocabularyDataType.Word;
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

            /*TEST: add a lot of logs
            for (int i = 0; i < 100; i++)
            {
                var lp = new LogAI.LearnResultParameters();
                lp.dataType = resultsList[0].dataType;
                lp.elementId = resultsList[0].elementId;
                lp.nCorrect = 5;
                lp.nWrong = 2;
                resultsList.Add(lp);
            }*/

            LogManager.I.LogLearn(sessionName, miniGameCode, resultsList);
        }
        #endregion

        #region Journey Scores



        #endregion

        #region Mood
        // direct into API
        #endregion

        #region internal data structures and interfaces
        interface IBufferizableLog
        {
            string CachedType { get; }
        }

        struct ILivingLetterAnswerData : IBufferizableLog
        {
            public string CachedType { get { return "ILivingLetterAnswerData"; } }
            public ILivingLetterData _data;
            public bool _isPositiveResult;
        }
        #endregion
    }
}