using System;
using System.Collections;
using EA4S.Db;
using UnityEngine;
using Kore.Coroutines;

namespace EA4S.Assessment
{
    public class DialogueManager : IDialogueManager
    {
        IAudioManager audioManager;
        ISubtitlesWidget widget;
        private bool isPlayingAudio;

        public DialogueManager( IAudioManager audioManager, ISubtitlesWidget widget)
        {
            this.audioManager = audioManager;
            this.widget = widget;
            isPlayingAudio = false;
        }

        public IYieldable Dialogue( LocalizationDataId ID, bool showWalkieTalkie)
        {
            return new WaitCoroutine( DialogueCoroutine( ID, showWalkieTalkie, true));
        }

        private IEnumerator DialogueCoroutine( LocalizationDataId ID, bool showWalkieTalkie, bool showSubtitles)
        {
            while (IsPlayingAudio())
                yield return null;
            isPlayingAudio = true;

            yield return Wait.For( 0.2f);

            if (showSubtitles)
                widget.DisplaySentence( ID, 2.2f, showWalkieTalkie);

            if(showWalkieTalkie && showSubtitles) // give time for walkietalkie sound
                yield return Wait.For( 0.2f);

            audioManager.PlayDialogue( ID, () => OnStopPlaying());

            while (IsPlayingAudio())
                yield return null;

            if(showSubtitles)
                widget.Clear();

            yield return Wait.For( 0.2f);
        }

        private void OnStopPlaying()
        {
            isPlayingAudio = false;
        }

        private bool IsPlayingAudio()
        {
            return isPlayingAudio;
        }

        public IYieldable Speak( LocalizationDataId ID)
        {
            return new WaitCoroutine( DialogueCoroutine( ID, false, false));
        }
    }
}
