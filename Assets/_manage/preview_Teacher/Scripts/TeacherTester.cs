using System;
using System.Collections;
using System.Collections.Generic;
using EA4S.MinigamesAPI;
using EA4S.UI;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S.Teacher.Test
{
    /// <summary>
    /// Helper class to test Teacher functionality regardless of minigames.
    /// </summary>
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

        public Dictionary<MiniGameCode, Button> buttonsDict = new Dictionary<MiniGameCode, Button>();

        void Start()
        {
            // Setup for testing
            SetVerboseAI(true);
            ConfigAI.verboseQuestionPacks = verboseQuestionPacks;
            ConfigAI.verboseDataFiltering = verboseDataFiltering;
            ConfigAI.verboseDataSelection = verboseDataSelection;
            ConfigAI.verbosePlaySessionInitialisation = verbosePlaySessionInitialisation;
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

        int currentJourneyStage = 1;
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

        [Header("Reporting")]
        public bool verboseQuestionPacks = false;
        public bool verboseDataSelection = false;
        public bool verboseDataFiltering = false;
        public bool verbosePlaySessionInitialisation = false;

        private void InitialisePlaySession()
        {
            AppManager.I.Player.CurrentJourneyPosition.SetPosition(currentJourneyStage, currentJourneyLB, currentJourneyPS);
            AppManager.I.Teacher.InitialiseNewPlaySession();
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

        void SetVerboseAI(bool choice)
        {
            ConfigAI.verboseTeacher = choice;
        }

        void ReportPacks(List<QuestionPackData> packs)
        {
            if (verboseQuestionPacks)
            {
                string packsString = "----- GENERATED PACKS ----";
                foreach (var pack in packs)
                {
                    packsString += "\n" + pack.ToString();
                }
                ConfigAI.AppendToTeacherReport(packsString);
            }
        }

        #region Simulation

        [Header("Simulation")]
        public int numberOfSimulations = 50;
        public int yieldEverySimulations = 20;


        public void TestAllMiniGames()
        {
            SetVerboseAI(false);

            foreach (var code in Helpers.GenericHelper.SortEnums<MiniGameCode>())
            {
                var colors = buttonsDict[code].colors;
                colors.normalColor = Color.green;
                try
                {
                    SimulateMiniGame(code);
                }
                catch (Exception e)
                {
                    Debug.LogError(code + ": " + e);
                    colors.normalColor = Color.red;
                }
                buttonsDict[code].colors = colors;
            }

            SetVerboseAI(true);
        }

        public IEnumerator SimulateMiniGameCO(MiniGameCode code)
        {
            InitialisePlaySession();
            for (int i = 0; i < numberOfSimulations; i++)
            {
                var config = AppManager.I.GameLauncher.ConfigureMiniGame(code, System.DateTime.Now.Ticks.ToString());
                InitialisePlaySession();
                var builder = config.SetupBuilder();
                Debug.Log("SIMULATION " + (i + 1) + " minigame: " + code + " with builder " + builder.GetType().Name);
                builder.CreateAllQuestionPacks();
                if (i % yieldEverySimulations == 0)
                    yield return null;
            }
        }

        public void SimulateMiniGame(MiniGameCode code)
        {
            ConfigAI.StartTeacherReport();
            InitialisePlaySession();
            for (int i = 0; i < numberOfSimulations; i++)
            {
                var config = AppManager.I.GameLauncher.ConfigureMiniGame(code, System.DateTime.Now.Ticks.ToString());
                ConfigAI.AppendToTeacherReport("SIMULATION " + (i + 1) + " minigame: " + code);
                InitialisePlaySession();
                var builder = config.SetupBuilder();
                var packs = builder.CreateAllQuestionPacks();
                ReportPacks(packs);
            }
            ConfigAI.PrintTeacherReport();
        }

        #endregion


        #region  Question Builder testing

        private void TestQuestionBuilder(IQuestionBuilder builder)
        {
            ConfigAI.StartTeacherReport();
            var packs = builder.CreateAllQuestionPacks();
            ReportPacks(packs);
            ConfigAI.PrintTeacherReport();
        }

        public void RandomLettersTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new RandomLettersQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong,
                firstCorrectIsQuestion: true, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void AlphabetTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new AlphabetQuestionBuilder(parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void LettersBySunMoonTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new LettersBySunMoonQuestionBuilder(nPacks: nPacks, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void LettersByTypeTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new LettersByTypeQuestionBuilder(nPacks: nPacks, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void LettersInWordTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new LettersInWordQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, useAllCorrectLetters: true, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void LetterFormInWordsTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new LetterFormsInWordsQuestionBuilder(nPacks, 3, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void CommonLettersInWordTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new CommonLettersInWordQuestionBuilder(nPacks: nPacks, nMaxCommonLetters: 3, nWords: 2, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void RandomWordsTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new RandomWordsQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, firstCorrectIsQuestion: true, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void OrderedWordsTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new OrderedWordsQuestionBuilder(Database.WordDataCategory.NumberOrdinal, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void WordsWithLetterTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new WordsWithLetterQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void WordsByFormTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new WordsByFormQuestionBuilder(nPacks: nPacks, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void WordsByArticleTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new WordsByArticleQuestionBuilder(nPacks: nPacks, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void WordsBySunMoonTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new WordsBySunMoonQuestionBuilder(nPacks: nPacks, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void WordsInPhraseTest()
        {
            var builderParams = SetupFakeGame();
            var builder = new WordsInPhraseQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, useAllCorrectWords: false, usePhraseAnswersIfFound: true, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        public void PhraseQuestions()
        {
            var builderParams = SetupFakeGame();
            var builder = new PhraseQuestionsQuestionBuilder(nPacks: nPacks, nWrong: nWrong, parameters: builderParams);
            TestQuestionBuilder(builder);
        }

        #endregion

    }

}
