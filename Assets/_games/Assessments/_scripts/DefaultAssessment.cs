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
                                    IGameContext game_context,
                                    IDialogueManager dialogues,
                                    Db.LocalizationDataId gameDescription)
        {
            AnswerPlacer = answ_placer;
            QuestionGenerator = question_generator;
            QuestionPlacer = question_placer;
            LogicInjector = logic_injector;
            Configuration = game_conf;
            GameContext = game_context;
            Dialogues = dialogues;
            GameDescription = gameDescription;
        }

        #region AUDIO
        private YieldInstruction PlayStartSound()
        {
            return Dialogues.Dialogue(Localization.Random(
                                        Db.LocalizationDataId.Assessment_Start_1,
                                        Db.LocalizationDataId.Assessment_Start_2,
                                        Db.LocalizationDataId.Assessment_Start_3), true);
        }

        private YieldInstruction PlayAnturaIsComingSound()
        {
            return Dialogues.Dialogue( Localization.Random(
                                        Db.LocalizationDataId.Assessment_Upset_2,
                                        Db.LocalizationDataId.Assessment_Upset_3), true);
        }

        private YieldInstruction PlayPushAnturaSound()
        {
            return Dialogues.Dialogue( Localization.Random(
                                        Db.LocalizationDataId.Assessment_Push_Dog_1,
                                        Db.LocalizationDataId.Assessment_Push_Dog_2,
                                        Db.LocalizationDataId.Assessment_Push_Dog_3), true);
        }

        private YieldInstruction PlayAnturaGoneSound()
        {
            return Dialogues.Dialogue( Localization.Random(
                                        Db.LocalizationDataId.Assessment_Dog_Gone_1,
                                        Db.LocalizationDataId.Assessment_Dog_Gone_2,
                                        Db.LocalizationDataId.Assessment_Dog_Gone_3), true);
        }

        private YieldInstruction PlayAssessmentCompleteSound()
        {
            return Dialogues.Dialogue( Localization.Random(
                                        Db.LocalizationDataId.Assessment_Complete_1,
                                        Db.LocalizationDataId.Assessment_Complete_2,
                                        Db.LocalizationDataId.Assessment_Complete_3), true);
        }

        private YieldInstruction PlayGameDescription()
        {
            return Dialogues.Speak( GameDescription);
        }

        #endregion

        public IEnumerator PlayCoroutine( Action gameEndedCallback)
        {
            yield return PlayStartSound();

            bool AnturaShowed = false;

            for (int round = 0; round< Configuration.Rounds; round++)
            {
                #region Init
                QuestionGenerator.InitRound();

                for(int question = 0; question<Configuration.SimultaneosQuestions; question++)

                    LogicInjector.Wire( 
                        QuestionGenerator.GetNextQuestion(),                 
                        QuestionGenerator.GetNextAnswers()      );

                LogicInjector.CompleteWiring();
                LogicInjector.EnableDragOnly(); //as by new requirments

                //mute feedback audio while speaker is speaking
                bool answerConfigurationCache = AssessmentConfiguration.Instance.PronunceAnswerWhenClicked;
                AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = false;

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

                LogicInjector.AnswersAdded();

                if (AnturaShowed == false)
                {
                    #region ANTURA ANIMATION

                    PlayAnturaIsComingSound();
                    var anturaController = AnturaFactory.Instance.SleepingAntura();

                    anturaController.StartAnimation( ()=> PlayPushAnturaSound(), ()=>PlayAnturaGoneSound());

                    while (anturaController.IsAnimating())
                        yield return null;

                    yield return TimeEngine.Wait( 0.3f);
                    yield return PlayGameDescription();
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
                // Restore audio when playing
                AssessmentConfiguration.Instance.PronunceAnswerWhenClicked = answerConfigurationCache;
                LogicInjector.EnableGamePlay();

                while (LogicInjector.AllAnswersCorrect() == false)
                    yield return null;
                ////___
                //// GAME LOGIC END
                //////////////////////////////

                LogicInjector.RemoveDraggables();

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

        public IAssessmentConfiguration Configuration { get; private set; }

        public IGameContext GameContext { get; private set; }

        public IDialogueManager Dialogues { get; private set; }

        public Db.LocalizationDataId GameDescription { get; private set; }
    }
}
