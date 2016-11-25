using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class DefaultAnswerChecker : IAnswerChecker
    {
        private ICheckmarkWidget checkmarkWidget;
        private IAudioManager audioManager;
        private IDialogueManager dialogueManager;

        public DefaultAnswerChecker(    ICheckmarkWidget checkmarkWidget, 
                                        IAudioManager audioManager, 
                                        IDialogueManager dialogueManager)
        {
            this.checkmarkWidget = checkmarkWidget;
            this.audioManager = audioManager;
            this.dialogueManager = dialogueManager;
        }

        private bool isAnimating = false;
        private bool allCorrect = false;

        public bool AllCorrect()
        {
            if (coroutineEnded)  // Needed to see All Correct only when animation ended
            {
                isAnimating = false;
                return allCorrect;
            }

            return false;
        }

        YieldInstruction PlayAnswerWrong()
        {
            return dialogueManager.Dialogue( Localization.Random(
                                            Db.LocalizationDataId.Assessment_Wrong_1,
                                            Db.LocalizationDataId.Assessment_Wrong_2,
                                            Db.LocalizationDataId.Assessment_Wrong_3 ));
        }


        // Problem => not called if all answers placed but not correct.. why?
        public void Check( List< PlaceholderBehaviour> placeholders, IDragManager dragManager)
        {
            isAnimating = true;
            coroutineEnded = false;
            allCorrect = false;
            Coroutine.Start( CheckCoroutine( placeholders, dragManager));
        }

        private bool coroutineEnded = false;
        private IEnumerator CheckCoroutine( List< PlaceholderBehaviour> placeholders, IDragManager dragManager)
        {
            dragManager.DisableInput();

            bool areAllCorrect = true;
            foreach (var p in placeholders)
            {
                var droppa = p.LinkedDroppable.gameObject.GetComponent< AnswerBehaviour>();
                var place = p.Placeholder;
                place.LinkAnswer( droppa.GetAnswer().GetAnswerSet());
                if (place.IsAnswerCorrect() == false)
                {
                    // Wrong answers are detached immediatly
                    areAllCorrect = false;
                    p.LinkedDroppable.Detach();
                    p.Placeholder.LinkAnswer( 0);
                }
                else
                {
                    var behaviour =
                    p.Placeholder.GetQuestion().gameObject
                        .GetComponent< QuestionBehaviour>();
                    behaviour.OnQuestionAnswered();

                    // why the heck was I waiting here? Because LetterShape have to show letters
                    // So ok to wait. But only for LetterShape! Infact that is already implemented _-_
                    yield return TimeEngine.Wait( behaviour.TimeToWait());
                }
            }

            allCorrect = areAllCorrect;

            while (wrongAnswerAnimationPlaying)
                yield return null; // wait only if previous message has not finished

            if (allCorrect)
            {
                audioManager.PlaySound( Sfx.StampOK);
                yield return TimeEngine.Wait( 0.4f);
                checkmarkWidget.Show( true);
                yield return TimeEngine.Wait( 1.0f);
            }
            else
            {
                wrongAnswerAnimationPlaying = true;
                Coroutine.Start( WrongAnswerCoroutine());
            }
            
            coroutineEnded = true;
            dragManager.EnableInput();
        }

        private bool wrongAnswerAnimationPlaying = false;

        private IEnumerator WrongAnswerCoroutine()
        {
            checkmarkWidget.Show( false);
            audioManager.PlaySound( Sfx.KO);
            yield return PlayAnswerWrong();
            wrongAnswerAnimationPlaying = false;
        }

        private bool WrongAnswerAnimationPlaying()
        {
            return wrongAnswerAnimationPlaying;
        }

        public bool IsAnimating()
        {
            return isAnimating;
        }
    }
}
