using DG.Tweening;
using EA4S.MinigamesCommon;
using Kore.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    public class DefaultQuestionPlacer : IQuestionPlacer
    {
        protected IAudioManager audioManager;
        protected float questionSize;
        protected float answerSize;
        protected bool alsoDrawing;

        public DefaultQuestionPlacer(   IAudioManager audioManager, float questionSize, 
                                        float answerSize, bool alsoDrawing = false)
        {
            this.audioManager = audioManager;
            this.questionSize = questionSize;
            this.answerSize = answerSize;
            this.alsoDrawing = alsoDrawing;
        }

        protected bool isAnimating = false;
            
        public bool IsAnimating()
        {
            return isAnimating;
        }

        protected IQuestion[] allQuestions;
        protected List< IEnumerator> questionSounds;
        private List< StillLetterBox> images;

        public void Place( IQuestion[] question, bool playSound)
        {
            allQuestions = question;
            isAnimating = true;
            images = new List< StillLetterBox>();
            questionSounds = new List< IEnumerator>();
            Koroutine.Run( PlaceCoroutine( playSound));
        }


        IEnumerator PlaceCoroutine( bool playAudio)
        {
            return GetPlaceCoroutine( playAudio);
        }

        public virtual IEnumerator GetPlaceCoroutine( bool playAudio)
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
            if (alsoDrawing)
                occupiedSpace += questionsNumber * 3f;

            float blankSpace = gap - occupiedSpace;

            //  3 words => 4 white zones  (need increment by 1)
            //  |  O   O   O  |
            float spaceIncrement = blankSpace / (questionsNumber + 1);

            //Implement Line Break only if needed
            if ( blankSpace <= bounds.HalfLetterSize()/2f )
                throw new InvalidOperationException( "Need a line break becase 1 line is not enough for all");

            var flow = AssessmentOptions.Instance.LocaleTextFlow;
            float sign;
            Vector3 currentPos;

            if (flow == TextFlow.RightToLeft)
            {
                currentPos = bounds.ToTheRightQuestionStart();
                sign = -1;
            }
            else
            {
                currentPos = bounds.ToTheLeftQuestionStart();
                currentPos.x += answerSize / 2.0f;
                sign = 1;
            }

            currentPos.y -= 1.5f;
            
            int questionIndex = 0; //TODO: check if this redundant

            for (int i = 0; i < questionsNumber; i++)
            {
                currentPos.x += (spaceIncrement + questionSize/2) * sign;
                yield return PlaceQuestion(allQuestions[questionIndex], currentPos, playAudio);
                currentPos.x += (questionSize * sign) / 2;

                if (alsoDrawing)
                {
                    currentPos.x += (3f * sign) / 2;
                    yield return PlaceImage( allQuestions[questionIndex], currentPos);
                    currentPos.x += (3.3f * sign) / 2;
                }

                foreach (var p in allQuestions[ questionIndex].GetPlaceholders())
                {
                    currentPos.x += (answerSize * sign)/2;
                    yield return PlacePlaceholder( p, currentPos);
                    currentPos.x += (answerSize * sign) / 2;
                }

                questionIndex++;
            }

            // give time to finish animating elements
            yield return Wait.For( 0.65f);
            isAnimating = false;
        }

        protected IYieldable PlaceImage( IQuestion q, Vector3 imagePos)
        {
            var ll = LivingLetterFactory.Instance.SpawnQuestion( q.Image());

            images.Add( ll);
            ll.transform.position = imagePos;
            ll.InstaShrink();
            ll.Poof();
            audioManager.PlaySound( Sfx.Poof);
            ll.Magnify();
            return Wait.For( 1.0f);
        }

        IQuestion lastPlacedQuestion = null;

        protected IYieldable PlaceQuestion( IQuestion q, Vector3 position, bool playAudio)
        {
            lastPlacedQuestion = q;
            var ll = q.gameObject.GetComponent< StillLetterBox>();

            ll.Poof();

            audioManager.PlaySound( Sfx.Poof);
            ll.transform.localPosition = position;

            ll.transform.GetComponent< StillLetterBox>().Magnify();

            if ( playAudio)
                q.QuestionBehaviour.ReadMeSound();

            return Wait.For( 1.0f);
        }

        protected IYieldable PlacePlaceholder( GameObject placeholder, Vector3 position)
        {
            var tr = placeholder.transform;
            audioManager.PlaySound( Sfx.StarFlower);
            tr.localPosition = position;
            tr.GetComponent< StillLetterBox>().Magnify();
            return Wait.For( 0.4f);
        }

        public void RemoveQuestions()
        {
            isAnimating = true;
            Koroutine.Run( RemoveCoroutine());
        }

        IEnumerator RemoveCoroutine()
        {
            foreach( var q in allQuestions)
            {
                foreach (var p in q.GetPlaceholders())
                    yield return Koroutine.Nested( FadeOutPlaceholder( p));

                foreach (var img in images)
                    yield return Koroutine.Nested( FadeOutImage( img));

                yield return Koroutine.Nested( FadeOutQuestion( q));
            }
            
            // give time to finish animating elements
            yield return Wait.For( 0.65f);
            isAnimating = false;
        }

        IEnumerator FadeOutImage( StillLetterBox image)
        {
            audioManager.PlaySound( Sfx.Poof);
            image.Poof();

            image.transform.DOScale( 0, 0.4f).OnComplete( () => GameObject.Destroy( image.gameObject));
            yield return Wait.For( 0.1f);
        }

        IEnumerator FadeOutQuestion( IQuestion q)
        {
            audioManager.PlaySound( Sfx.Poof);
            q.gameObject.GetComponent< StillLetterBox>().Poof();
            q.gameObject.transform.DOScale( 0, 0.4f).OnComplete(() => GameObject.Destroy( q.gameObject));
            yield return Wait.For( 0.1f);
        }

        IEnumerator FadeOutPlaceholder( GameObject go)
        {
            audioManager.PlaySound( Sfx.BalloonPop);

            go.transform.DOScale( 0, 0.23f).OnComplete(() => GameObject.Destroy( go));
            yield return Wait.For( 0.06f);
        }

        /// <summary>
        ///  Should highlight 1 QUESTION and play their audio. This is called
        ///  only if we have to pronunce question, and only if we should pronunce it
        ///  after the tutorial brief. Actually this is always called in the first round
        ///  (I find it more natural) but requisites may change later so, it is still 
        ///  possible that we have to play this Before the tutorial brief.
        ///  (that is the reason I still have a
        ///  AssessmentOptions.Instance.PlayQuestionAudioAfterTutorial
        ///  flag
        /// </summary>
        public IYieldable PlayQuestionSound()
        {
            audioManager.PlaySound( Sfx.Blip);
            var sequence = DOTween.Sequence();
            lastPlacedQuestion.QuestionBehaviour.ReadMeSound();
            lastPlacedQuestion.gameObject.GetComponent< StillLetterBox>().Poof();

            sequence
                .Append( lastPlacedQuestion.gameObject.transform.DOScale( 0.5f, 0.15f))
                .Append( lastPlacedQuestion.gameObject.transform.DOScale( 1.0f, 0.15f));

            return Wait.For( 1.0f);
        }
    }
}
