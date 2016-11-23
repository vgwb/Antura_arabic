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
                                    IGameContext game_context,
                                    IAudioManager audio_manager,
                                    ISubtitlesWidget subtitles)
        {
            AnswerPlacer = answ_placer;
            QuestionGenerator = question_generator;
            QuestionPlacer = question_placer;
            LogicInjector = logic_injector;
            AssessmentConfiguration = game_conf;
            GameContext = game_context;
            AudioManager = audio_manager;
            Subtitles = subtitles;
        }

        #region AUDIO

        private void Dialogue(Db.LocalizationDataId ID)
        {
            Coroutine.Start( PlayDialogueCoroutine( ID));
        }

        bool isPlayingAudio = false;
        IEnumerator PlayDialogueCoroutine(Db.LocalizationDataId ID)
        {
            while (isPlayingAudio)
                yield return null;

            isPlayingAudio = true;
            AudioManager.PlayDialogue( ID);
            Subtitles.DisplaySentence( ID, 2, false, ()=> isPlayingAudio = false);

            while (isPlayingAudio)
                yield return null;
        }

        private void PlayStartSound()
        {
            AudioManager.PlayDialogue( RandomHelper.GetRandomParams( Db.LocalizationDataId.Assessment_Start_1,
                                                      Db.LocalizationDataId.Assessment_Start_2,
                                                      Db.LocalizationDataId.Assessment_Start_3));
        }

        private void PlayAnturaIsComingSound()
        {
            AudioManager.PlayDialogue(RandomHelper.GetRandomParams(Db.LocalizationDataId.Assessment_Upset_1,
                                                      Db.LocalizationDataId.Assessment_Upset_2,
                                                      Db.LocalizationDataId.Assessment_Upset_3));
        }

        private void PlayPushAnturaSound()
        {
            AudioManager.PlayDialogue(RandomHelper.GetRandomParams(Db.LocalizationDataId.Assessment_Push_Dog_1,
                                                      Db.LocalizationDataId.Assessment_Push_Dog_2,
                                                      Db.LocalizationDataId.Assessment_Push_Dog_3));
        }
        #endregion

        public IEnumerator PlayCoroutine( Action gameEndedCallback)
        {
            //PlayStartSound();
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
                    /*yield return TimeEngine.Wait( 1.7f);
                    var anturaController = AnturaFactory.Instance.SleepingAntura();

                    bool anturaIsGone = false;
                    anturaController.StartAnimation( () => anturaIsGone = true);*/

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

        public IAudioManager AudioManager { get; private set; }

        public ISubtitlesWidget Subtitles { get; private set; }
    }
}
