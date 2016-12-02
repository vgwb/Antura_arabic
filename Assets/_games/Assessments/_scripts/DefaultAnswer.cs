using System;
using UnityEngine;

namespace EA4S.Assessment
{
    public class DefaultAnswer : IAnswer
    {
        private LetterObjectView view;
        private bool isCorrect;

        public DefaultAnswer( LetterObjectView letter, bool correct)
        {
            view = letter;
            isCorrect = correct;
            var answer = letter.gameObject.AddComponent< AnswerBehaviour>();
            answer.SetAnswer(this);
        }

        public override bool Equals(object obj)
        {
            var other = (DefaultAnswer)obj;
            return other == this;
        }

        public GameObject gameObject
        {
            get
            {
                return view.gameObject;
            }
        }

        public bool IsCorrect()
        {
            return isCorrect;
        }

        int answerSet = 0;

        public void SetAnswerSet( int set)
        {
            answerSet = set;
        }

        public int GetAnswerSet()
        {
            return answerSet;
        }

        public bool Equals( IAnswer other)
        {
            if (Data().Equals(other.Data()))
            {
                Debug.Log("TRUE: " + Data().Id + " and " + other.Data().Id);
                return true;
            }
            return false;
        }

        public ILivingLetterData Data()
        {
            return view.Data;
        }
    }
}
