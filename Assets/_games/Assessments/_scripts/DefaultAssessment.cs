using System;
using System.Collections;
using UnityEngine;

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

        public IEnumerator PlayCoroutine( Action gameEndedCallback)
        {
            yield return TimeEngine.Wait(0.7f);

            for (int round = 0; round< AssessmentConfiguration.Rounds; round++)
            {
                QuestionGenerator.InitRound();

                for(int question = 0; question<AssessmentConfiguration.SimultaneosQuestions; question++)

                    LogicInjector.Wire( 
                        QuestionGenerator.GetNextQuestion(),                 
                        QuestionGenerator.GetNextAnswers()      );

                LogicInjector.CompleteWiring();
                
                QuestionGenerator.CompleteRound();

                QuestionPlacer.Place( QuestionGenerator.GetAllQuestions());
                while (QuestionPlacer.IsAnimating())
                    yield return null;

                AnswerPlacer.Place( QuestionGenerator.GetAllAnswers());
                while (AnswerPlacer.IsAnimating())
                    yield return null;

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
