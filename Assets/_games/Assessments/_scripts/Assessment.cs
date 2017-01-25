using Kore.Coroutines;
using Kore.Utils;
using System.Collections;
using EA4S.MinigamesCommon;
using UnityEngine;

namespace EA4S.Assessment
{
    public class Assessment
    {
        public Assessment(  IAnswerPlacer answ_placer,
                            IQuestionPlacer question_placer,
                            IQuestionGenerator question_generator,
                            ILogicInjector logic_injector,
                            IAssessmentConfiguration game_conf,
                            IGameContext game_context,
                            AssessmentDialogues dialogues)
        {
            AnswerPlacer = answ_placer;
            QuestionGenerator = question_generator;
            QuestionPlacer = question_placer;
            LogicInjector = logic_injector;
            Configuration = game_conf;
            GameContext = game_context;
            Dialogues = dialogues;
        }

        public void StartGameSession( KoreCallback gameEndedCallback)
        {
            Koroutine.Run( RoundsCoroutine( gameEndedCallback));
        }

        private IEnumerator RoundsCoroutine( KoreCallback gameEndedCallback)
        {
            int anturaGagRound = Random.Range( 1, Configuration.Rounds);

            for (int round = 0; round < Configuration.Rounds; round++)
            {
                // Show antura only on the elected round.
                if (anturaGagRound == round)
                    yield return Koroutine.Nested( AnturaGag());

                InitRound();

                yield return Koroutine.Nested( RoundBegin());
                yield return Koroutine.Nested( PlaceAnswers());

                bool playQuestion = AssessmentOptions.Instance.PlayQuestionAlsoAfterTutorial;

                IYieldable audio = null;
                if(round == 0)
                    audio = new WaitCoroutine( DescriptionAudio( playQuestion));

                yield return Koroutine.Nested( GamePlay());
                
                if(audio != null)
                    yield return audio;

                yield return Koroutine.Nested( ClearRound());
            }

            gameEndedCallback();
        }

        private IEnumerator DescriptionAudio( bool playQuestion)
        {
            yield return Dialogues.PlayGameDescription();

            if(playQuestion)
                yield return QuestionPlacer.PlayQuestionSound();
        }

        private IEnumerator AnturaGag()
        {
            yield return null;
        }

        private IEnumerator RoundBegin()
        {
            bool playSound = AssessmentOptions.Instance.QuestionSpawnedPlaySound;
            yield return Koroutine.Nested( PlaceQuestions( playSound));
        }

        private IEnumerator PlaceQuestions( bool playAudio = false)
        {
            // It is possible that in definite game release the boolean flag will be always
            // false. In that case we can simplify the code later.
            QuestionPlacer.Place( QuestionGenerator.GetAllQuestions(), playAudio);
            while ( QuestionPlacer.IsAnimating())
                yield return null;
        }

        private IEnumerator PlaceAnswers()
        {
            AnswerPlacer.Place( QuestionGenerator.GetAllAnswers());
            while ( AnswerPlacer.IsAnimating())
                yield return null;

            LogicInjector.AnswersAdded();
        }

        private IEnumerator GamePlay()
        {
            LogicInjector.EnableGamePlay();

            while (LogicInjector.AllAnswersCorrect() == false)
                yield return null;
        }

        private IEnumerator ClearRound()
        {
            LogicInjector.RemoveDraggables();

            QuestionPlacer.RemoveQuestions();
            AnswerPlacer.RemoveAnswers();

            while (QuestionPlacer.IsAnimating() || AnswerPlacer.IsAnimating())
                yield return null;

            LogicInjector.ResetRound();
        }

        private void InitRound()
        {
            QuestionGenerator.InitRound();

            for (int question = 0; question < Configuration.SimultaneosQuestions; question++)
                WireLogicInjector( LogicInjector, QuestionGenerator);

            LogicInjector.CompleteWiring();
            LogicInjector.EnableDragOnly();

            QuestionGenerator.CompleteRound();
        }

        private void WireLogicInjector( ILogicInjector injector, IQuestionGenerator generator)
        {
            try
            {
                IQuestion question = generator.GetNextQuestion();
                Answer[] answers = generator.GetNextAnswers();

                injector.Wire( question, answers);
            }
            catch ( System.Exception ex)
            {
                Debug.LogWarning( "Not enough teacher data to generate an additional question at this stage: " +
                    ex.Message);
            }
        }

        public IAnswerPlacer AnswerPlacer { get; private set; }

        public IQuestionGenerator QuestionGenerator { get; private set; }

        public ILogicInjector LogicInjector { get; private set; }

        public IQuestionPlacer QuestionPlacer { get; private set; }

        public IAssessmentConfiguration Configuration { get; private set; }

        public IGameContext GameContext { get; private set; }

        public AssessmentDialogues Dialogues { get; private set; }
    }
}
