using Antura.Core;
using Antura.MinigamesCommon;
using Antura.Teacher;
using UnityEngine;

namespace Antura.MinigamesAPI
{
    /// <summary>
    /// Handles the logic to launch minigames with the correct configuration.
    /// </summary>
    public class MiniGameLauncher
    {
        private QuestionPacksGenerator questionPacksGenerator;
        private TeacherAI teacher;

        public MiniGameLauncher(TeacherAI _teacher)
        {
            teacher = _teacher;
            questionPacksGenerator = new QuestionPacksGenerator();
        }

        /// <summary>
        /// Prepare the context and start a minigame.
        /// </summary>
        /// <param name="_gameCode">The minigame code.</param>
        /// <param name="_launchConfiguration">The launch configuration. If null, the Teacher will generate a new one.</param>
        /// <param name="forceNewPlaySession">Is this a new play session?</param>
        public void LaunchGame(MiniGameCode _gameCode, MinigameLaunchConfiguration _launchConfiguration = null, bool forceNewPlaySession = false)
        {
            ConfigAI.StartTeacherReport();
            if (_launchConfiguration == null) {
                var difficulty = teacher.GetCurrentDifficulty(_gameCode);
                var numberOfRounds = teacher.GetCurrentNumberOfRounds(_gameCode);
                _launchConfiguration = new MinigameLaunchConfiguration(difficulty, numberOfRounds);
            }

            var miniGameData = AppManager.I.DB.GetMiniGameDataByCode(_gameCode);

            if (forceNewPlaySession) {
                AppManager.I.NavigationManager.InitNewPlaySession(miniGameData);
            }

            if (AppConstants.DebugLogEnabled) Debug.Log("StartGame " + _gameCode.ToString());

            // Assign the configuration for the given minigame
            var minigameSession = System.DateTime.Now.Ticks.ToString();
            var currentGameConfig = ConfigureMiniGame(_gameCode, minigameSession);
            currentGameConfig.Difficulty = _launchConfiguration.Difficulty;
            currentGameConfig.TutorialEnabled = _launchConfiguration.TutorialEnabled;

            // Set also the number of rounds
            // @note: only for assessment, for now
            if (currentGameConfig is Assessment.IAssessmentConfiguration) {
                var assessmentConfig = currentGameConfig as Assessment.IAssessmentConfiguration;
                assessmentConfig.NumberOfRounds = _launchConfiguration.NumberOfRounds;
            }

            // Retrieve the packs for the current minigame configuration
            var questionBuilder = currentGameConfig.SetupBuilder();
            var questionPacks = questionPacksGenerator.GenerateQuestionPacks(questionBuilder);
            currentGameConfig.Questions = new SequentialQuestionPackProvider(questionPacks);

            // Communicate to LogManager the start of a new single minigame play session.
            if (AppConstants.DebugLogDbInserts) Debug.Log("InitGameplayLogSession " + _gameCode.ToString());
            LogManager.I.LogInfo(InfoEvent.GameStart, "{\"minigame\":\"" + _gameCode.ToString() + "\"}");
            LogManager.I.StartMiniGame();

            // Print the teacher's report now
            ConfigAI.PrintTeacherReport();

            // Play the title dialog for the game
            //AudioManager.I.PlayDialogue(_gameCode.ToString()+"_Title");

            // Launch the game
            AppManager.I.NavigationManager.GotoMinigameScene();
        }

