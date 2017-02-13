using System;
using System.Collections;
using System.Collections.Generic;
using DG.DeInspektor.Attributes;
using EA4S.UI;
using Kore.Coroutines;
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

        [Range(0, 10)]
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
                for (int i = 0; i < packs.Count; i++)
                {
                    packsString += "\n" + (i+1) + ": " + packs[i].ToString();
                }
                ConfigAI.AppendToTeacherReport(packsString);
            }
        }


        #region Testing API

        void ApplyParameters()
        {
            ConfigAI.verboseQuestionPacks = verboseQuestionPacks;
            ConfigAI.verboseDataFiltering = verboseDataFiltering;
            ConfigAI.verboseDataSelection = verboseDataSelection;
            ConfigAI.verbosePlaySessionInitialisation = verbosePlaySessionInitialisation;
        }

        [DeMethodButton("Test Minimum Journey")]
        public void DoTestMinimumJourney()
        {
            // @todo:
            //DoTestAllMiniGames();
            //DoTestAllQuestionBuilders();
        }

        [DeMethodButton("Test Everything")]
        public void DoTestEverything()
        {
            StartCoroutine(DoTest(() => DoTestEverythingCO()));
        }
        private IEnumerator DoTestEverythingCO()
        { 
            yield return StartCoroutine(DoTestAllMiniGamesCO());
            yield return StartCoroutine(DoTestAllQuestionBuildersCO());
        }

        [DeMethodButton("Test Minigames")]
        public void DoTestAllMiniGames()
        {
            StartCoroutine(DoTest(() => DoTestAllMiniGamesCO()));
        }
        private IEnumerator DoTestAllMiniGamesCO()
        {
            foreach (var code in Helpers.GenericHelper.SortEnums<MiniGameCode>())
            {
                if (code == MiniGameCode.Invalid) continue;
                if (code == MiniGameCode.Assessment_VowelOrConsonant) continue;
                yield return StartCoroutine(DoTestMinigameCO(code));
            }
        }

        [DeMethodButton("Test QuestionBuilders")]
        public void DoTestAllQuestionBuilders()
        {
            StartCoroutine(DoTest(() => DoTestAllQuestionBuildersCO()));
        }
        private IEnumerator DoTestAllQuestionBuildersCO()
        {
            foreach (var type in Helpers.GenericHelper.SortEnums<QuestionBuilderType>())
            {
                yield return StartCoroutine(DoTestQuestionBuilderCO(type));
            }
        }

        public void DoTestQuestionBuilder(QuestionBuilderType type)
        {
            StartCoroutine(DoTest(() => DoTestQuestionBuilderCO(type)));
        }
        private IEnumerator DoTestQuestionBuilderCO(QuestionBuilderType type)
        {
            SetButtonStatus(qbButtonsDict[type], Color.yellow);
            yield return new WaitForSeconds(0.1f);
            var statusColor = Color.green;
            try
            {
                SimulateQuestionBuilder(type);
            }
            catch (Exception e)
            {
                Debug.LogError("!! " + type + "\n " + e.Message);
                statusColor = Color.red;
            }
            SetButtonStatus(qbButtonsDict[type], statusColor);
            yield return null;
        }

        public void DoTestMinigame(MiniGameCode code)
        {
            StartCoroutine(DoTest(() => DoTestMinigameCO(code)));
        }
        private IEnumerator DoTestMinigameCO(MiniGameCode code)
        {
            SetButtonStatus(minigamesButtonsDict[code], Color.yellow);
            yield return new WaitForSeconds(0.1f);
            var statusColor = Color.green; 
            try
            {
                SimulateMiniGame(code);
            }
            catch (Exception e)
            {
                Debug.LogError("!! " + code + "\n " + e.Message);
                statusColor = Color.red;
            }
            SetButtonStatus(minigamesButtonsDict[code], statusColor);
            yield return null;
        }

        private void SetButtonStatus(Button button, Color statusColor)
        {
            var colors = button.colors;
            colors.normalColor = statusColor;
            colors.highlightedColor = statusColor * 1.2f;
            button.colors = colors;
        }
        private IEnumerator DoTest(Func<IEnumerator> CoroutineFunc)
        {
            ConfigAI.StartTeacherReport();
            ApplyParameters();
            InitialisePlaySession();
            for (int i = 1; i <= numberOfSimulations; i++)
            {
                ConfigAI.AppendToTeacherReport("************ Simulation " + i + " ************");
                yield return StartCoroutine(CoroutineFunc());
            }
            ConfigAI.PrintTeacherReport();
        }

        #endregion


        #region Minigames Simulation

        private void SimulateMiniGame(MiniGameCode code)
        {
            var config = AppManager.I.GameLauncher.ConfigureMiniGame(code, System.DateTime.Now.Ticks.ToString());
            //InitialisePlaySession();
            var builder = config.SetupBuilder();
            ConfigAI.AppendToTeacherReport("** Minigame " + code + " - " + builder.GetType().Name);
            var packs = builder.CreateAllQuestionPacks();
            ReportPacks(packs);
        }

        #endregion

        #region  QuestionBuilder Simulation

        private void SimulateQuestionBuilder(QuestionBuilderType builderType)
        {
            var builderParams = SetupBuilderParameters();
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
                    builder = new LettersInWordQuestionBuilder(nPacks: nPacks, nCorrect: nCorrectAnswers, nWrong: nWrongAnswers, useAllCorrectLetters: true, packsUsedTogether: true, parameters: builderParams);
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
                    builder = new WordsWithLetterQuestionBuilder(nPacks: nPacks, nCorrect: nCorrectAnswers, nWrong: nWrongAnswers, packsUsedTogether: true, parameters: builderParams);
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
        }

        QuestionBuilderParameters SetupBuilderParameters()
        {
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
