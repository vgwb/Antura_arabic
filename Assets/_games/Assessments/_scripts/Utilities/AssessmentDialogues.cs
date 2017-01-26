using EA4S.Db;
using Kore.Coroutines;
using System.Collections;
using EA4S.MinigamesCommon;
using UnityEngine;

namespace EA4S.Assessment
{
    public class AssessmentDialogues
    {
        public IYieldable PlayStartSound()
        {
            return Dialogue( Localization.Random(
                                        LocalizationDataId.Assessment_Start_1,
                                        LocalizationDataId.Assessment_Start_2,
                                        LocalizationDataId.Assessment_Start_3), true);
        }

        public IYieldable PlayAnturaIsComingSound()
        {
            return Dialogue( Localization.Random(
                                        LocalizationDataId.Assessment_Upset_2,
                                        LocalizationDataId.Assessment_Upset_3), true);
        }

        public IYieldable PlayPushAnturaSound()
        {
            return Dialogue( Localization.Random(
                                        LocalizationDataId.Assessment_Push_Dog_1,
                                        LocalizationDataId.Assessment_Push_Dog_2,
                                        LocalizationDataId.Assessment_Push_Dog_3), true);
        }

        public IYieldable PlayAnturaGoneSound()
        {
            return Dialogue( Localization.Random(
                                        LocalizationDataId.Assessment_Dog_Gone_1,
                                        LocalizationDataId.Assessment_Dog_Gone_2,
                                        LocalizationDataId.Assessment_Dog_Gone_3), true);
        }

        public IYieldable PlayAssessmentCompleteSound()
        {
            return Dialogue( Localization.Random(
                                        LocalizationDataId.Assessment_Complete_1,
                                        LocalizationDataId.Assessment_Complete_2,
                                        LocalizationDataId.Assessment_Complete_3), true);
        }

        public IYieldable PlayAnswerWrong()
        {
            return Speak( Localization.Random(
                                        LocalizationDataId.Assessment_Wrong_1,
                                        LocalizationDataId.Assessment_Wrong_2,
                                        LocalizationDataId.Assessment_Wrong_3));
        }

        public IYieldable PlayGameDescription()
        {
            return Speak( gameDescription);
        }

        /// <summary>
        /// Play audio and show subtitles for a dialogue. You can "yield return it"
        /// </summary>
        /// <param name="ID">Dialogue ID</param>
        /// <returns>Yield instruction to wait it ends</returns>
        public IYieldable Dialogue( LocalizationDataId ID, 
                                    bool showWalkieTalkie)
        {
            return new WaitCoroutine( DialogueCoroutine( ID, showWalkieTalkie, true));
        }

        /// <summary>
        /// Play audio for a dialogue. You can "yield return it"
        /// </summary>
        /// <param name="ID">Dialogue ID</param>
        /// <returns>Yield instruction to wait it ends</returns>
        public IYieldable Speak( LocalizationDataId ID)
        {
            return new WaitCoroutine( DialogueCoroutine( ID, false, false));
        }

        private IAudioManager audioManager;
        private ISubtitlesWidget widget;
        private LocalizationDataId gameDescription;
        private PriorityTikets ticket = new PriorityTikets();

        public AssessmentDialogues( IAudioManager audioManager, 
                                    ISubtitlesWidget widget,
                                    LocalizationDataId gameDescription)
        {
            this.audioManager = audioManager;
            this.widget = widget;
            this.gameDescription = gameDescription;
        }

        /// <summary>
        /// Play LL sound files
        /// </summary>
        /// <param name="data">Sound to play</param>
        public void PlayLetterData( ILivingLetterData data)
        {
            Koroutine.Run( PlayLetterDataCoroutine( data));
        }

        private IEnumerator PlayLetterDataCoroutine( ILivingLetterData data)
        {
            //var audioTicket = ticket.LockLowPriority();

            var audioSource = 
            AssessmentConfiguration.Instance.Context.GetAudioManager()
                    .PlayLetterData( data);

            yield return null;

            /*while (audioSource.IsPlaying && ticket.IsLowPriorityTicketValid(audioTicket))
                yield return null;


            if (audioSource.IsPlaying)
                audioSource.Stop();

            ticket.UnlockLowPriorityTicket( audioTicket);*/
        }


        /// <summary>
        /// Now dialougues are just ignored if there's already some audio playing.
        /// </summary>
        /// <param name="ID"> Statement to play/display</param>
        private IEnumerator DialogueCoroutine( LocalizationDataId ID, bool showWalkieTalkie, bool showSubtitles)
        {
            yield return Wait.For( 0.2f);

            var audioTicket = ticket.LockHighPriority();
            bool playing = false;

            if (ticket.IsHighPriorityTicketValid( audioTicket))
            {
                // Can Play Audio
                playing = true;

                if (showSubtitles)
                    widget.DisplaySentence( ID, 2.2f, showWalkieTalkie);

                if (showWalkieTalkie && showSubtitles) // give time for walkietalkie sound
                    yield return Wait.For( 0.2f);

                audioManager.PlayDialogue( ID, () => { playing = false; });

                while (playing)
                    yield return null;

                if (showSubtitles)
                    widget.Clear();

                yield return Wait.For( 0.2f);
            }

            ticket.UnlockHighPriorityTicket( audioTicket);
        }
    }
}
