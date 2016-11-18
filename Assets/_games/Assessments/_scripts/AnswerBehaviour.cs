using System;
using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// Keeps linked IAnswer and LL Gameobject
    /// </summary>
    public class AnswerBehaviour : MonoBehaviour
    {
        private IAnswer answer = null;
        public void SetAnswer( IAnswer answ)
        {
            if (answ == null)
                throw new ArgumentException( "Null questions");

            if (answer == null)
                answer = answ;
            else
                throw new ArgumentException( "Answer already added");
        }

        void OnMouseDown()
        {
            if(AssessmentConfiguration.Instance.PronunceAnswerWhenClicked)
                AssessmentConfiguration.Instance.Context.GetAudioManager()
                    .PlayLetterData( GetComponent< LetterObjectView>().Data);
        }

        public IAnswer GetAnswer()
        {
            return answer;
        }
    }
}
