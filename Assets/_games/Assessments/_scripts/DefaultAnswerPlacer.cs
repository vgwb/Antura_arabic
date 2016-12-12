using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    public class DefaultAnswerPlacer : IAnswerPlacer
    {
        public DefaultAnswerPlacer(IAudioManager audio)
        {
            audioManager = audio;
        }

        private bool isAnimating = false;
        public bool IsAnimating()
        {
            return isAnimating;
        }

        private IAnswer[] allAnswers;
        private IAudioManager audioManager;

        public void Place(IAnswer[] answer)
        {
            allAnswers = answer;
            isAnimating = true;
            Coroutine.Start(PlaceCoroutine());
        }

        public void RemoveAnswers()
        {
            isAnimating = true;
            Coroutine.Start(RemoveCoroutine());
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

            if (AppConstants.VerboseLogging) {
                Debug.Log("xMin" + xMin);
                Debug.Log("xMax" + xMax);
                Debug.Log("yMin" + yMin);
                Debug.Log("yMax" + yMax);
            }

            for (float x = xMin; x < xMax; x += 4.3f)
                for (float y = yMin; y < yMax; y += 1.7f) {
                    float dx = Random.Range(-0.2f, 0.2f);
                    float dy = Random.Range(-0.1f, 0.2f);
                    var vec = new Vector3(x + dx, y + dy, z);
                    positions.Add(vec);
                    if (AppConstants.VerboseLogging)
                        Debug.Log("VECTOR" + vec);
                }

            if (AppConstants.VerboseLogging)
                Debug.Log("COUNT:" + positions.Count);
            positions = positions.Shuffle();

            foreach (var a in allAnswers)
                yield return PlaceAnswer(a, positions);

            yield return TimeEngine.Wait(0.65f);
            isAnimating = false;
        }

        private IEnumerator PlaceAnswer(IAnswer answer, List<Vector3> positions)
        {
            var go = answer.gameObject;
            go.transform.localPosition = positions.Pull();
            go.transform.DOScale(1, 0.4f);
            go.GetComponent<LetterObjectView>().Poof(ElementsSize.PoofOffset);
            audioManager.PlaySound(Sfx.Poof);

            yield return TimeEngine.Wait(Random.Range(0.07f, 0.13f));
        }

        private IEnumerator RemoveCoroutine()
        {
            foreach (var a in allAnswers)
                yield return RemoveAnswer(a.gameObject);

            yield return TimeEngine.Wait(0.65f);
            isAnimating = false;
        }

        private IEnumerator RemoveAnswer(GameObject answ)
        {
            audioManager.PlaySound(Sfx.Poof);

            answ.GetComponent<LetterObjectView>().Poof(ElementsSize.PoofOffset);
            answ.transform.DOScale(0, 0.3f).OnComplete(() => GameObject.Destroy(answ));

            yield return TimeEngine.Wait(0.1f);
        }
    }
}
