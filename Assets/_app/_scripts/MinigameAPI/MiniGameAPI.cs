using UnityEngine;
using System.Collections.Generic;
using ModularFramework.Core;
using EA4S;
using System.Linq;
using System;
using EA4S.Db;

namespace EA4S.API
{

    /// <summary>
    /// TODO: this API implementation will be replaced by dependency injection pattern implementation.
    /// </summary>
    /// <seealso cref="ModularFramework.Core.Singleton{EA4S.API.MiniGameAPI}" />
    public class MiniGameAPI : Singleton<MiniGameAPI>
    {

        #region Learning course 

        #endregion

        #region Teacher AI
        /// TODO:
        /// - Game To Show in the wheel
        /// - Is it time for an Assessment?
        /// - Is it time for an extra (outbound) activity?


        /// <summary>
        /// Return all information needed from minigame to setup gameplay session.
        /// </summary>
        /// <returns></returns>
        public PlaySessionInfo GetPlaySessionInfo()
        {
            PlaySessionInfo playSessionInfo = new PlaySessionInfo();
            // TODO
            return playSessionInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_result"></param>
        public void PushPlaySessionResult(PlaySessionResult _result)
        {
            // TODO
        }

        /// <summary>
        /// Get corresponding arabic letter with _letterId param.
        /// </summary>
        /// <param name="_letterId"></param>
        /// <returns></returns>
        public string GetArabicLetter(string _letterId)
        {
            // TODO
            return "";
        }

        /// <summary>
        /// Return List of Letter Data valid for this gameplay and actual profile data.
        /// </summary>
        /// <returns></returns>
        public List<LetterData> GetValidLetters(int _amount = -1)
        {
            // TODO
            return new List<LetterData>();
        }

        /// <summary>
        /// Return List of Word Data valid for this gameplay and actual profile data.
        /// </summary>
        /// <param name="_amount"></param>
        /// <returns></returns>
        public List<WordData> GetValidWorlds(int _amount = -1)
        {
            // TODO
            return new List<WordData>();
        }

        #endregion

        #region Log System

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_area"></param>
        /// <param name="_context"></param>
        /// <param name="_action"></param>
        /// <param name="_data"></param>
        public void Log(string _area, string _context, string _action, string _data)
        {
            LoggerEA4S.Log(_area, _context, _action, _data);
        }

        #endregion

        #region Player profile

        #endregion

        #region Audio

        #endregion

        #region Localization Data
        /// TODO: 
        /// - Create localization interface for data structure for single language.
        #endregion

        #region AssetManager
        /// TODO:
        /// - Living letters base
        /// -- base init by ILivingLetterData
        /// -- animation controller
        /// - Input Controller
        /// - AI controller

        /// <summary>
        /// Return asset with specific generic asset Id and theme.
        /// </summary>
        /// <param name="_prefId"></param>
        /// <param name="_themeId"></param>
        /// <returns></returns>
        public GameObject GetAsset(string _prefId, int _themeId)
        {
            // TODO
            return null;
        }

        #endregion

        #region Gameplay Management

        public void StartGame(MiniGameCode _gameCode, List<IQuestionPack> _gameData, GameConfiguration _gameConfiguration)
        {
            MiniGameData miniGameData = AppManager.Instance.DB.GetMiniGameDataByCode(_gameCode);
            string miniGameScene = miniGameData.Scene;

            switch (_gameCode) {
                case MiniGameCode.Assessment_Letters:
                case MiniGameCode.Assessment_LettersMatchShape:
                    break;
                case MiniGameCode.AlphabetSong:
                    break;
                case MiniGameCode.Balloons_counting:
                    break;
                case MiniGameCode.Balloons_letter:
                    break;
                case MiniGameCode.Balloons_spelling:
                    break;
                case MiniGameCode.Balloons_words:
                    break;
                case MiniGameCode.ColorTickle:
                    break;
                case MiniGameCode.DancingDots:
                    break;
                case MiniGameCode.DontWakeUp:
                    break;
                case MiniGameCode.Egg:
                    //Egg.EggConfiguration.Instance.Difficulty = _gameConfiguration.Difficulty;
                    //Egg.EggConfiguration.Instance.QuestionProvider = new FindRightLetterQuestionProvider(_gameData, miniGameData.Description);
                    //Egg.EggConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    break;
                case MiniGameCode.FastCrowd_alphabet:
                    FastCrowd.FastCrowdConfiguration.Instance.Difficulty = _gameConfiguration.Difficulty;
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Alphabet;
                    FastCrowd.FastCrowdConfiguration.Instance.Questions = new FindRightLetterQuestionProvider(_gameData, miniGameData.Description);
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    // Scene name override to test new version
                    miniGameScene = miniGameData.Scene + "_new";
                    break;
                case MiniGameCode.FastCrowd_counting:
                    FastCrowd.FastCrowdConfiguration.Instance.Difficulty = _gameConfiguration.Difficulty;
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Counting;
                    FastCrowd.FastCrowdConfiguration.Instance.Questions = new FindRightLetterQuestionProvider(_gameData, miniGameData.Description);
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    // Scene name override to test new version
                    miniGameScene = miniGameData.Scene + "_new";
                    break;
                case MiniGameCode.FastCrowd_letter:
                    FastCrowd.FastCrowdConfiguration.Instance.Difficulty = _gameConfiguration.Difficulty;
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Letter;
                    FastCrowd.FastCrowdConfiguration.Instance.Questions = new FindRightLetterQuestionProvider(_gameData, miniGameData.Description);
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    // Scene name override to test new version
                    miniGameScene = miniGameData.Scene + "_new";
                    break;
                case MiniGameCode.FastCrowd_spelling:
                    FastCrowd.FastCrowdConfiguration.Instance.Difficulty = _gameConfiguration.Difficulty;
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Spelling;
                    FastCrowd.FastCrowdConfiguration.Instance.Questions = new FindRightLetterQuestionProvider(_gameData, miniGameData.Description);
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    // Scene name override to test new version
                    miniGameScene = miniGameData.Scene + "_new";
                    break;
                case MiniGameCode.FastCrowd_words:
                    FastCrowd.FastCrowdConfiguration.Instance.Difficulty = _gameConfiguration.Difficulty;
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Words;
                    FastCrowd.FastCrowdConfiguration.Instance.Questions = new FindRightLetterQuestionProvider(_gameData, miniGameData.Description);
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    // Scene name override to test new version
                    miniGameScene = miniGameData.Scene + "_new";
                    break;
                case MiniGameCode.HiddenSource:
                    break;
                case MiniGameCode.HideSeek:
                    break;
                case MiniGameCode.MakeFriends:
                    break;
                case MiniGameCode.Maze:
                    break;
                case MiniGameCode.MissingLetter:
                    break;
                case MiniGameCode.MissingLetter_phrases:
                    break;
                case MiniGameCode.MixedLetters_alphabet:
                    break;
                case MiniGameCode.MixedLetters_spelling:
                    break;
                case MiniGameCode.SickLetter:
                    break;
                case MiniGameCode.ReadingGame:
                    break;
                case MiniGameCode.Scanner:
                    break;
                case MiniGameCode.Scanner_phrase:
                    break;
                case MiniGameCode.ThrowBalls_letters:
                    break;
                case MiniGameCode.ThrowBalls_words:
                    break;
                case MiniGameCode.Tobogan_letters:
                    Tobogan.ToboganConfiguration.Instance.Difficulty = _gameConfiguration.Difficulty;
                    Tobogan.ToboganConfiguration.Instance.Variation = Tobogan.ToboganVariation.LetterInAWord;
                    Tobogan.ToboganConfiguration.Instance.PipeQuestions = new FindRightLetterQuestionProvider(_gameData, miniGameData.Description);
                    Tobogan.ToboganConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    break;
                case MiniGameCode.Tobogan_words:
                    Tobogan.ToboganConfiguration.Instance.Difficulty = _gameConfiguration.Difficulty;
                    Tobogan.ToboganConfiguration.Instance.Variation = Tobogan.ToboganVariation.SunMoon;
                    Tobogan.ToboganConfiguration.Instance.PipeQuestions = new FindRightLetterQuestionProvider(_gameData, miniGameData.Description);
                    Tobogan.ToboganConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    break;
                default:
                    Debug.LogWarningFormat("Minigame selected {0} not found.", _gameCode.ToString());
                    break;
            }

            // Call game start
            AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition(miniGameScene);
        }



        #endregion

    }

