using DG.Tweening;
using EA4S.LivingLetters;
using Kore.Coroutines;
using ModularFramework.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// Place answers to random places over a grid with small displacement
    /// (answers are not overlapped and start in different positions,
    /// without overlapping the question area).
    /// </summary>
    public class RandomAnswerPlacer : IAnswerPlacer
    {
        public RandomAnswerPlacer( IAudioManager audio)
        {
            audioManager = audio;
        }

        private bool isAnimating = false;
        public bool IsAnimating()
        {
            return isAnimating;
        }

        private Answer[] allAnswers;
        private IAudioManager audioManager;

        public void Place(Answer[] answer)
        {
            allAnswers = answer;
            isAnimating = true;
            Koroutine.Run( PlaceCoroutine());
        }

        public void RemoveAnswers()
        {
            isAnimating = true;
            Koroutine.Run( RemoveCoroutine());
        }

        private IEnumerator PlaceCoroutine()
        {
            List<Vector3> positions = new List<Vector3>();
            WorldBounds bounds = WorldBounds.Instance;
            float xMin = bounds.ToTheLeftQuestionStart().x;
            float xMax = bounds.ToTheRightQuestionStart().x;
            float yMin = bounds.YMin();
            float yMax = bounds.YMax();
            float z = bounds.DefaultZ();

            for (float x = xMin; x < xMax; x += 4.3f)
                for (float y = yMin; y < yMax; y += 1.7f) {
                    float dx = Random.Range(-0.2f, 0.2f);
                    float dy = Random.Range(-0.1f, 0.2f);
                    var vec = new Vector3(x + dx, y + dy, z);
                    positions.Add(vec);
                }

            positions.Shuffle();

            foreach (var a in allAnswers)
                yield return Koroutine.Nested( PlaceAnswer( a, positions));

            yield return Wait.For( 0.65f);
            isAnimating = false;
        }

        private IEnumerator PlaceAnswer( Answer answer, List<Vector3> positions)
        {
            var go = answer.gameObject;
            go.transform.localPosition = positions.Pull();
            go.transform.DOScale(1, 0.4f);
            go.GetComponent<LetterObjectView>().Poof(ElementsSize.PoofOffset);
            audioManager.PlaySound(Sfx.Poof);

            yield return Wait.For( Random.Range( 0.07f, 0.13f));
        }

        private IEnumerator RemoveCoroutine()
        {
            foreach (var a in allAnswers)
                yield return Koroutine.Nested( RemoveAnswer(a.gameObject));

            yield return Wait.For( 0.65f);
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
