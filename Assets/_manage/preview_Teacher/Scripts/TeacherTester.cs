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
        public Dropdown history_in;
        public Toggle baseignorejourney_in;
        public Toggle wrongignorejourney_in;

        void Start()
        {
            // Setup for testing
            ConfigAI.verboseDataSelection = true;
            ConfigAI.verboseTeacher = true;
            ConfigAI.forceJourneyIgnore = false;

            journey_stage_in.onValueChanged.AddListener(x => { currentJourneyStage = int.Parse(x); });
            journey_learningblock_in.onValueChanged.AddListener(x => { currentJourneyLB = int.Parse(x); });
            journey_playsession_in.onValueChanged.AddListener(x => { currentJourneyPS = int.Parse(x);  });

            npacks_in.onValueChanged.AddListener(x => { nPacks = int.Parse(x); });
            ncorrect_in.onValueChanged.AddListener(x => { nCorrect = int.Parse(x); });
            nwrong_in.onValueChanged.AddListener(x => { nWrong = int.Parse(x); });

            severity_in.onValueChanged.AddListener(x => { selectionSeverity = (SelectionSeverity)x; });
            history_in.onValueChanged.AddListener(x => { questionHistory = (PackListHistory)x; });

            baseignorejourney_in.onValueChanged.AddListener(x => { baseIgnoreJourney = x; });
            wrongignorejourney_in.onValueChanged.AddListener(x => { wrongIgnoreJourney = x; });
        }

        int currentJourneyStage = 1;
        int currentJourneyLB = 1;
        int currentJourneyPS = 1;
        int nPacks = 5;
        int nCorrect = 1;
        int nWrong = 1;
        SelectionSeverity selectionSeverity;
        PackListHistory questionHistory;
        bool baseIgnoreJourney = false;
        bool wrongIgnoreJourney = false;

        #region  Question Builder testing

        void SetupFakeGame()
        {
            AppManager.Instance.Player.CurrentJourneyPosition.Stage = currentJourneyStage;
            AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = currentJourneyLB;
            AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = currentJourneyPS;
            AppManager.Instance.Teacher.InitialiseCurrentPlaySession();
        }

        public void RandomLettersTest()
        {
            SetupFakeGame();
            var builder = new RandomLettersQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, 
                firstCorrectIsQuestion: true, wrongIgnoreJourney: wrongIgnoreJourney, correctChoicesHistory:questionHistory, wrongChoicesHistory:questionHistory);
            builder.CreateAllQuestionPacks();
        }

        public void AlphabetTest()
        {
            SetupFakeGame();
            var builder = new AlphabetQuestionBuilder(ignoreJourney: baseIgnoreJourney);
            builder.CreateAllQuestionPacks();
        }

        public void LettersBySunMoonTest()
        {
            SetupFakeGame();
            var builder = new LettersBySunMoonQuestionBuilder(nPacks: nPacks, severity:selectionSeverity);
            builder.CreateAllQuestionPacks();
        }

        public void LettersByTypeTest()
        {
            SetupFakeGame();
            var builder = new LettersByTypeQuestionBuilder(nPacks: nPacks, severity: selectionSeverity);
            builder.CreateAllQuestionPacks();
        }

        public void LettersInWordTest()
        {
            SetupFakeGame();
            var builder = new LettersInWordQuestionBuilder(nPacks: nPacks, nCorrect:nCorrect, nWrong:nWrong, useAllCorrectLetters:true);
            builder.CreateAllQuestionPacks();
        }

        public void CommonLettersInWordTest()
        {
            SetupFakeGame();
            var builder = new CommonLettersInWordQuestionBuilder(nPacks: nPacks);
            builder.CreateAllQuestionPacks();
        }


        public void RandomWordsTest()
        {
            SetupFakeGame();
            var builder = new RandomWordsQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, firstCorrectIsQuestion: true);
            builder.CreateAllQuestionPacks();
        }

        public void OrderedWordsTest()
        {
            SetupFakeGame();
            var builder = new OrderedWordsQuestionBuilder(Db.WordDataCategory.NumberOrdinal, selectionSeverity);
            builder.CreateAllQuestionPacks();
        }
        
        public void WordsWithLetterTest()
        {
            SetupFakeGame();
            var builder = new WordsWithLetterQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong);
            builder.CreateAllQuestionPacks();
        }

        public void WordsByFormTest()
        {
            SetupFakeGame();
            var builder = new WordsByFormQuestionBuilder(nPacks: nPacks, severity: selectionSeverity);
            builder.CreateAllQuestionPacks();
        }

        public void WordsByArticleTest()
        {
            SetupFakeGame();
            var builder = new WordsByArticleQuestionBuilder(nPacks: nPacks, severity: selectionSeverity);
            builder.CreateAllQuestionPacks();
        }

        public void WordsBySunMoonTest()
        {
            SetupFakeGame();
            var builder = new WordsBySunMoonQuestionBuilder(nPacks: nPacks, severity: selectionSeverity);
            builder.CreateAllQuestionPacks();
        }

        public void WordsInPhraseTest()
        {
            SetupFakeGame();
            var builder = new WordsInPhraseQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong, useAllCorrectWords: false, usePhraseAnswersIfFound: true, questionHistory: questionHistory);
            builder.CreateAllQuestionPacks();
        }

        public void PhraseQuestions()
        {
            SetupFakeGame();
            var builder = new PhraseQuestionsQuestionBuilder(nPacks: nPacks, nCorrect: nCorrect, nWrong: nWrong);
            builder.CreateAllQuestionPacks();
        }

        #endregion
    }

}
