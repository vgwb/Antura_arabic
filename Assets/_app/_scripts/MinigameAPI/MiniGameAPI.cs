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
        public List<LL_LetterData> GetValidLetters(int _amount = -1)
        {
            // TODO
            return new List<LL_LetterData>();
        }

        /// <summary>
        /// Return List of Word Data valid for this gameplay and actual profile data.
        /// </summary>
        /// <param name="_amount"></param>
        /// <returns></returns>
        public List<LL_WordData> GetValidWorlds(int _amount = -1)
        {
            // TODO
            return new List<LL_WordData>();
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

        public void StartGame(MiniGameCode _gameCode, GameConfiguration _gameConfiguration)
        {
            // To be deleted
            List<IQuestionPack> _gameData = null;

            MiniGameData miniGameData = AppManager.Instance.DB.GetMiniGameDataByCode(_gameCode);
            string miniGameScene = miniGameData.Scene;
            IQuestionBuilder rules = null;
            IGameConfiguration actualConfig = null;

            switch (_gameCode) {
                case MiniGameCode.Assessment_LetterShape:
                case MiniGameCode.Assessment_WordsWithLetter:
                case MiniGameCode.Assessment_MatchLettersToWord:
                case MiniGameCode.Assessment_CompleteWord:
                case MiniGameCode.Assessment_OrderLettersOfWord:
                case MiniGameCode.Assessment_VowelOrConsonant:
                case MiniGameCode.Assessment_SelectPronouncedWord:
                case MiniGameCode.Assessment_MatchWordToImage:
                case MiniGameCode.Assessment_WordArticle:
                case MiniGameCode.Assessment_SingularDualPlural:
                case MiniGameCode.Assessment_SunMoonWord:
                case MiniGameCode.Assessment_SunMoonLetter:
                case MiniGameCode.Assessment_QuestionAndReply:
                    break;
                case MiniGameCode.AlphabetSong:
                    // Must be defined how use sentence data structure
                    break;
                case MiniGameCode.Balloons_counting:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Counting;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_letter:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Letter;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_spelling:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Spelling;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_words:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Words;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.ColorTickle:
                    ColorTickle.ColorTickleConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = ColorTickle.ColorTickleConfiguration.Instance;
                    break;
                case MiniGameCode.DancingDots:
                    DancingDots.DancingDotsConfiguration.Instance.Variation = DancingDots.DancingDotsVariation.V_1;
                    DancingDots.DancingDotsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = DancingDots.DancingDotsConfiguration.Instance;
                    break;
                case MiniGameCode.DontWakeUp:
                    // 
                    break;
                case MiniGameCode.Egg:
                    Egg.EggConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Egg.EggConfiguration.Instance;
                    miniGameScene = "game_Egg"; // TODO: check
                    break;
                case MiniGameCode.FastCrowd_alphabet:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Alphabet;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_counting:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Counting;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_letter:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Letter;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_spelling:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Spelling;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_words:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Words;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.TakeMeHome:
                    TakeMeHome.TakeMeHomeConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = TakeMeHome.TakeMeHomeConfiguration.Instance;
                    break;
                case MiniGameCode.HiddenSource:
                    // It has now become TakeMeHome
                    break;
                case MiniGameCode.HideSeek:
                    HideAndSeek.HideAndSeekConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = HideAndSeek.HideAndSeekConfiguration.Instance;
                    break;
                case MiniGameCode.MakeFriends:
                    MakeFriends.MakeFriendsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = MakeFriends.MakeFriendsConfiguration.Instance;
                    break;
                case MiniGameCode.Maze:
                    Maze.MazeConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Maze.MazeConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter:
                    MissingLetter.MissingLetterConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter_phrases:
                    // Must be defined how use sentence data structure
                    break;
                case MiniGameCode.MixedLetters_alphabet:
                    // TODO: set variation
                    MixedLetters.MixedLettersConfiguration.Instance.Variation = MixedLetters.MixedLettersConfiguration.MixedLettersVariation.Alphabet;
                    MixedLetters.MixedLettersConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.MixedLetters_spelling:
                    MixedLetters.MixedLettersConfiguration.Instance.Variation = MixedLetters.MixedLettersConfiguration.MixedLettersVariation.Spelling;
                    MixedLetters.MixedLettersConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.SickLetters:
                    SickLetters.SickLettersConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = SickLetters.SickLettersConfiguration.Instance;
                    break;
                case MiniGameCode.ReadingGame:
                    // Must be defined how use sentence data structure
                    break;
                case MiniGameCode.Scanner:
                    //                    Scanner.ScannerConfiguration.Instance.Variation = Scanner.ScannerVariation.V_1;
                    Scanner.ScannerConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Scanner.ScannerConfiguration.Instance;
                    break;
                //                case MiniGameCode.Scanner_phrase:
                //                    Scanner.ScannerConfiguration.Instance.Variation = Scanner.ScannerVariation.phrase;
                //                    Scanner.ScannerConfiguration.Instance.Context = AnturaMinigameContext.Default;
                //                    actualConfig = Scanner.ScannerConfiguration.Instance;
                //                    break;
                case MiniGameCode.ThrowBalls_letters:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.letters;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_words:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.words;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_letterinword:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.lettersinword;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_letters:
                    Tobogan.ToboganConfiguration.Instance.Variation = Tobogan.ToboganVariation.LetterInAWord;
                    Tobogan.ToboganConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Tobogan.ToboganConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_words:
                    Tobogan.ToboganConfiguration.Instance.Variation = Tobogan.ToboganVariation.SunMoon;
                    Tobogan.ToboganConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    actualConfig = Tobogan.ToboganConfiguration.Instance;
                    break;
                default:
                    Debug.LogWarningFormat("Minigame selected {0} not found.", _gameCode.ToString());
                    break;
            }

            // Set game configuration instance with game data
            // game difficulty
            actualConfig.Difficulty = _gameConfiguration.Difficulty;
            // rule setted in config and used by AI to create correct game data
            rules = actualConfig.SetupBuilder();
            // question packs (game data)
            actualConfig.Questions = new FindRightLetterQuestionProvider(AppManager.Instance.GameLauncher.RetrieveQuestionPacks(rules), miniGameData.Description);

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
            audioManager.Reset();
            inputManager.Reset();
        }

        #region CheckmarkWidget provider
        public ICheckmarkWidget checkmarkWidget = new SampleCheckmarkWidget();
        public ICheckmarkWidget GetCheckmarkWidget()
        {
            return checkmarkWidget;
        }
        #endregion

        #region OverlayWidget provider
        IOverlayWidget overlayWidget = new MinigamesOverlayWidget();
        public IOverlayWidget GetOverlayWidget()
        {
            return overlayWidget;
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
        IEnumerable<ILivingLetterData> questionsSentences;
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
            this.questionsSentences = new List<ILivingLetterData>() { questionSentence };
            this.wrongAnswersSentence = wrongAnswersSentence;
            this.correctAnswersSentence = correctAnswersSentence;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FindRightDataQuestionPack"/> class.
        /// </summary>
        /// <param name="questionsSentences">The questions sentences.</param>
        /// <param name="wrongAnswersSentence">The wrong answers sentence.</param>
        /// <param name="correctAnswersSentence">The correct answers sentence.</param>
        public FindRightDataQuestionPack(IEnumerable<ILivingLetterData> questionsSentences, IEnumerable<ILivingLetterData> wrongAnswersSentence, IEnumerable<ILivingLetterData> correctAnswersSentence)
        {
            this.questionsSentences = questionsSentences;
            this.wrongAnswersSentence = wrongAnswersSentence;
            this.correctAnswersSentence = correctAnswersSentence;
        }

        ILivingLetterData IQuestionPack.GetQuestion()
        {
            return questionsSentences.First();
        }

        public IEnumerable<ILivingLetterData> GetQuestions()
        {
            return questionsSentences;
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

        #region Test
        public void TestDb()
        {
            AppManager.Instance.DB.Insert(new LogInfoData() {

            });
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