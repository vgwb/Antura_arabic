using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace EA4S.Assessment
{
    public class DefaultQuestionPlacer : IQuestionPlacer
    {
        private IAudioManager audioManager;
        private float questionSize;
        private float answerSize;

        public DefaultQuestionPlacer( IAudioManager audioManager, float questionSize, float answerSize)
        {
            this.audioManager = audioManager;
            this.questionSize = questionSize;
            this.answerSize = answerSize;
        }

        private bool isAnimating = false;
            
        public bool IsAnimating()
        {
            return isAnimating;
        }

        private IQuestion[] allQuestions;

        public void Place( IQuestion[] question)
        {
            allQuestions = question;
            isAnimating = true;
            Coroutine.Start( PlaceCoroutine());
        }

        IEnumerator PlaceCoroutine()
        {
            // Count questions and answers
            int questionsNumber = 0;
            int placeHoldersNumber = 0;

            foreach (var q in allQuestions)
            {
                questionsNumber++;
                placeHoldersNumber += q.PlaceholdersCount();
            }

            var bounds = WorldBounds.Instance;

            // Text justification "algorithm"
            var gap = bounds.QuestionGap();

            float occupiedSpace = questionsNumber*questionSize + answerSize*placeHoldersNumber;
            float blankSpace = gap - occupiedSpace;

            //  3 words => 4 white zones  (need increment by 1)
            //  |  O   O   O  |
            float spaceIncrement = blankSpace / (questionsNumber + 1);

            //Implement Line Break only if needed
            if ( blankSpace <= bounds.HalfLetterSize() )
                throw new InvalidOperationException( "Need a line break becase 1 line is not enough for all");

            var flow = AssessmentConfiguration.Instance.LocaleTextFlow;
            float sign;
            Vector3 currentPos;

            if (flow == AssessmentConfiguration.TextFlow.RightToLeft)
            {
                currentPos = bounds.ToTheRightQuestionStart();
                //currentPos.x -= answerSize / 2.0f;
                sign = -1;
            }
            else
            {
                currentPos = bounds.ToTheLeftQuestionStart();
                currentPos.x += answerSize / 2.0f;
                sign = 1;
            }
            
            int questionIndex = 0;

            for (int i = 0; i < questionsNumber; i++)
            {
                currentPos.x += (spaceIncrement + questionSize/2) * sign;
                yield return PlaceQuestion( allQuestions[ questionIndex], currentPos);
                currentPos.x += (questionSize * sign) / 2;

                foreach (var p in allQuestions[ questionIndex].GetPlaceholders())
                {
                    currentPos.x += (answerSize * sign)/2;
                    yield return PlacePlaceholder( allQuestions[ questionIndex], p, currentPos);
                    currentPos.x += (answerSize * sign) / 2;
                }

                questionIndex++;
            }

                /*if (flow == AssessmentConfiguration.TextFlow.RightToLeft)
                {
                    for (int i = 0; i < questionsNumber; i++)
                    {
                        currentPos.x += spaceIncrement + bounds.LetterSize();

                        foreach (var p in allQuestions[ questionIndex].GetPlaceholders())
                        {
                            yield return PlacePlaceholder( allQuestions[ questionIndex], p, currentPos);
                            currentPos.x += bounds.LetterSize();
                        }

                        yield return PlaceQuestion(
                            allQuestions[ questionIndex], currentPos);

                        questionIndex++;
                    }
                }
                else
                {
                    for (int i = 0; i < questionsNumber; i++)
                    {
                        currentPos.x += spaceIncrement + bounds.LetterSize();

                        yield return PlaceQuestion(
                            allQuestions[ questionIndex], currentPos);

                        foreach (var p in allQuestions[ questionIndex].GetPlaceholders())
                        {
                            currentPos.x += bounds.LetterSize();
                            yield return PlacePlaceholder( allQuestions[ questionIndex], p, currentPos);
                        }

                        questionIndex++;
                    }
                }*/

            // give time to finish animating elements
            yield return TimeEngine.Wait( 0.65f);
            isAnimating = false;
        }

        private IEnumerator PlaceQuestion( IQuestion q, Vector3 position)
        {
            var ll = q.gameObject.GetComponent< LetterObjectView>();

            ll.Poof( ElementsSize.PoofOffset);
            audioManager.PlaySound( Sfx.Poof);
            ll.transform.localPosition = position;
            ll.transform.DOScale( 1, 0.3f);
            q.gameObject.GetComponent< QuestionBehaviour>().OnSpawned();
            return TimeEngine.Wait( 1.0f);
        }

        private IEnumerator PlacePlaceholder( IQuestion q, GameObject placeholder, Vector3 position)
        {
            Transform tr = placeholder.transform;
            tr.localPosition = position + new Vector3( 0, 5, 0);
            tr.localScale = new Vector3( 0.5f, 0.5f, 0.5f);
            audioManager.PlaySound( Sfx.StarFlower);

            float adjust = 4.5f;

            if (answerSize == (1.5f * 3f))
                adjust = 3;

            if (answerSize == (1f * 3f))
                adjust = ElementsSize.DropZoneScale;


            var seq = DOTween.Sequence();
            seq
                .Insert( 0, tr.DOScale( new Vector3( adjust, ElementsSize.DropZoneScale, 1), 0.4f))
                .Insert( 0, tr.DOMove( position, 0.6f));

            return TimeEngine.Wait( 0.4f);
        }

        public void RemoveQuestions()
        {
            isAnimating = true;
            Coroutine.Start( RemoveCoroutine());
        }

        IEnumerator RemoveCoroutine()
        {
            foreach( var q in allQuestions)
            {
                foreach (var p in q.GetPlaceholders())
                    yield return FadeOutPlaceholder( p);

                yield return FadeOutQuestion( q);
            }

            // give time to finish animating elements
            yield return TimeEngine.Wait( 0.65f);
            isAnimating = false;
        }

        IEnumerator FadeOutQuestion( IQuestion q)
        {
            audioManager.PlaySound( Sfx.Poof);
            q.gameObject.GetComponent< LetterObjectView>().Poof( ElementsSize.PoofOffset);


            q.gameObject.transform.DOScale( 0, 0.4f).OnComplete(() => GameObject.Destroy( q.gameObject));
            yield return TimeEngine.Wait( 0.1f);
        }

        IEnumerator FadeOutPlaceholder( GameObject go)
        {
            audioManager.PlaySound( Sfx.BalloonPop);

            go.transform.DOScale(0, 0.23f).OnComplete(() => GameObject.Destroy( go));
            yield return TimeEngine.Wait(0.06f);
        }
    }
}