    #region Data Structures

    /// <summary>
    /// Information needed from minigame to setup gameplay session.
    /// </summary>
    public class PlaySessionInfo
    {

        /// <summary>
        /// List of learning objectives to use in the game (letters, words, etc).
        /// </summary>
        public List<ILivingLetterData> LivingLetterData;

        /// TODO:
        /// - AbilityType1, AbilityType2, etc...
        /// - level
        /// - difficulty
        /// - parameters

    }


    /// <summary>
    /// Information used to comunicate result of minigame gameplay session.
    /// </summary>
    public class PlaySessionResult
    {
        /// <summary>
        /// Number of stars based on playsession result.
        /// 0 = negative result.
        /// 1, 2, 3 = positive result.
        /// </summary>
        public int StarResult;


    }

    public class ProfileInfoForMinigame
    {

    }

    #endregion

    #region Context and providers

    /// <summary>
    /// Default Context for Antura Minigame.
    /// </summary>
    /// <seealso cref="EA4S.IGameContext" />
    public class AnturaMinigameContext : IGameContext
    {

        #region Log Manager 
        public ILogManager logManager = new AnturaLogManager();

        public ILogManager GetLogManager()
        {
            return logManager;
        }
        #endregion

        #region AudioManager provider
        public IAudioManager audioManager = new SampleAudioManager();

