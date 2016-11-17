using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class DefaultAnswerChecker : IAnswerChecker
    {
        private ICheckmarkWidget checkmarkWidget;

        public DefaultAnswerChecker( ICheckmarkWidget checkmarkWidget)
        {
            this.checkmarkWidget = checkmarkWidget;
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
                    //TODO: Inject the question behaviour
                    p.Placeholder.GetQuestion().gameObject
                        .GetComponent< QuestionBehaviour>().OnQuestionAnswered();
                    yield return TimeEngine.Wait(1.0f);
                }
            }

            allCorrect = areAllCorrect;
            checkmarkWidget.Show( allCorrect);

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
