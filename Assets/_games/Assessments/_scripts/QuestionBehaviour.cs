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
        void SetAnswer( IQuestion qst)
        {
            if (question == null)
                question = qst;
            else
                throw new ArgumentException( "Answer already added");

            question.SetGameObject( gameObject);
        }

        IQuestion GetQuestion()
        {
            return question;
        }
    }
}
