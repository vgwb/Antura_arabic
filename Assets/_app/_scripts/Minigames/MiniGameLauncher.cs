using EA4S.Core;
using EA4S.MinigamesCommon;
using EA4S.Teacher;
using UnityEngine;
using EA4S.Assessment;
using EA4S.Audio;

namespace EA4S.MinigamesAPI
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
                float difficulty = teacher.GetCurrentDifficulty(_gameCode);
                int numberOfRounds = teacher.GetCurrentNumberOfRounds(_gameCode);
                _launchConfiguration = new MinigameLaunchConfiguration(difficulty, numberOfRounds);
            }

            Database.MiniGameData miniGameData = AppManager.I.DB.GetMiniGameDataByCode(_gameCode);

            if (forceNewPlaySession) {
                AppManager.I.NavigationManager.InitialiseNewPlaySession(miniGameData);
            }

            if (AppConstants.VerboseLogging) Debug.Log("StartGame " + _gameCode.ToString());

            // Retrieve the configuration for the given minigame
            string minigameSession = System.DateTime.Now.Ticks.ToString();
            IGameConfiguration currentGameConfig = ConfigureMiniGame(_gameCode, minigameSession);
            currentGameConfig.Difficulty = _launchConfiguration.Difficulty;

            // Set also the number of rounds
            // @note: only for assessment, for now
            if (currentGameConfig is IAssessmentConfiguration) {
                IAssessmentConfiguration assessmentConfig = currentGameConfig as IAssessmentConfiguration;
                assessmentConfig.NumberOfRounds = _launchConfiguration.NumberOfRounds;
            }

            // Retrieve the packs for the current minigame configuration
            IQuestionBuilder questionBuilder = currentGameConfig.SetupBuilder();
            var questionPacks = questionPacksGenerator.GenerateQuestionPacks(questionBuilder);
            currentGameConfig.Questions = new SequentialQuestionPackProvider(questionPacks);

            // Comunicate to LogManager the start of a new single minigame play session.
            if (AppConstants.DebugLogInserts) Debug.Log("InitGameplayLogSession " + _gameCode.ToString());
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
        // refactor: this depends on the specific minigames, should be abstracted
        public IGameConfiguration ConfigureMiniGame(MiniGameCode code, string sessionName)
        {
            var defaultContext = new MinigamesGameContext(code, sessionName);

            IGameConfiguration currentGameConfig = null;
            switch (code) {
                case MiniGameCode.Assessment_LetterForm:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.LetterForm;
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
                case MiniGameCode.Assessment_MatchLettersToWord_Form:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.MatchLettersToWord_Form;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_CompleteWord:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.CompleteWord;
                    Assessment.AssessmentConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Assessment.AssessmentConfiguration.Instance;
                    break;
                case MiniGameCode.Assessment_CompleteWord_Form:
                    Assessment.AssessmentConfiguration.Instance.assessmentType = Assessment.AssessmentCode.CompleteWord_Form;
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
                    Minigames.ColorTickle.ColorTickleConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.ColorTickle.ColorTickleConfiguration.Instance;
                    break;
                case MiniGameCode.DancingDots:
                    Minigames.DancingDots.DancingDotsConfiguration.Instance.Variation = Minigames.DancingDots.DancingDotsVariation.V_1;
                    Minigames.DancingDots.DancingDotsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.DancingDots.DancingDotsConfiguration.Instance;
                    break;
                case MiniGameCode.Egg_letters:
                    Minigames.Egg.EggConfiguration.Instance.Variation = Minigames.Egg.EggConfiguration.EggVariation.Single;
                    Minigames.Egg.EggConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.Egg.EggConfiguration.Instance;
                    break;
                case MiniGameCode.Egg_sequence:
                    Minigames.Egg.EggConfiguration.Instance.Variation = Minigames.Egg.EggConfiguration.EggVariation.Sequence;
                    Minigames.Egg.EggConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.Egg.EggConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_alphabet:
                    Minigames.FastCrowd.FastCrowdConfiguration.Instance.Variation = Minigames.FastCrowd.FastCrowdVariation.Alphabet;
                    Minigames.FastCrowd.FastCrowdConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_counting:
                    Minigames.FastCrowd.FastCrowdConfiguration.Instance.Variation = Minigames.FastCrowd.FastCrowdVariation.Counting;
                    Minigames.FastCrowd.FastCrowdConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_letter:
                    Minigames.FastCrowd.FastCrowdConfiguration.Instance.Variation = Minigames.FastCrowd.FastCrowdVariation.Letter;
                    Minigames.FastCrowd.FastCrowdConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_spelling:
                    Minigames.FastCrowd.FastCrowdConfiguration.Instance.Variation = Minigames.FastCrowd.FastCrowdVariation.Spelling;
                    Minigames.FastCrowd.FastCrowdConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.FastCrowd_words:
                    Minigames.FastCrowd.FastCrowdConfiguration.Instance.Variation = Minigames.FastCrowd.FastCrowdVariation.Words;
                    Minigames.FastCrowd.FastCrowdConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.FastCrowd.FastCrowdConfiguration.Instance;
                    break;
                case MiniGameCode.TakeMeHome:
                    Minigames.TakeMeHome.TakeMeHomeConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.TakeMeHome.TakeMeHomeConfiguration.Instance;
                    break;
                case MiniGameCode.HideSeek:
                    Minigames.HideAndSeek.HideAndSeekConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.HideAndSeek.HideAndSeekConfiguration.Instance;
                    break;
                case MiniGameCode.MakeFriends:
                    Minigames.MakeFriends.MakeFriendsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.MakeFriends.MakeFriendsConfiguration.Instance;
                    break;
                case MiniGameCode.Maze:
                    Minigames.Maze.MazeConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.Maze.MazeConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter:
                    Minigames.MissingLetter.MissingLetterConfiguration.Instance.Variation = Minigames.MissingLetter.MissingLetterVariation.MissingLetter;
                    Minigames.MissingLetter.MissingLetterConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter_forms:
                    Minigames.MissingLetter.MissingLetterConfiguration.Instance.Variation = Minigames.MissingLetter.MissingLetterVariation.MissingForm;
                    Minigames.MissingLetter.MissingLetterConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MissingLetter_phrases:
                    Minigames.MissingLetter.MissingLetterConfiguration.Instance.Variation = Minigames.MissingLetter.MissingLetterVariation.MissingWord;
                    Minigames.MissingLetter.MissingLetterConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.MissingLetter.MissingLetterConfiguration.Instance;
                    break;
                case MiniGameCode.MixedLetters_alphabet:
                    Minigames.MixedLetters.MixedLettersConfiguration.Instance.Variation = Minigames.MixedLetters.MixedLettersConfiguration.MixedLettersVariation.Alphabet;
                    Minigames.MixedLetters.MixedLettersConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.MixedLetters_spelling:
                    Minigames.MixedLetters.MixedLettersConfiguration.Instance.Variation = Minigames.MixedLetters.MixedLettersConfiguration.MixedLettersVariation.Spelling;
                    Minigames.MixedLetters.MixedLettersConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.MixedLetters.MixedLettersConfiguration.Instance;
                    break;
                case MiniGameCode.SickLetters:
                    Minigames.SickLetters.SickLettersConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.SickLetters.SickLettersConfiguration.Instance;
                    break;
                case MiniGameCode.ReadingGame:
                    Minigames.ReadingGame.ReadingGameConfiguration.Instance.Variation = Minigames.ReadingGame.ReadingGameVariation.ReadAndAnswer;
                    Minigames.ReadingGame.ReadingGameConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.ReadingGame.ReadingGameConfiguration.Instance;
                    break;
                case MiniGameCode.AlphabetSong_alphabet:
                    Minigames.ReadingGame.ReadingGameConfiguration.Instance.Variation = Minigames.ReadingGame.ReadingGameVariation.AlphabetSong;
                    Minigames.ReadingGame.ReadingGameConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.ReadingGame.ReadingGameConfiguration.Instance;
                    break;
                case MiniGameCode.AlphabetSong_letters:
                    Minigames.ReadingGame.ReadingGameConfiguration.Instance.Variation = Minigames.ReadingGame.ReadingGameVariation.DiacriticSong;
                    Minigames.ReadingGame.ReadingGameConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.ReadingGame.ReadingGameConfiguration.Instance;
                    break;
                case MiniGameCode.Scanner:
                    Minigames.Scanner.ScannerConfiguration.Instance.Variation = Minigames.Scanner.ScannerVariation.OneWord;
                    Minigames.Scanner.ScannerConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.Scanner.ScannerConfiguration.Instance;
                    break;
                case MiniGameCode.Scanner_phrase:
                    Minigames.Scanner.ScannerConfiguration.Instance.Variation = Minigames.Scanner.ScannerVariation.MultipleWords;
                    Minigames.Scanner.ScannerConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.Scanner.ScannerConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_letters:
                    Minigames.ThrowBalls.ThrowBallsConfiguration.Instance.Variation = Minigames.ThrowBalls.ThrowBallsVariation.letters;
                    Minigames.ThrowBalls.ThrowBallsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_words:
                    Minigames.ThrowBalls.ThrowBallsConfiguration.Instance.Variation = Minigames.ThrowBalls.ThrowBallsVariation.words;
                    Minigames.ThrowBalls.ThrowBallsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.ThrowBalls_letterinword:
                    Minigames.ThrowBalls.ThrowBallsConfiguration.Instance.Variation = Minigames.ThrowBalls.ThrowBallsVariation.lettersinword;
                    Minigames.ThrowBalls.ThrowBallsConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.ThrowBalls.ThrowBallsConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_letters:
                    Minigames.Tobogan.ToboganConfiguration.Instance.Variation = Minigames.Tobogan.ToboganVariation.LetterInAWord;
                    Minigames.Tobogan.ToboganConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.Tobogan.ToboganConfiguration.Instance;
                    break;
                case MiniGameCode.Tobogan_words:
                    Minigames.Tobogan.ToboganConfiguration.Instance.Variation = Minigames.Tobogan.ToboganVariation.SunMoon;
                    Minigames.Tobogan.ToboganConfiguration.Instance.Context = defaultContext;
                    currentGameConfig = Minigames.Tobogan.ToboganConfiguration.Instance;
                    break;
                default:
                    Debug.LogWarningFormat("Minigame selected {0} not found.", code.ToString());
                    break;
            }

            return currentGameConfig;
        }

    }

}