        /// <summary>
        /// Prepare the configuration for a given minigame.
        /// </summary>
        // TODO refactor: this depends on the specific minigames, should be abstracted
        public IGameConfiguration ConfigureMiniGame(MiniGameCode code, string sessionName)
        {
            var defaultContext = new MinigamesGameContext(code, sessionName);

            IGameConfiguration currentGameConfig = null;
            switch (code)
            {
                case MiniGameCode.Assessment_LetterForm:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_WordsWithLetter:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_MatchLettersToWord:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_MatchLettersToWord_Form:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_CompleteWord:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_CompleteWord_Form:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_OrderLettersOfWord:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_VowelOrConsonant:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SelectPronouncedWord:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_MatchWordToImage:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_WordArticle:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SingularDualPlural:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SunMoonWord:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_SunMoonLetter:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_QuestionAndReply:
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_counting:
                    currentGameConfig = Minigames.Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_letter:
                    currentGameConfig = Minigames.Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_spelling:
                    currentGameConfig = Minigames.Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.Balloons_words:
                    currentGameConfig = Minigames.Balloons.BalloonsConfiguration.Instance;
                    break;
                case MiniGameCode.ColorTickle:
                    currentGameConfig = Minigames.ColorTickle.ColorTickleConfiguration.Instance;
                    break;
                case MiniGameCode.DancingDots:
                    currentGameConfig = Minigames.DancingDots.DancingDotsConfiguration.Instance;
                    break;
                case MiniGameCode.Egg_letters:
                    currentGameConfig = Minigames.Egg.EggConfiguration.Instance;
                    break;
                case MiniGameCode.Egg_sequence:
                    currentGameConfig = Minigames.Egg.EggConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_alphabet:
                    currentGameConfig = Minigames.FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_counting:
                    currentGameConfig = Minigames.FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_letter:
                    currentGameConfig = Minigames.FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_spelling:
                    currentGameConfig = Minigames.FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_words:
                    currentGameConfig = Minigames.FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.TakeMeHome:
                    currentGameConfig = Minigames.TakeMeHome.TakeMeHomeConfiguration.Instance;
                    break;
                case MiniGameCode.HideSeek:
                    currentGameConfig = Minigames.HideAndSeek.HideAndSeekConfiguration.Instance;
                    break;
                case MiniGameCode.MakeFriends:
                    currentGameConfig = Minigames.MakeFriends.MakeFriendsConfiguration.Instance;
                    break;
                case MiniGameCode.Maze:
                    currentGameConfig = Minigames.Maze.MazeConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter:
                    currentGameConfig = Minigames.MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter_forms:
                    currentGameConfig = Minigames.MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter_phrases:
                    currentGameConfig = Minigames.MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MixedLetters_alphabet:
                    currentGameConfig = Minigames.MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.MixedLetters_spelling:
                    currentGameConfig = Minigames.MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.SickLetters:
                    currentGameConfig = Minigames.SickLetters.SickLettersConfiguration.Instance;
                    break;
                case MiniGameCode.ReadingGame:
                    currentGameConfig = Minigames.ReadingGame.ReadingGameConfiguration.Instance;
                    break;
                case MiniGameCode.AlphabetSong_alphabet:
                    currentGameConfig = Minigames.ReadingGame.ReadingGameConfiguration.Instance;
                    break;
                case MiniGameCode.AlphabetSong_letters:
                    currentGameConfig = Minigames.ReadingGame.ReadingGameConfiguration.Instance;
                    break;
                case MiniGameCode.Scanner:
                    currentGameConfig = Minigames.Scanner.ScannerConfiguration.Instance;
                    break;
                case MiniGameCode.Scanner_phrase:
                    currentGameConfig = Minigames.Scanner.ScannerConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_letters:
                    currentGameConfig = Minigames.ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_words:
                    currentGameConfig = Minigames.ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_letterinword:
                    currentGameConfig = Minigames.ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_letters:
                    currentGameConfig = Minigames.Tobogan.ToboganConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_words:
                    currentGameConfig = Minigames.Tobogan.ToboganConfiguration.Instance;
                    break;
                default:
                    Debug.LogWarningFormat("Configuration for MiniGame {0} not found.", code);
                    break;
            }

            if (currentGameConfig != null)
            {
                currentGameConfig.Context = defaultContext;
                currentGameConfig.SetMiniGameCode(code);
            }

            return currentGameConfig;
        }
    }
}
