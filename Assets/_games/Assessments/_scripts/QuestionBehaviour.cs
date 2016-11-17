using System;
using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// Keeps linked IQuestion and LL Gameobject
    /// </summary>
    public class QuestionBehaviour : MonoBehaviour
    {
        private IQuestion question = null;
        public void SetQuestion( IQuestion qst)
        {
            if (qst == null)
                throw new ArgumentException("Null questions");

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
            if(AssessmentConfiguration.Instance.PronunceQuestionWhenClicked)
                AssessmentConfiguration.Instance.Context.GetAudioManager()
                    .PlayLetterData( GetComponent< LetterObjectView>().Data);
        }

        public IQuestionDecoration questionAnswered;

        internal void OnQuestionAnswered()
        {
            questionAnswered.TriggerOnAnswered();
        }

        internal void OnSpawned()
        {
            questionAnswered.TriggerOnSpawned();
        }

        internal float TimeToWait()
        {
            return questionAnswered.TimeToWait();
        }
    }
}
