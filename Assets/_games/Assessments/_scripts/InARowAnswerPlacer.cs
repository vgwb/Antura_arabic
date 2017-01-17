using DG.Tweening;
using Kore.Coroutines;
using System;
using System.Collections;
using EA4S.LivingLetters;
using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// Place answers in a line, ready to be sorted
    /// </summary>
    internal class InARowAnswerPlacer : IAnswerPlacer
    {
        private IAudioManager audioManager;
        private float letterSize;

        public InARowAnswerPlacer( IAudioManager audioManager, float letterSize)
        {
            this.audioManager = audioManager;
            this.letterSize = letterSize;
        }

        private bool isAnimating = false;
        public bool IsAnimating()
        {
            return isAnimating;
        }

        private IAnswer[] allAnswers;

        public void Place( IAnswer[] answer)
        {
            IAnswer[] original = new IAnswer[answer.Length];
            for (int i = 0; i < answer.Length; i++)
                original[i] = answer[i];

            Debug.Log("AnswerPlace");
            for (int i = 0; i < answer.Length; i++)
                answer[ i].AddTicket(i);

            answer = answer.Shuffle();

            ForceRandom( answer, original);

            allAnswers = answer;
            isAnimating = true;
            Koroutine.Run( PlaceCoroutine());
        }

        public void ForceRandom( IAnswer[] answer, IAnswer[] original)
        {
            for (int i = 0; i < answer.Length; i++)
                // if there is one element out of place.. GOOD!
                if( answer[i].Equals(original[i]) == false)
                    return;

            // Otherwise we swap the first 2 elements that are different
            // (we force at least 1 out of place to prevent automatic victory!)
            for (int i = 0; i < answer.Length; i++)
                if (answer[i].Equals(answer[0]) == false)
                {
                    IAnswer first = answer[0];
                    answer[0] = answer[i];
                    answer[i] = first;
                }
        }

        private IEnumerator PlaceCoroutine()
        {
            var bounds = WorldBounds.Instance;

            // Text justification "algorithm"
            float letterGap = 1.3f;
            float occupiedSpace = allAnswers.Length * ( letterGap * letterSize);
            float spaceIncrement = ( letterGap * letterSize);

            var flow = AssessmentOptions.Instance.LocaleTextFlow;
            float sign;
            Vector3 currentPos = Vector3.zero;
            currentPos.y = -1;
            currentPos.z = bounds.DefaultZ();

            if (flow == TextFlow.RightToLeft)
            {
                currentPos.x = occupiedSpace / 2f;
                sign = -1;
            }
            else
            {
                currentPos.x = -occupiedSpace / 2f;
                sign = 1;
            }

            currentPos.y -= 1.5f;

            foreach (var a in allAnswers)
            {
                yield return Koroutine.Nested(PlaceAnswer(a, currentPos));
                currentPos.x += spaceIncrement * sign;
            }

            yield return Wait.For( 0.65f);
            isAnimating = false;
        }

        private IEnumerator PlaceAnswer( IAnswer a, Vector3 currentPos)
        {
            var go = a.gameObject;
            go.transform.localPosition = currentPos;
            go.transform.DOScale( 1, 0.4f);
            go.GetComponent< LetterObjectView>().Poof( ElementsSize.PoofOffset);
            audioManager.PlaySound( Sfx.Poof);

            yield return Wait.For( UnityEngine.Random.Range( 0.07f, 0.13f));
        }

        public void RemoveAnswers()
        {
            isAnimating = true;
            Koroutine.Run( RemoveCoroutine());
        }

        private IEnumerator RemoveCoroutine()
        {
            foreach (var a in allAnswers)
                yield return Koroutine.Nested( RemoveAnswer( a.gameObject));

            yield return Wait.For(0.65f);
            isAnimating = false;
        }

        private IEnumerator RemoveAnswer(GameObject answ)
        {
            audioManager.PlaySound(Sfx.Poof);

            answ.GetComponent<LetterObjectView>().Poof(ElementsSize.PoofOffset);
            answ.transform.DOScale(0, 0.3f).OnComplete(() => GameObject.Destroy(answ));

            yield return Wait.For( 0.1f);
        }
    }
}
