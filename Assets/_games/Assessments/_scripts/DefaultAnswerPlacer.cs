using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    public class DefaultAnswerPlacer : IAnswerPlacer
    {
        public DefaultAnswerPlacer( IAudioManager audio)
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

        public void Place( IAnswer[] answer)
        {
            allAnswers = answer;
            isAnimating = true;
            Coroutine.Start( PlaceCoroutine());
        }

        public void RemoveAnswers()
        {
            isAnimating = true;
            Coroutine.Start( RemoveCoroutine());
        }

        private IEnumerator PlaceCoroutine()
        {
            foreach( var a in allAnswers)
                yield return PlaceAnswer( a);

            yield return TimeEngine.Wait( 0.65f);
            isAnimating = false;
        }

        private IEnumerator PlaceAnswer( IAnswer answer)
        {
            var go = answer.gameObject;
            var pos = Vector3.zero;
            List< Vector3> positions = new List< Vector3>();
            bool overlapping = false;
            int attemps = 10000;
            for (int i = 0; i < attemps; i++)
            {
                pos = WorldBounds.Instance.RandomAnswerPosition();

                foreach( var p in positions)
                {
                    // If overlapping, which is the nearest tile?
                    float localDistance = pos.SquaredDistance( p);
                    if (pos.DistanceIsLessThan( p, 2.1f * WorldBounds.Instance.LetterSize()))
                        overlapping = true;
                }

                if (overlapping == false)
                    break;
            }

            positions.Add( pos);
            go.transform.localPosition = pos;
            go.transform.DOScale( 1, 0.4f);
            go.GetComponent< LetterObjectView>().Poof( ElementsSize.PoofOffset);
            audioManager.PlaySound( Sfx.Poof);

            yield return TimeEngine.Wait( Random.Range( 0.07f, 0.13f));
        }

        private IEnumerator RemoveCoroutine()
        {
            foreach (var a in allAnswers)
                yield return RemoveAnswer( a.gameObject);

            yield return TimeEngine.Wait( 0.65f);
            isAnimating = false;
        }

        private IEnumerator RemoveAnswer( GameObject answ)
        {  
            audioManager.PlaySound( Sfx.Poof);

            answ.GetComponent< LetterObjectView>().Poof( ElementsSize.PoofOffset);
            answ.transform.DOScale( 0, 0.3f).OnComplete(() => GameObject.Destroy( answ));

            yield return TimeEngine.Wait( 0.1f);
        }
    }
}
