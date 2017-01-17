using UnityEngine;
using ModularFramework.Core;
using EA4S.Db;

namespace EA4S.API
{

    /// <summary>
    /// This singleton class is the entry point to a minigame from the core app.
    /// </summary>
    // refactor: merge with MiniGameLauncher to provide a single entry point.
    public class MiniGameAPI : Singleton<MiniGameAPI>
    {

        #region Gameplay Management

        /// <summary>
        /// Prepare all context needed and starts the a minigame.
        /// </summary>
        /// <param name="_gameCode">The minigame code.</param>
        /// <param name="_gameConfiguration">The minigame configuration.</param>
        public void StartGame(MiniGameCode _gameCode, GameConfiguration _gameConfiguration)
        {
            if (AppConstants.VerboseLogging) Debug.Log("StartGame " + _gameCode.ToString());

            MiniGameData miniGameData = AppManager.I.DB.GetMiniGameDataByCode(_gameCode);
            IQuestionBuilder rules = null;
            IGameConfiguration currentGameConfig = null;

            string minigameSession = System.DateTime.Now.Ticks.ToString();

            currentGameConfig = ConfigureMiniGame(_gameCode, minigameSession);

            // game difficulty
            currentGameConfig.Difficulty = _gameConfiguration.Difficulty;
            // rule setted in config and used by AI to create correct game data
            rules = currentGameConfig.SetupBuilder();
            // question packs (game data)
            currentGameConfig.Questions = new FindRightLetterQuestionProvider(AppManager.I.GameLauncher.RetrieveQuestionPacks(rules), miniGameData.Description);

            // Save current game code to appmanager currentminigame
            AppManager.I.CurrentMinigame = miniGameData;
            // Comunicate to LogManager that start new single minigame play session.

            if (AppConstants.DebugLogInserts) Debug.Log("InitGameplayLogSession " + _gameCode.ToString());
            LogManager.I.LogInfo(InfoEvent.GameStart, _gameCode.ToString());

            // Call game start
            //NavigationManager.I.GoToNextScene();
            NavigationManager.I.GoToScene(miniGameData.Scene);
        }

        public IGameConfiguration ConfigureMiniGame(MiniGameCode code, string sessionName)
        {
            var defaultContext = new MinigamesGameContext(code, sessionName);

            IGameConfiguration currentGameConfig = null;
            switch (code) {
                case MiniGameCode.Assessment_LetterShape:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.LetterShape;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_WordsWithLetter:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.WordsWithLetter;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_MatchLettersToWord:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.MatchLettersToWord;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_CompleteWord:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.CompleteWord;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_OrderLettersOfWord:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.OrderLettersOfWord;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_VowelOrConsonant:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.VowelOrConsonant;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SelectPronouncedWord:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.SelectPronouncedWord;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_MatchWordToImage:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.MatchWordToImage;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_WordArticle:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.WordArticle;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SingularDualPlural:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.SingularDualPlural;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SunMoonWord:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.SunMoonWord;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SunMoonLetter:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.SunMoonLetter;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_QuestionAndReply:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.QuestionAndReply;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_counting:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Counting;
                    Balloons.BalloonsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_letter:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Letter;
                    Balloons.BalloonsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_spelling:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Spelling;
                    Balloons.BalloonsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_words:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Words;
                    Balloons.BalloonsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.ColorTickle:
                    ColorTickle.ColorTickleConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = ColorTickle.ColorTickleConfiguration.Instance;
                    break;
                case MiniGameCode.DancingDots:
                    DancingDots.DancingDotsConfiguration.Instance.Variation = DancingDots.DancingDotsVariation.V_1;
                    DancingDots.DancingDotsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = DancingDots.DancingDotsConfiguration.Instance;
                    break;
                case MiniGameCode.Egg:
                    Egg.EggConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Egg.EggConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_alphabet:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Alphabet;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_counting:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Counting;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_letter:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Letter;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_spelling:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Spelling;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_words:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Words;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.TakeMeHome:
                    TakeMeHome.TakeMeHomeConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = TakeMeHome.TakeMeHomeConfiguration.Instance;
                    break;
                case MiniGameCode.HideSeek:
                    HideAndSeek.HideAndSeekConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = HideAndSeek.HideAndSeekConfiguration.Instance;
                    break;
                case MiniGameCode.MakeFriends:
                    MakeFriends.MakeFriendsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = MakeFriends.MakeFriendsConfiguration.Instance;
                    break;
                case MiniGameCode.Maze:
                    Maze.MazeConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Maze.MazeConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter:
                    MissingLetter.MissingLetterConfiguration.Instance.Variation = MissingLetter.MissingLetterVariation.MissingLetter;
                    MissingLetter.MissingLetterConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter_phrases:
                    MissingLetter.MissingLetterConfiguration.Instance.Variation = MissingLetter.MissingLetterVariation.MissingWord;
                    MissingLetter.MissingLetterConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MixedLetters_alphabet:
                    // TODO: set variation
                    MixedLetters.MixedLettersConfiguration.Instance.Variation = MixedLetters.MixedLettersConfiguration.MixedLettersVariation.Alphabet;
                    MixedLetters.MixedLettersConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.MixedLetters_spelling:
                    MixedLetters.MixedLettersConfiguration.Instance.Variation = MixedLetters.MixedLettersConfiguration.MixedLettersVariation.Spelling;
                    MixedLetters.MixedLettersConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.SickLetters:
                    SickLetters.SickLettersConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = SickLetters.SickLettersConfiguration.Instance;
                    break;
                case MiniGameCode.ReadingGame:
                    ReadingGame.ReadingGameConfiguration.Instance.Variation = ReadingGame.ReadingGameVariation.ReadAndAnswer;
                    ReadingGame.ReadingGameConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = ReadingGame.ReadingGameConfiguration.Instance;
                    break;
                case MiniGameCode.AlphabetSong:
                    ReadingGame.ReadingGameConfiguration.Instance.Variation = ReadingGame.ReadingGameVariation.AlphabetSong;
                    ReadingGame.ReadingGameConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = ReadingGame.ReadingGameConfiguration.Instance;
                    break;
                case MiniGameCode.Scanner:
                    Scanner.ScannerConfiguration.Instance.Variation = Scanner.ScannerVariation.OneWord;
                    Scanner.ScannerConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Scanner.ScannerConfiguration.Instance;
                    break;
                case MiniGameCode.Scanner_phrase:
                    Scanner.ScannerConfiguration.Instance.Variation = Scanner.ScannerVariation.MultipleWords;
                    Scanner.ScannerConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Scanner.ScannerConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_letters:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.letters;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_words:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.words;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_letterinword:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.lettersinword;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_letters:
                    Tobogan.ToboganConfiguration.Instance.Variation = Tobogan.ToboganVariation.LetterInAWord;
                    Tobogan.ToboganConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Tobogan.ToboganConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_words:
                    Tobogan.ToboganConfiguration.Instance.Variation = Tobogan.ToboganVariation.SunMoon;
                    Tobogan.ToboganConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Tobogan.ToboganConfiguration.Instance;
                    break;
                default:
                    Debug.LogWarningFormat("Minigame selected {0} not found.", code.ToString());
                    break;
            }

            return currentGameConfig;
        }

        #endregion

    }
}