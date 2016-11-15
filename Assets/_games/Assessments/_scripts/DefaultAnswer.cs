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
            answer.SetAnswer( this);
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
    }
}
