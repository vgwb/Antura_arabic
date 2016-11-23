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
                                    ISubtitlesWidget subtitles,
                                    Db.LocalizationDataId gameDescription)
        {
            AnswerPlacer = answ_placer;
            QuestionGenerator = question_generator;
            QuestionPlacer = question_placer;
            LogicInjector = logic_injector;
            AssessmentConfiguration = game_conf;
            GameContext = game_context;
            AudioManager = audio_manager;
            Subtitles = subtitles;
            GameDescription = gameDescription;
        }

        #region AUDIO

        private void Dialogue( Db.LocalizationDataId ID)
        {
            isPlayingAudio = true;
            Coroutine.Start( PlayDialogueCoroutine( ID));
        }

        private bool IsAudioPlaying()
        {
            return isPlayingAudio;
        }

        bool isPlayingAudio = false;

        private void StoppedPlaying()
        {
            isPlayingAudio = false;
        }
        IEnumerator PlayDialogueCoroutine( Db.LocalizationDataId ID)
        {
            Subtitles.DisplaySentence( ID, 1, false);
            AudioManager.PlayDialogue( ID, () => StoppedPlaying() );

            while (isPlayingAudio)
                yield return null;

            // Clear dialog before 10 seconds
            Subtitles.Clear();
        }

        private void PlayStartSound()
        {
            Dialogue(Localization.Random(Db.LocalizationDataId.Assessment_Start_1,
                                        Db.LocalizationDataId.Assessment_Start_2,
                                        Db.LocalizationDataId.Assessment_Start_3));
        }

        private void PlayAnturaIsComingSound()
        {
            Dialogue(Localization.Random(Db.LocalizationDataId.Assessment_Upset_2,
                                        Db.LocalizationDataId.Assessment_Upset_3));
        }

        private void PlayPushAnturaSound()
        {
            Dialogue(Localization.Random(Db.LocalizationDataId.Assessment_Push_Dog_1,
                                        Db.LocalizationDataId.Assessment_Push_Dog_2,
                                        Db.LocalizationDataId.Assessment_Push_Dog_3));
        }

        private void PlayAnturaGoneSound()
        {
            Dialogue(Localization.Random(Db.LocalizationDataId.Assessment_Dog_Gone_1,
                                        Db.LocalizationDataId.Assessment_Dog_Gone_2,
                                        Db.LocalizationDataId.Assessment_Dog_Gone_3));
        }

        private void PlayAssessmentCompleteSound()
        {
            Dialogue(Localization.Random(Db.LocalizationDataId.Assessment_Complete_1,
                                        Db.LocalizationDataId.Assessment_Complete_2,
                                        Db.LocalizationDataId.Assessment_Complete_3));
        }

        private void PlayAssessmentWrongSound()
        {
            Dialogue(Localization.Random(Db.LocalizationDataId.Assessment_Wrong_1,
                                        Db.LocalizationDataId.Assessment_Wrong_2,
                                        Db.LocalizationDataId.Assessment_Wrong_3));
        }

        private void PlayGameDescription()
        {
            Dialogue( GameDescription);
        }

        #endregion

        public IEnumerator PlayCoroutine( Action gameEndedCallback)
        {
            PlayStartSound();
            while(IsAudioPlaying())
                yield return null;

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

                    PlayAnturaIsComingSound();
                    var anturaController = AnturaFactory.Instance.SleepingAntura();

                    anturaController.StartAnimation( ()=> PlayPushAnturaSound());

                    while (anturaController.IsAnimating() || IsAudioPlaying())
                        yield return null;

                    PlayAnturaGoneSound();

                    while (IsAudioPlaying())
                        yield return null;

                    yield return TimeEngine.Wait( 0.3f);

                    PlayGameDescription();

                    while (IsAudioPlaying())
                        yield return null;

                    yield return TimeEngine.Wait(0.3f);
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

        public Db.LocalizationDataId GameDescription { get; private set; }
    }
}