        public IAudioManager GetAudioManager()
        {
            return audioManager;
        }
        #endregion

        #region InputManger provider
        public IInputManager inputManager = new SampleInputManager();

        public IInputManager GetInputManager()
        {
            return inputManager;
        }
        #endregion

        #region SubTitle provider
        public ISubtitlesWidget subtitleWidget = new SampleSubtitlesWidget();

        public IStarsWidget GetStarsWidget()
        {
            return starsWidget;
        }
        #endregion

        #region StarsWidget provider
        public IStarsWidget starsWidget = new SampleStarsWidget();

        public ISubtitlesWidget GetSubtitleWidget()
        {
            return subtitleWidget;
        }
        #endregion

        #region PopupWidget provider
        public IPopupWidget questionWidget = new SamplePopupWidget();
        public IPopupWidget GetPopupWidget()
        {
            return questionWidget;
        }
        #endregion

        public void Reset()
        {
            inputManager.Reset();
        }

        #region CheckmarkWidget provider
        public ICheckmarkWidget checkmarkWidget = new SampleCheckmarkWidget();
        public ICheckmarkWidget GetCheckmarkWidget()
        {
            return checkmarkWidget;
        }
        #endregion

        #region Context Presets

        public static AnturaMinigameContext Default = new AnturaMinigameContext() {
            logManager = new AnturaLogManager(),
            audioManager = new SampleAudioManager(),
            subtitleWidget = new SampleSubtitlesWidget(),
            starsWidget = new SampleStarsWidget(),
            questionWidget = new SamplePopupWidget(),
        };

        /// <summary>
        /// Example for custom context preset used for fast crowd.
        /// </summary>
        public static AnturaMinigameContext FastCrowd = new AnturaMinigameContext() {
            logManager = new AnturaLogManager(),
            audioManager = new SampleAudioManager(),
            subtitleWidget = new SampleSubtitlesWidget(),
            starsWidget = new SampleStarsWidget(),
            questionWidget = new SamplePopupWidget(),
        };

        #endregion
    }

    /// <summary>
    /// Default IQuestionProvider that find the right letter question.
    /// </summary>
    /// <seealso cref="EA4S.IQuestionProvider" />
    public class FindRightLetterQuestionProvider : IQuestionProvider
    {

