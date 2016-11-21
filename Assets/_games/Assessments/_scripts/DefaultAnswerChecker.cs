using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class DefaultAnswerChecker : IAnswerChecker
    {
        private ICheckmarkWidget checkmarkWidget;
        private IAudioManager audioManager;

        public DefaultAnswerChecker( ICheckmarkWidget checkmarkWidget, IAudioManager audioManager)
        {
            this.checkmarkWidget = checkmarkWidget;
            this.audioManager = audioManager;
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
            Debug.Log("CheckCoroutine");
            dragManager.DisableInput();

            bool areAllCorrect = true;
            foreach (var p in placeholders)
            {
                var droppa = p.LinkedDroppable.gameObject.GetComponent< AnswerBehaviour>();
                var place = p.Placeholder;
                place.LinkAnswer( droppa.GetAnswer().GetAnswerSet());
                if (place.IsAnswerCorrect() == false)
                {
                    // NEED TO FIND QUESTION HERE
                    areAllCorrect = false;
                    p.LinkedDroppable.Detach();
                    p.Placeholder.LinkAnswer(0);
                }
                else
                {
                    var behaviour =
                    p.Placeholder.GetQuestion().gameObject
                        .GetComponent< QuestionBehaviour>();
                    behaviour.OnQuestionAnswered();
                    yield return TimeEngine.Wait(behaviour.TimeToWait());
                }
            }

            allCorrect = areAllCorrect;
            

            if (allCorrect)
            {
                audioManager.PlaySound( Sfx.StampOK);
                yield return TimeEngine.Wait( 0.4f);
                checkmarkWidget.Show( true);
            }
            else
            {
                checkmarkWidget.Show( false);
                audioManager.PlaySound( Sfx.KO);
            }

            yield return TimeEngine.Wait( 1.0f);
            coroutineEnded = true;
            dragManager.EnableInput();
        }

        public bool IsAnimating()
        {
            return isAnimating;
        }
    }
}
