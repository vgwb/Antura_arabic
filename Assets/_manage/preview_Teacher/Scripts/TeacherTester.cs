using System;
using System.Collections;
using System.Collections.Generic;
using DG.DeInspektor.Attributes;
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
        [DeBeginGroup]
        [Header("Reporting")]
        [DeToggleButton(DePosition.HHalfLeft)]
        public bool verboseQuestionPacks = false;
        [DeToggleButton(DePosition.HHalfRight)]
        public bool verboseDataSelection = false;
        [DeToggleButton(DePosition.HHalfLeft)]
        public bool verboseDataFiltering = false;
        [DeEndGroup]
        [DeToggleButton(DePosition.HHalfRight)]
        public bool verbosePlaySessionInitialisation = false;

        [DeBeginGroup]
        [Header("Simulation")]
        public int numberOfSimulations = 50;
        [DeEndGroup]
        public int yieldEverySimulations = 20;

        // Current options
        [DeBeginGroup]
        [Header("Journey")]
        [Range(1, 6)]
        public int currentJourneyStage = 1;
        [Range(1, 15)]
        public int currentJourneyLB = 1;
        [DeToggleButton()]
        [DeEndGroup]
        public bool isAssessment = false;
        //int currentJourneyPS = 1;

        [DeBeginGroup]
        [Header("Selection Parameters")]
        [Range(1, 10)]
        public int nPacks = 5;

        [Range(1, 10)]
        public int nCorrectAnswers = 1;
        public SelectionSeverity correctSeverity = SelectionSeverity.MayRepeatIfNotEnough;
        public PackListHistory correctHistory = PackListHistory.RepeatWhenFull;
        [DeToggleButton()]
        public bool journeyEnabledForBase = true;

        [Range(1, 10)]
        public int nWrongAnswers = 1;
        public SelectionSeverity wrongSeverity = SelectionSeverity.MayRepeatIfNotEnough;
        public PackListHistory wrongHistory = PackListHistory.RepeatWhenFull;
        [DeEndGroup]
        [DeToggleButton()]
        public bool journeyEnabledForWrong = true;

        [HideInInspector]
        public InputField journey_stage_in;
        [HideInInspector]
        public InputField journey_learningblock_in;
        [HideInInspector]
        public InputField journey_playsession_in;
        [HideInInspector]
        public InputField npacks_in;
        [HideInInspector]
        public InputField ncorrect_in;
        [HideInInspector]
        public InputField nwrong_in;
        [HideInInspector]
        public Dropdown severity_in;
        [HideInInspector]
        public Dropdown severitywrong_in;
        [HideInInspector]
        public Dropdown history_in;
        [HideInInspector]
        public Dropdown historywrong_in;
        [HideInInspector]
        public Toggle journeybase_in;
        [HideInInspector]
        public Toggle journeywrong_in;

        [HideInInspector]
        public Dictionary<MiniGameCode, Button> minigamesButtonsDict = new Dictionary<MiniGameCode, Button>();
        [HideInInspector]
        public Dictionary<QuestionBuilderType, Button> qbButtonsDict = new Dictionary<QuestionBuilderType, Button>();

        void Start()
        {
            // Setup for testing
            SetVerboseAI(true);
            ConfigAI.forceJourneyIgnore = false;

            /*
            journey_stage_in.onValueChanged.AddListener(x => { currentJourneyStage = int.Parse(x); });
            journey_learningblock_in.onValueChanged.AddListener(x => { currentJourneyLB = int.Parse(x); });
            journey_playsession_in.onValueChanged.AddListener(x => { currentJourneyPS = int.Parse(x); });

            npacks_in.onValueChanged.AddListener(x => { nPacks = int.Parse(x); });
            ncorrect_in.onValueChanged.AddListener(x => { nCorrectAnswers = int.Parse(x); });
            nwrong_in.onValueChanged.AddListener(x => { nWrongAnswers = int.Parse(x); });

            severity_in.onValueChanged.AddListener(x => { correctSeverity = (SelectionSeverity)x; });
            severitywrong_in.onValueChanged.AddListener(x => { wrongSeverity = (SelectionSeverity)x; });

            history_in.onValueChanged.AddListener(x => { correctHistory = (PackListHistory)x; });
            historywrong_in.onValueChanged.AddListener(x => { wrongHistory = (PackListHistory)x; });

            journeybase_in.onValueChanged.AddListener(x => { journeyEnabledForBase = x; });
            journeywrong_in.onValueChanged.AddListener(x => { journeyEnabledForWrong = x; });
            */

            GlobalUI.ShowPauseMenu(false);
        }

        private void InitialisePlaySession()
        {
            AppManager.I.Player.CurrentJourneyPosition.SetPosition(currentJourneyStage, currentJourneyLB, isAssessment ? 100 : 1);
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
                string packsString = ConfigAI.FormatTeacherHeader("Generated Packs");
                foreach (var pack in packs)
                {
                    packsString += "\n" + pack.ToString();
                }
                ConfigAI.AppendToTeacherReport(packsString);
            }
        }


        #region Testing

        void ApplyParameters()
        {
            ConfigAI.verboseQuestionPacks = verboseQuestionPacks;
            ConfigAI.verboseDataFiltering = verboseDataFiltering;
            ConfigAI.verboseDataSelection = verboseDataSelection;
            ConfigAI.verbosePlaySessionInitialisation = verbosePlaySessionInitialisation;
        }

        [DeMethodButton("Test Everything")]
        public void TestEverything()
        {
            TestAllMiniGames();
            TestAllQuestionBuilders();
        }

        [DeMethodButton("Test Minigames")]
        public void TestAllMiniGames()
        {
            //SetVerboseAI(false);

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

            //SetVerboseAI(true);
        }

        [DeMethodButton("Test QuestionBuilders")]
        public void TestAllQuestionBuilders()
        {
            ApplyParameters();
            //SetVerboseAI(false);

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

            //SetVerboseAI(true);
        }

        #endregion


        #region Minigames

        public IEnumerator SimulateMiniGameCO(MiniGameCode code)
        {
            ApplyParameters();
            ConfigAI.StartTeacherReport();
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
            ConfigAI.PrintTeacherReport();
        }

        public void SimulateMiniGame(MiniGameCode code)
        {
            ApplyParameters();
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
            ApplyParameters();
            ConfigAI.StartTeacherReport();

            var builderParams = SetupFakeGame();
            IQuestionBuilder builder = null;
            switch (builderType)
            {
                case QuestionBuilderType.RandomLetters:
                    builder = new RandomLettersQuestionBuilder(nPacks: nPacks, nCorrect: nCorrectAnswers, nWrong: nWrongAnswers, firstCorrectIsQuestion: true, parameters: builderParams);
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
                    builder = new LettersInWordQuestionBuilder(nPacks: nPacks, nCorrect: nCorrectAnswers, nWrong: nWrongAnswers, useAllCorrectLetters: true, parameters: builderParams);
                    break;
                case QuestionBuilderType.LetterFormsInWords:
                    builder = new LetterFormsInWordsQuestionBuilder(nPacks, 3, parameters: builderParams);
                    break;
                case QuestionBuilderType.CommonLettersInWords:
                    builder = new CommonLettersInWordQuestionBuilder(nPacks: nPacks, nWrong: nWrongAnswers, parameters: builderParams);
                    break;
                case QuestionBuilderType.RandomWords:
                    builder = new RandomWordsQuestionBuilder(nPacks: nPacks, nCorrect: nCorrectAnswers, nWrong: nWrongAnswers, firstCorrectIsQuestion: true, parameters: builderParams);
                    break;
                case QuestionBuilderType.OrderedWords:
                    builder = new OrderedWordsQuestionBuilder(Database.WordDataCategory.NumberOrdinal, parameters: builderParams);
                    break;
                case QuestionBuilderType.WordsWithLetter:
                    builder = new WordsWithLetterQuestionBuilder(nPacks: nPacks, nCorrect: nCorrectAnswers, nWrong: nWrongAnswers, parameters: builderParams);
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
                    builder = new WordsInPhraseQuestionBuilder(nPacks: nPacks, nCorrect: nCorrectAnswers, nWrong: nWrongAnswers, useAllCorrectWords: false, usePhraseAnswersIfFound: true, parameters: builderParams);
                    break;
                case QuestionBuilderType.PhraseQuestions:
                    builder = new PhraseQuestionsQuestionBuilder(nPacks: nPacks, nWrong: nWrongAnswers, parameters: builderParams);
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
