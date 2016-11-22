using System;
using System.Collections;

namespace EA4S.Assessment
{
    public class DefaultAssessment : IAssessment
    {
        public DefaultAssessment(   IAnswerPlacer answ_placer,
                                    IQuestionPlacer question_placer,
                                    IQuestionGenerator question_generator,
                                    ILogicInjector logic_injector,
                                    IAssessmentConfiguration game_conf,
                                    IGameContext game_context)
        {
            AnswerPlacer = answ_placer;
            QuestionGenerator = question_generator;
            QuestionPlacer = question_placer;
            LogicInjector = logic_injector;
            AssessmentConfiguration = game_conf;
            GameContext = game_context;
        }

        private void PlayStartSound()
        {
           // GameContext.GetAudioManager().PlayDialogue( "Assessment_Start_1");
        }

        public IEnumerator PlayCoroutine( Action gameEndedCallback)
        {
            yield return TimeEngine.Wait( 0.7f);
            bool AnturaShowed = false;

            for (int round = 0; round< AssessmentConfiguration.Rounds; round++)
            {
                #region Init
                QuestionGenerator.InitRound();

                for(int question = 0; question<AssessmentConfiguration.SimultaneosQuestions; question++)

                    LogicInjector.Wire( 
                        QuestionGenerator.GetNextQuestion(),                 
                        QuestionGenerator.GetNextAnswers()      );

                LogicInjector.CompleteWiring();
                
                QuestionGenerator.CompleteRound();
                #endregion

                if (AnturaShowed)
                {
                    // Show question only after antura animation is done.
                    QuestionPlacer.Place( QuestionGenerator.GetAllQuestions());
                    while ( QuestionPlacer.IsAnimating())
                        yield return null;
                }

                AnswerPlacer.Place( QuestionGenerator.GetAllAnswers());
                while (AnswerPlacer.IsAnimating())
                    yield return null;

                if (AnturaShowed == false)
                {
                    #region ANTURA ANIMATION

                    var anturaController = AnturaFactory.Instance.SleepingAntura();

                    bool anturaIsGone = false;
                    anturaController.StartAnimation( () => anturaIsGone = true);

                    #endregion

                    QuestionPlacer.Place( QuestionGenerator.GetAllQuestions());
                    while ( QuestionPlacer.IsAnimating())
                        yield return null;

                    AnturaShowed = true;
                }

                #region GamePlay
                //////////////////////////////
                //// GAME LOGIC (WIP)
                ////----
                LogicInjector.EnableGamePlay();

                while (LogicInjector.AllAnswersCorrect() == false)
                    yield return null;
                ////___
                //// GAME LOGIC END
                //////////////////////////////

                QuestionPlacer.RemoveQuestions();
                AnswerPlacer.RemoveAnswers();

                while (QuestionPlacer.IsAnimating() || AnswerPlacer.IsAnimating())
                    yield return null;

                LogicInjector.ResetRound();
                #endregion
            }

            gameEndedCallback();
        }

        public IAnswerPlacer AnswerPlacer { get; private set; }

        public IQuestionGenerator QuestionGenerator { get; private set; }

        public ILogicInjector LogicInjector { get; private set; }

        public IQuestionPlacer QuestionPlacer { get; private set; }

        public IAssessmentConfiguration AssessmentConfiguration { get; private set; }

        public IGameContext GameContext { get; private set; }
    }
}
