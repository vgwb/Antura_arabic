using System;
using System.Collections;
using System.Collections.Generic;
using EA4S.UI;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S.Teacher.Test
{
    public enum QuestionBuilderType
    {
        Empty,

        RandomLetters,
        Alphabet,
        LettersBySunMoon,
        LettersByType,

        RandomWords,
        OrderedWords,
        WordsByArticle,
        WordsByForm,
        WordsBySunMoon,

        LettersInWord,
        LetterFormsInWords,
        CommonLettersInWords,
        WordsWithLetter,

        WordsInPhrase,
        PhraseQuestions,

        MAX
    }

    /// <summary>
    /// Helper class to test Teacher functionality regardless of minigames.
    /// </summary>
    public class TeacherTester : MonoBehaviour
    {
        [Header("Reporting")]
        public bool verboseQuestionPacks = false;
        public bool verboseDataSelection = false;
        public bool verboseDataFiltering = false;
        public bool verbosePlaySessionInitialisation = false;

        [Header("Simulation")]
        public int numberOfSimulations = 50;
        public int yieldEverySimulations = 20;

        [Header("UI")]
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

        // Current options
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

        public Dictionary<MiniGameCode, Button> minigamesButtonsDict = new Dictionary<MiniGameCode, Button>();
        public Dictionary<QuestionBuilderType, Button> qbButtonsDict = new Dictionary<QuestionBuilderType, Button>();

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

        private void InitialisePlaySession()
        {
            AppManager.I.Player.CurrentJourneyPosition.SetPosition(currentJourneyStage, currentJourneyLB, currentJourneyPS);
            AppManager.I.Teacher.InitialiseNewPlaySession();
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


        #region Testing

        public void TestEverything()
        {
            TestAllMiniGames();
            TestAllQuestionBuilders();
        }
        
        public void TestAllMiniGames()
        {
            SetVerboseAI(false);

            foreach (var code in Helpers.GenericHelper.SortEnums<MiniGameCode>())
            {
                if (code == MiniGameCode.Invalid) continue;
                if (code == MiniGameCode.Assessment_VowelOrConsonant) continue;

                var colors = minigamesButtonsDict[code].colors;
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
                minigamesButtonsDict[code].colors = colors;
            }

            SetVerboseAI(true);
        }


        public void TestAllQuestionBuilders()
        {
            SetVerboseAI(false);

            foreach (var type in Helpers.GenericHelper.SortEnums<QuestionBuilderType>())
            {
                var colors = qbButtonsDict[type].colors;
                colors.normalColor = Color.green;
                try
                {
                    TestQuestionBuilder(type);
                }
                catch (Exception e)
                {
                    Debug.LogError(type + ": " + e);
                    colors.normalColor = Color.red;
                }
                qbButtonsDict[type].colors = colors;
            }

            SetVerboseAI(true);
        }

        #endregion


        #region Minigames

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


        public void TestQuestionBuilder(QuestionBuilderType builderType)
        {
            ConfigAI.StartTeacherReport();

            var builderParams = SetupFakeGame();
            IQuestionBuilder builder = null;
            switch (builderType)
            {
                case QuestionBuilderType.RandomLetters:
                    builder = new RandomLettersQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, firstCorrectIsQuestion: true, parameters: builderParams);
                    break;
                case QuestionBuilderType.Alphabet:
                    builder = new AlphabetQuestionBuilder(parameters: builderParams);
                    break;
                case QuestionBuilderType.LettersBySunMoon:
                    builder = new LettersBySunMoonQuestionBuilder(nPacks: nPacks, parameters: builderParams);
                    break;
                case QuestionBuilderType.LettersByType:
                    builder = new LettersByTypeQuestionBuilder(nPacks: nPacks, parameters: builderParams);
                    break;
                case QuestionBuilderType.LettersInWord:
                    builder = new LettersInWordQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, useAllCorrectLetters: true, parameters: builderParams);
                    break;
                case QuestionBuilderType.LetterFormsInWords:
                    builder = new LetterFormsInWordsQuestionBuilder(nPacks, 3, parameters: builderParams);
                    break;
                case QuestionBuilderType.CommonLettersInWords:
                    builder = new CommonLettersInWordQuestionBuilder(nPacks: nPacks, nWrong: nWrong, parameters: builderParams);
                    break;
                case QuestionBuilderType.RandomWords:
                    builder = new RandomWordsQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, firstCorrectIsQuestion: true, parameters: builderParams);
                    break;
                case QuestionBuilderType.OrderedWords:
                    builder = new OrderedWordsQuestionBuilder(Database.WordDataCategory.NumberOrdinal, parameters: builderParams);
                    break;
                case QuestionBuilderType.WordsWithLetter:
                    builder = new WordsWithLetterQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, parameters: builderParams);
                    break;
                case QuestionBuilderType.WordsByForm:
                    builder = new WordsByFormQuestionBuilder(nPacks: nPacks, parameters: builderParams);
                    break;
                case QuestionBuilderType.WordsByArticle:
                    builder = new WordsByArticleQuestionBuilder(nPacks: nPacks, parameters: builderParams);
                    break;
                case QuestionBuilderType.WordsBySunMoon:
                    builder = new WordsBySunMoonQuestionBuilder(nPacks: nPacks, parameters: builderParams);
                    break;
                case QuestionBuilderType.WordsInPhrase:
                    builder = new WordsInPhraseQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, useAllCorrectWords: false, usePhraseAnswersIfFound: true, parameters: builderParams);
                    break;
                case QuestionBuilderType.PhraseQuestions:
                    builder = new PhraseQuestionsQuestionBuilder(nPacks: nPacks, nWrong: nWrong, parameters: builderParams);
                    break;
            }

            var packs = builder.CreateAllQuestionPacks();
            ReportPacks(packs);

            ConfigAI.PrintTeacherReport();
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

        #endregion

    }

}
