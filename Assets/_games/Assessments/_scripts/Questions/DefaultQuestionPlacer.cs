using DG.Tweening;
using Kore.Coroutines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    public class DefaultQuestionPlacer : IQuestionPlacer
    {
        protected AssessmentAudioManager audioManager;
        protected QuestionPlacerOptions options;

        public DefaultQuestionPlacer(   AssessmentAudioManager audioManager, QuestionPlacerOptions options)
        {
            this.audioManager = audioManager;
            this.options = options;
        }

        protected bool isAnimating = false;
            
        public bool IsAnimating()
        {
            return isAnimating;
        }

        protected IQuestion[] allQuestions;
        protected List< IEnumerator> questionSounds;
        private List< StillLetterBox> images;
        private List< QuestionBox> boxesList;

        public void Place( IQuestion[] question, bool playSound)
        {
            allQuestions = question;
            isAnimating = true;
            images = new List< StillLetterBox>();
            questionSounds = new List< IEnumerator>();
            boxesList = new List< QuestionBox>();
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

            float questionSize = options.QuestionSize;
            float occupiedSpace = 
                    placeHoldersNumber * options.SlotSize
                +   questionsNumber*options.QuestionSize;

            float blankSpace = options.RightX - options.LeftX - occupiedSpace;
            float spaceIncrement = blankSpace / (questionsNumber + 1);
            
            var flow = AssessmentOptions.Instance.LocaleTextFlow;
            float sign;
            Vector3 currentPos = new Vector3( 0, options.QuestionY, 5f);

            if (flow == TextFlow.RightToLeft)
            {
                currentPos.x = options.RightX;
                sign = -1;
            }
            else
            {
                currentPos.x = options.LeftX;
                sign = 1;
            }
            
            int questionIndex = 0;

            for (int i = 0; i < questionsNumber; i++)
            {
                currentPos.x += (spaceIncrement + questionSize/2) * sign;
                yield return PlaceQuestion( allQuestions[ questionIndex], currentPos, playAudio);
                currentPos.x += (questionSize * sign) / 2;

                if ( options.SpawnImageWithQuestion)
                {
                    currentPos.x += (sign * options.ImageSize) /1.8f ;
                    yield return PlaceImage( allQuestions[ questionIndex], currentPos);
                    currentPos.x += (sign * options.ImageSize) / 1.8f;
                }

                foreach (var p in allQuestions[ questionIndex].GetPlaceholders())
                {
                    currentPos.x += sign * options.SlotSize / 2;
                    yield return PlacePlaceholder( p, currentPos);
                    currentPos.x += sign * options.SlotSize / 2;
                }

                WrapQuestionInABox( allQuestions[ questionIndex]);

                questionIndex++;
            }

            // give time to finish animating elements
            yield return Wait.For( 0.65f);
            isAnimating = false;
        }

        protected void WrapQuestionInABox( IQuestion q)
        {
            var ll = q.gameObject.GetComponent< StillLetterBox>();

            int placeholdersCount = 0;

            foreach (var p in q.GetPlaceholders())
                placeholdersCount++;

            StillLetterBox[] boxes = new StillLetterBox[ placeholdersCount + 1];

            placeholdersCount = 0;
            foreach (var p in q.GetPlaceholders())
                boxes[placeholdersCount++] = p.GetComponent< StillLetterBox>();

            boxes[boxes.Length - 1] = ll;

            var box = LivingLetterFactory.Instance.SpawnQuestionBox( boxes);
            box.Show();
            audioManager.PlayPoofSound();

            boxesList.Add( box);
        }

        protected IYieldable PlaceImage( IQuestion q, Vector3 imagePos)
        {
            var ll = LivingLetterFactory.Instance.SpawnQuestion( q.Image());

            images.Add( ll);
            ll.transform.position = imagePos;
            ll.InstaShrink();
            ll.Poof();
            audioManager.PlayPoofSound();
            ll.Magnify();
            return Wait.For( 1.0f);
        }

        IQuestion lastPlacedQuestion = null;

        protected IYieldable PlaceQuestion( IQuestion q, Vector3 position, bool playAudio)
        {
            lastPlacedQuestion = q;
            var ll = q.gameObject.GetComponent< StillLetterBox>();

            ll.Poof();

            audioManager.PlayPoofSound();
            ll.transform.localPosition = position;

            ll.transform.GetComponent< StillLetterBox>().Magnify();

            if ( playAudio)
                q.QuestionBehaviour.ReadMeSound();

            return Wait.For( 1.0f);
        }

        protected IYieldable PlacePlaceholder( GameObject placeholder, Vector3 position)
        {
            var tr = placeholder.transform;
            audioManager.PlayPlaceSlot();
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
            foreach (var box in boxesList)
                box.Hide();

            foreach ( var q in allQuestions)
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
            audioManager.PlayPoofSound();
            image.Poof();

            image.transform.DOScale( 0, 0.4f).OnComplete( () => GameObject.Destroy( image.gameObject));
            yield return Wait.For( 0.1f);
        }

        IEnumerator FadeOutQuestion( IQuestion q)
        {
            audioManager.PlayPoofSound();
            q.gameObject.GetComponent< StillLetterBox>().Poof();
            q.gameObject.transform.DOScale( 0, 0.4f).OnComplete(() => GameObject.Destroy( q.gameObject));
            yield return Wait.For( 0.1f);
        }

        IEnumerator FadeOutPlaceholder( GameObject go)
        {
            audioManager.PlayRemoveSlot();

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
            audioManager.PlayQuestionBlip();

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
