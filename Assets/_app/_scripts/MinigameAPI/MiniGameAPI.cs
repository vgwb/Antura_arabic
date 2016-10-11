using UnityEngine;
using System.Collections.Generic;
using ModularFramework.Core;
using EA4S;
using System.Linq;

namespace EA4S.API {

    /// <summary>
    /// TODO: this API implementation will be replaced by dependency injection pattern implementation.
    /// </summary>
    /// <seealso cref="ModularFramework.Core.Singleton{EA4S.API.MiniGameAPI}" />
    public class MiniGameAPI : Singleton<MiniGameAPI> {

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
        public PlaySessionInfo GetPlaySessionInfo() {
            PlaySessionInfo playSessionInfo = new PlaySessionInfo();
            // TODO
            return playSessionInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_result"></param>
        public void PushPlaySessionResult(PlaySessionResult _result) {
            // TODO
        }

        /// <summary>
        /// Get corresponding arabic letter with _letterId param.
        /// </summary>
        /// <param name="_letterId"></param>
        /// <returns></returns>
        public string GetArabicLetter(string _letterId) {
            // TODO
            return "";
        }

        /// <summary>
        /// Return List of Letter Data valid for this gameplay and actual profile data.
        /// </summary>
        /// <returns></returns>
        public List<LetterData> GetValidLetters(int _amount = -1) {
            // TODO
            return new List<LetterData>();
        }

        /// <summary>
        /// Return List of Word Data valid for this gameplay and actual profile data.
        /// </summary>
        /// <param name="_amount"></param>
        /// <returns></returns>
        public List<WordData> GetValidWorlds(int _amount = -1) {
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
        public void Log(string _area, string _context, string _action, string _data) {
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
        public GameObject GetAsset(string _prefId, int _themeId) {
            // TODO
            return null;
        }

        #endregion

        #region Gameplay Management

        static string ActualGame = string.Empty;
        public string[] ActiveGames = new string[] { "Tobogan", "TestGame", "FastCrowd_v1", "FastCrowd_v2", "FastCrowd_v3", "FastCrowd_v4", };



        public void StartGame(string _gameName) {
            string prefix = "game_";
            switch (_gameName) {
                case "Tobogan":
                    // ====================================================
                    // Set configuration for the actual learning course context.
                    // ====================================================
                    Tobogan.ToboganConfiguration.Instance.Difficulty = 0.2f;
                    Tobogan.ToboganConfiguration.Instance.PipeQuestions = new AnturaDefaultQuestionProvider();
                    Tobogan.ToboganConfiguration.Instance.Context = new AnturaMinigameContext() {
                        audioManager = new SampleAudioManager(),
                        subtitleWidget = new SampleSubtitlesWidget(),
                        starsWidget = new SampleStarsWidget(),
                        questionWidget = new SamplePopupWidget(),
                    };
                    // ====================================================
                    // Call game start
                    AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition(prefix + "Tobogan");
                    break;
                case "TestGame":
                    break;
                case "FastCrowd_v1":
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = 1;
                    FastCrowd.FastCrowdConfiguration.Instance.PlayTime = 70;
                    FastCrowd.FastCrowdConfiguration.Instance.PipeQuestions = new AnturaDefaultQuestionProvider();
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.FastCrowd;
                    AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition(prefix + "FastCrowd");
                    break;
                case "FastCrowd_v2":
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = 2;
                    FastCrowd.FastCrowdConfiguration.Instance.PlayTime = 80;
                    FastCrowd.FastCrowdConfiguration.Instance.PipeQuestions = new AnturaDefaultQuestionProvider();
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.FastCrowd;
                    AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition(prefix + "FastCrowd");
                    break;
                case "FastCrowd_v3":
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = 3;
                    FastCrowd.FastCrowdConfiguration.Instance.PipeQuestions = new AnturaDefaultQuestionProvider();
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.FastCrowd;
                    AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition(prefix + "FastCrowd");
                    break;
                case "FastCrowd_v4":
                    FastCrowd.FastCrowdConfiguration.Instance.Variation = 4;
                    FastCrowd.FastCrowdConfiguration.Instance.PipeQuestions = new AnturaDefaultQuestionProvider();
                    FastCrowd.FastCrowdConfiguration.Instance.Context = AnturaMinigameContext.FastCrowd;
                    AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition(prefix + "FastCrowd");
                    break;
                default:
                    Debug.LogWarningFormat("Game {0} is not a valid active minigame!", _gameName);
                    break;
            }
        }

        #endregion

    }

    #region Data Structures

    /// <summary>
    /// Information needed from minigame to setup gameplay session.
    /// </summary>
    public class PlaySessionInfo {

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
    public class PlaySessionResult {
        /// <summary>
        /// Number of stars based on playsession result.
        /// 0 = negative result.
        /// 1, 2, 3 = positive result.
        /// </summary>
        public int StarResult;


    }

    public class ProfileInfoForMinigame {

    }

    #endregion

    #region Context and providers

    /// <summary>
    /// Default Context for Antura Minigame.
    /// </summary>
    /// <seealso cref="EA4S.IGameContext" />
    public class AnturaMinigameContext : IGameContext {

        public IAudioManager audioManager = new SampleAudioManager();
        public IInputManager inputManager = new SampleInputManager();
        public ISubtitlesWidget subtitleWidget = new SampleSubtitlesWidget();
        public IStarsWidget starsWidget = new SampleStarsWidget();
        public IPopupWidget questionWidget = new SamplePopupWidget();

        public IAudioManager GetAudioManager() {
            return audioManager;
        }

        public IInputManager GetInputManager()
        {
            return inputManager;
        }

        public IStarsWidget GetStarsWidget() {
            return starsWidget;
        }

        public ISubtitlesWidget GetSubtitleWidget() {
            return subtitleWidget;
        }

        public IPopupWidget GetPopupWidget() {
            return questionWidget;
        }

        #region presets

        public static AnturaMinigameContext Default = new AnturaMinigameContext() {
            audioManager = new SampleAudioManager(),
            subtitleWidget = new SampleSubtitlesWidget(),
            starsWidget = new SampleStarsWidget(),
            questionWidget = new SamplePopupWidget(),
        };

        /// <summary>
        /// Example for custom context preset used for fast crowd.
        /// </summary>
        public static AnturaMinigameContext FastCrowd = new AnturaMinigameContext() {
            audioManager = new SampleAudioManager(),
            subtitleWidget = new SampleSubtitlesWidget(),
            starsWidget = new SampleStarsWidget(),
            questionWidget = new SamplePopupWidget(),
        };

        #endregion
    }

    /// <summary>
    /// Default IQuestionProvider.
    /// </summary>
    /// <seealso cref="EA4S.IQuestionProvider" />
    public class AnturaDefaultQuestionProvider : IQuestionProvider {
        List<SampleQuestionPack> questions = new List<SampleQuestionPack>();
        string description;

        int currentQuestion;

        public AnturaDefaultQuestionProvider() {
            currentQuestion = 0;

            description = "Antura Questions";

            // 10 QuestionPacks
            for (int i = 0; i < 3; i++) {
                List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
                List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

                WordData newWordData = AppManager.Instance.Teacher.GimmeAGoodWordData();
                foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(newWordData.Word, AppManager.Instance.Letters)) {
                    correctAnswers.Add(letterData);
                }

                correctAnswers = correctAnswers.Distinct().ToList();

                // At least 4 wrong letters
                while (wrongAnswers.Count < 4) {
                    var letter = AppManager.Instance.Teacher.GimmeARandomLetter();

                    if (!correctAnswers.Contains(letter) && !wrongAnswers.Contains(letter)) {
                        wrongAnswers.Add(letter);
                    }
                }

                var currentPack = new SampleQuestionPack(newWordData, wrongAnswers, correctAnswers);
                questions.Add(currentPack);
            }
        }

        public string GetDescription() {
            return description;
        }

        IQuestionPack IQuestionProvider.GetNextQuestion() {
            currentQuestion++;

            if (currentQuestion >= questions.Count)
                currentQuestion = 0;

            return questions[currentQuestion];
        }
    }

    #endregion 
}