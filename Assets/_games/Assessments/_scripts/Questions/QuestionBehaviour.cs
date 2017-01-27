using DG.Tweening;
using EA4S.LivingLetters;
using System;
using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// Keeps linked IQuestion and LL Gameobject
    /// </summary>
    public class QuestionBehaviour: MonoBehaviour
    {
        private IQuestion question = null;

        public void ReadMeSound()
        {
            dialogues.PlayLetterData( GetComponent< LetterObjectView>().Data);
        }

        public void FaceDownInstant()
        {
            Debug.Log("FaceDown");
            transform.rotation = Quaternion.Euler( new Vector3(0, 0, 0));
        }

        bool triggered = false;
        public void TurnFaceUp()
        {
            if (triggered)
                return;

            triggered = true;
            transform.DORotate( new Vector3( 0, 180, 0), 1);
        }

        AssessmentDialogues dialogues;
        public void SetQuestion( IQuestion qst, AssessmentDialogues dialogues)
        {
            this.dialogues = dialogues;

            if (qst == null)
                throw new ArgumentException( "Null questions");

            if (question == null)
                question = qst;
            else
                throw new ArgumentException( "Answer already added");
        }

        public IQuestion GetQuestion()
        {
            return question;
        }

        void OnMouseDown()
        {
            if( AssessmentOptions.Instance.PronunceQuestionWhenClicked)
                dialogues.PlayLetterData( GetComponent< LetterObjectView>().Data);
        }

        public IQuestionDecoration questionAnswered;

        internal void OnQuestionAnswered()
        {
            if (AssessmentOptions.Instance.QuestionAnsweredPlaySound)
                ReadMeSound();

            if (AssessmentOptions.Instance.QuestionAnsweredFlip)
                TurnFaceUp();
        }

        internal void OnSpawned()
        {
            if (AssessmentOptions.Instance.QuestionSpawnedPlaySound)
                ReadMeSound();
        }

        internal float TimeToWait()
        {
            if( AssessmentOptions.Instance.QuestionAnsweredFlip ||
                AssessmentOptions.Instance.QuestionAnsweredPlaySound
              )
                return 1.0f;
            else
                return 0.05f;
        }
    }
}
