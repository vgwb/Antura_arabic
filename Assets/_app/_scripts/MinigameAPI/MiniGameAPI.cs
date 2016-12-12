using UnityEngine;
using System.Collections.Generic;
using ModularFramework.Core;
using EA4S.Db;
using EA4S;

namespace EA4S.API
{

    /// <summary>
    /// Entry point to start fame from app.
    /// </summary>
    /// <seealso cref="ModularFramework.Core.Singleton{EA4S.API.MiniGameAPI}" />
    public class MiniGameAPI : Singleton<MiniGameAPI>
    {

        #region Gameplay Management

        /// <summary>
        /// Prepare all context needed and starts the game.
        /// </summary>
        /// <param name="_gameCode">The game code.</param>
        /// <param name="_gameConfiguration">The game configuration.</param>
        public void StartGame(MiniGameCode _gameCode, GameConfiguration _gameConfiguration)
        {
            if (AppConstants.VerboseLogging) Debug.Log("StartGame " + _gameCode.ToString());

            MiniGameData miniGameData = AppManager.I.DB.GetMiniGameDataByCode(_gameCode);
            IQuestionBuilder rules = null;
            IGameConfiguration currentGameConfig = null;

            currentGameConfig = GetGameConfigurationForMiniGameCode(_gameCode);

            // game difficulty
            currentGameConfig.Difficulty = _gameConfiguration.Difficulty;
            // rule setted in config and used by AI to create correct game data
            rules = currentGameConfig.SetupBuilder();
            // question packs (game data)
            currentGameConfig.Questions = new FindRightLetterQuestionProvider(AppManager.I.GameLauncher.RetrieveQuestionPacks(rules), miniGameData.Description);

            // Save current game code to appmanager currentminigame
            AppManager.I.CurrentMinigame = miniGameData;
            // Comunicate to LogManager that start new single minigame play session.
            currentGameConfig.Context.GetLogManager().InitGameplayLogSession(_gameCode);

            // Call game start
            //NavigationManager.I.GoToNextScene();
            NavigationManager.I.GoToScene(miniGameData.Scene);
        }

        public IGameConfiguration GetGameConfigurationForMiniGameCode(MiniGameCode code)
        {
            IGameConfiguration currentGameConfig = null;
            switch (code) {
                case MiniGameCode.Assessment_LetterShape:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.LetterShape;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_WordsWithLetter:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.WordsWithLetter;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_MatchLettersToWord:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.MatchLettersToWord;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_CompleteWord:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.CompleteWord;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_OrderLettersOfWord:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.OrderLettersOfWord;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_VowelOrConsonant:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.VowelOrConsonant;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SelectPronouncedWord:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.SelectPronouncedWord;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_MatchWordToImage:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.MatchWordToImage;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_WordArticle:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.WordArticle;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SingularDualPlural:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.SingularDualPlural;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SunMoonWord:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.SunMoonWord;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SunMoonLetter:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.SunMoonLetter;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_QuestionAndReply:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.QuestionAndReply;
                    Assessment.AssessmentConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_counting:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Counting;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_letter:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Letter;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_spelling:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Spelling;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_words:
                    Balloons.BalloonsConfiguration.Instance.Variation = Balloons.BalloonsVariation.Words;
                    Balloons.BalloonsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.ColorTickle:
                    ColorTickle.ColorTickleConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = ColorTickle.ColorTickleConfiguration.Instance;
                    break;
                case MiniGameCode.DancingDots:
                    DancingDots.DancingDotsConfiguration.Instance.Variation = DancingDots.DancingDotsVariation.V_1;
                    DancingDots.DancingDotsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = DancingDots.DancingDotsConfiguration.Instance;
                    break;
                case MiniGameCode.DontWakeUp:
                    // 
                    break;
                case MiniGameCode.Egg:
                    Egg.EggConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Egg.EggConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_alphabet:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Alphabet;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_counting:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Counting;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_letter:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Letter;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_spelling:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Spelling;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_words:
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = FastCrowd.FastCrowdVariation.Words;
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.TakeMeHome:
                    TakeMeHome.TakeMeHomeConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = TakeMeHome.TakeMeHomeConfiguration.Instance;
                    break;
                case MiniGameCode.HiddenSource:
                    // It has now become TakeMeHome
                    break;
                case MiniGameCode.HideSeek:
                    HideAndSeek.HideAndSeekConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = HideAndSeek.HideAndSeekConfiguration.Instance;
                    break;
                case MiniGameCode.MakeFriends:
                    MakeFriends.MakeFriendsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = MakeFriends.MakeFriendsConfiguration.Instance;
                    break;
                case MiniGameCode.Maze:
                    Maze.MazeConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Maze.MazeConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter:
                    MissingLetter.MissingLetterConfiguration.Instance.Variation = MissingLetter.MissingLetterVariation.MissingLetter;
                    MissingLetter.MissingLetterConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter_phrases:
                    MissingLetter.MissingLetterConfiguration.Instance.Variation = MissingLetter.MissingLetterVariation.MissingWord;
                    MissingLetter.MissingLetterConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MixedLetters_alphabet:
                    // TODO: set variation
                    MixedLetters.MixedLettersConfiguration.Instance.Variation = MixedLetters.MixedLettersConfiguration.MixedLettersVariation.Alphabet;
                    MixedLetters.MixedLettersConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.MixedLetters_spelling:
                    MixedLetters.MixedLettersConfiguration.Instance.Variation = MixedLetters.MixedLettersConfiguration.MixedLettersVariation.Spelling;
                    MixedLetters.MixedLettersConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.SickLetters:
                    SickLetters.SickLettersConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = SickLetters.SickLettersConfiguration.Instance;
                    break;
                case MiniGameCode.ReadingGame:
                    ReadingGame.ReadingGameConfiguration.Instance.Variation = ReadingGame.ReadingGameVariation.ReadAndAnswer;
                    ReadingGame.ReadingGameConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = ReadingGame.ReadingGameConfiguration.Instance;
                    break;
                case MiniGameCode.AlphabetSong:
                    ReadingGame.ReadingGameConfiguration.Instance.Variation = ReadingGame.ReadingGameVariation.AlphabetSong;
                    ReadingGame.ReadingGameConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = ReadingGame.ReadingGameConfiguration.Instance;
                    break;
                case MiniGameCode.Scanner:
                    Scanner.ScannerConfiguration.Instance.Variation = Scanner.ScannerVariation.OneWord;
                    Scanner.ScannerConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Scanner.ScannerConfiguration.Instance;
                    break;
                case MiniGameCode.Scanner_phrase:
                    Scanner.ScannerConfiguration.Instance.Variation = Scanner.ScannerVariation.MultipleWords;
                    Scanner.ScannerConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Scanner.ScannerConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_letters:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.letters;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_words:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.words;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_letterinword:
                    ThrowBalls.ThrowBallsConfiguration.Instance.Variation = ThrowBalls.ThrowBallsVariation.lettersinword;
                    ThrowBalls.ThrowBallsConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_letters:
                    Tobogan.ToboganConfiguration.Instance.Variation = Tobogan.ToboganVariation.LetterInAWord;
                    Tobogan.ToboganConfiguration.Instance.Context = AnturaMinigameContext.Default;
                    currentGameConfig = Tobogan.ToboganConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_words:
                    Tobogan.ToboganConfiguration.Instance.Variation = Tobogan.ToboganVariation.SunMoon;
                    Tobogan.ToboganConfiguration.Instance.Context = AnturaMinigameContext.Default;
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