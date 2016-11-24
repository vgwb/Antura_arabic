using System.Collections;
using EA4S.Db;
using UnityEngine;

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

        public YieldInstruction Dialogue( LocalizationDataId ID, bool showWalkieTalkie)
        {
            return Coroutine.Start( DialogueCoroutine( ID, showWalkieTalkie));
        }

        private IEnumerator DialogueCoroutine( LocalizationDataId ID, bool showWalkieTalkie)
        {
            while (IsPlayingAudio())
                yield return null;
            isPlayingAudio = true;

            yield return TimeEngine.Wait( 0.2f);

            //TODO: speaker not implemented (does it overlaps switch sound to localized audio?)
            widget.DisplaySentence( ID, 2.2f, false);
            audioManager.PlayDialogue( ID, () => OnStopPlaying());

            while (IsPlayingAudio())
                yield return null;

            widget.Clear();
            yield return TimeEngine.Wait( 0.2f);
        }

        private void OnStopPlaying()
        {
            isPlayingAudio = false;
        }

        private bool IsPlayingAudio()
        {
            return isPlayingAudio;
        }
    }
}
