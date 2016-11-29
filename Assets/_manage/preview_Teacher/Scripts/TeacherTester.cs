using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EA4S.Teacher.Test
{

    public class TeacherTester : MonoBehaviour
    {
        public InputField journey_stage_in;
        public InputField journey_learningblock_in;
        public InputField journey_playsession_in;
        public InputField npacks_in;
        public InputField ncorrect_in;
        public InputField nwrong_in;
        public Dropdown severity_in;
        public Dropdown severitywrong_in;
        public Dropdown history_in;
        public Dropdown historywrong_in;
        public Toggle journeybase_in;
        public Toggle journeywrong_in;

        void Start()
        {
            // Setup for testing
            ConfigAI.verboseDataSelection = true;
            ConfigAI.verboseTeacher = true;
            ConfigAI.forceJourneyIgnore = false;

            journey_stage_in.onValueChanged.AddListener(x => { currentJourneyStage = int.Parse(x); });
            journey_learningblock_in.onValueChanged.AddListener(x => { currentJourneyLB = int.Parse(x); });
            journey_playsession_in.onValueChanged.AddListener(x => { currentJourneyPS = int.Parse(x); });

            npacks_in.onValueChanged.AddListener(x => { nPacks = int.Parse(x); });
            ncorrect_in.onValueChanged.AddListener(x => { nCorrect = int.Parse(x); });
            nwrong_in.onValueChanged.AddListener(x => { nWrong = int.Parse(x); });

            severity_in.onValueChanged.AddListener(x => { correctSeverity = (SelectionSeverity)x; });
            severitywrong_in.onValueChanged.AddListener(x => { wrongSeverity = (SelectionSeverity)x; });

            history_in.onValueChanged.AddListener(x => { correctHistory = (PackListHistory)x; });
            historywrong_in.onValueChanged.AddListener(x => { wrongHistory = (PackListHistory)x; });

            journeybase_in.onValueChanged.AddListener(x => { journeyEnabledForBase = x; });
            journeywrong_in.onValueChanged.AddListener(x => { journeyEnabledForWrong = x; });

            GlobalUI.ShowPauseMenu(false);
        }

        int currentJourneyStage = 3;
        int currentJourneyLB = 1;
        int currentJourneyPS = 1;
        int nPacks = 5;
        int nCorrect = 1;
        int nWrong = 1;
        SelectionSeverity correctSeverity;
        SelectionSeverity wrongSeverity;
        PackListHistory correctHistory;
        PackListHistory wrongHistory;
        bool journeyEnabledForBase = true;
        bool journeyEnabledForWrong = true;

        private void InitialisePlaySession()
        {
            AppManager.I.Player.CurrentJourneyPosition.Stage = currentJourneyStage;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = currentJourneyLB;
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = currentJourneyPS;
            AppManager.I.Teacher.InitialiseCurrentPlaySession();
        }

        QuestionBuilderParameters SetupFakeGame()
        {
            InitialisePlaySession();

            var builderParams = new QuestionBuilderParameters();
            builderParams.correctChoicesHistory = correctHistory;
            builderParams.wrongChoicesHistory = wrongHistory;
            builderParams.correctSeverity = correctSeverity;
            builderParams.wrongSeverity = wrongSeverity;
            builderParams.useJourneyForCorrect = journeyEnabledForBase;
            builderParams.useJourneyForWrong = journeyEnabledForWrong;
            return builderParams;
        }

        public void SimulateMiniGame(MiniGameCode code)
        {
            var config = API.MiniGameAPI.Instance.GetGameConfigurationForMiniGameCode(code);
            InitialisePlaySession();
            var builder = config.SetupBuilder();
            Debug.Log("Simulating minigame: " + code + " with builder " + builder.GetType().Name);
            builder.CreateAllQuestionPacks();
        }

        #region  Question Builder testing


        public void RandomLettersTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new RandomLettersQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong,
                firstCorrectIsQuestion: true, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void AlphabetTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new AlphabetQuestionBuilder(parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void LettersBySunMoonTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new LettersBySunMoonQuestionBuilder(nPacks: nPacks, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void LettersByTypeTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new LettersByTypeQuestionBuilder(nPacks: nPacks, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void LettersInWordTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new LettersInWordQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, useAllCorrectLetters: true, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void CommonLettersInWordTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new CommonLettersInWordQuestionBuilder(nPacks: nPacks, nMaxCommonLetters: 3, nWords: 2, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void RandomWordsTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new RandomWordsQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, firstCorrectIsQuestion: true, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void OrderedWordsTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new OrderedWordsQuestionBuilder(Db.WordDataCategory.NumberOrdinal, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void WordsWithLetterTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new WordsWithLetterQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void WordsByFormTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new WordsByFormQuestionBuilder(nPacks: nPacks, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void WordsByArticleTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new WordsByArticleQuestionBuilder(nPacks: nPacks, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void WordsBySunMoonTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new WordsBySunMoonQuestionBuilder(nPacks: nPacks, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void WordsInPhraseTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new WordsInPhraseQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, useAllCorrectWords: false, usePhraseAnswersIfFound: true, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        public void PhraseQuestions()
        {
            var builderParams = SetupFakeGame();
            var builder = new PhraseQuestionsQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, parameters: builderParams);
            builder.CreateAllQuestionPacks();
        }

        #endregion

    }

}