        #region properties
        List<IQuestionPack> questions = new List<IQuestionPack>();
        string description;
        int currentQuestion;
        #endregion

        public FindRightLetterQuestionProvider(List<IQuestionPack> _questionsPack, string descriptions)
        {
            currentQuestion = 0;
            description = "Antura Questions";

            questions.AddRange(_questionsPack);
        }

        public string GetDescription()
        {
            return description;
        }

        /// <summary>
        /// Provide me another question.
        /// </summary>
        /// <returns></returns>
        IQuestionPack IQuestionProvider.GetNextQuestion()
        {
            currentQuestion++;

            if (currentQuestion >= questions.Count)
                currentQuestion = 0;

            return questions[currentQuestion];
        }


    }

    /// <summary>
    /// Data Pack for "find right question" mechanics.
    /// One data question data, many right answare data, many answare data.
    /// </summary>
    /// <seealso cref="EA4S.IQuestionPack" />
    public class FindRightDataQuestionPack : IQuestionPack
    {
        ILivingLetterData questionSentence;
        IEnumerable<ILivingLetterData> wrongAnswersSentence;
        IEnumerable<ILivingLetterData> correctAnswersSentence;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindRightDataQuestionPack"/> class.
        /// </summary>
        /// <param name="questionSentence">The question sentence.</param>
        /// <param name="wrongAnswersSentence">The wrong answers sentence.</param>
        /// <param name="correctAnswersSentence">The correct answers sentence.</param>
        public FindRightDataQuestionPack(ILivingLetterData questionSentence, IEnumerable<ILivingLetterData> wrongAnswersSentence, IEnumerable<ILivingLetterData> correctAnswersSentence)
        {
            this.questionSentence = questionSentence;
            this.wrongAnswersSentence = wrongAnswersSentence;
            this.correctAnswersSentence = correctAnswersSentence;
        }

        ILivingLetterData IQuestionPack.GetQuestion()
        {
            return questionSentence;
        }

        IEnumerable<ILivingLetterData> IQuestionPack.GetWrongAnswers()
        {
            return wrongAnswersSentence;
        }

        public IEnumerable<ILivingLetterData> GetCorrectAnswers()
        {
            return correctAnswersSentence;
        }
    }


    #endregion

    /// <summary>
    /// Concrete implementation of log manager to store on db.
    /// </summary>
    public class AnturaLogManager : ILogManager
    {

        #region Log API for AI
        /// <summary>
        /// Answer result for question pack.
        /// </summary>
        /// <param name="_questionPack"></param>
        /// <param name="_isPositiveResult"></param>
        public void OnAnswer(IQuestionPack _questionPack, bool _isPositiveResult)
        {
            // Todo: Save on db
        }

        /// <summary>
        /// Answer result for single player action.
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="_isPositiveResult"></param>
        public void OnAnswer(ILivingLetterData _data, bool _isPositiveResult)
        {
            // Todo: Save on db
        }

        public void OnGameplaySessionResult(int _valuation)
        {
            // Todo: Save on db
        }
        #endregion

        #region Playsession Logs

        public void OnGameplayEvent(PlayerAbilities _ability, bool _isPositive)
        {
            switch (_ability) {
                case PlayerAbilities.precision:
                    break;
                case PlayerAbilities.speed:
                    break;
                default:
                    Debug.LogErrorFormat("Player ability {0} not found!", _ability);
                    break;
            }
        }


        #endregion

        #region Generic Log
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_area"></param>
        /// <param name="_context"></param>
        /// <param name="_action"></param>
        /// <param name="_data"></param>
        public void Log(string _area, string _context, string _action, string _data)
        {
            LoggerEA4S.Log(_area, _context, _action, _data);
        }
        #endregion

    }

    /// <summary>
    /// TODO: change location for this.
    /// Player abilities categories to trace player actions.
    /// </summary>
    public enum PlayerAbilities
    {
        precision,
        speed,
    }

}