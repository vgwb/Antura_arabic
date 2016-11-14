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
        void SetAnswer( IAnswer answ)
        {
            if (answer == null)
                answer = answ;
            else
                throw new ArgumentException( "Answer already added");

            answer.SetGameObject( gameObject);
        }

        IAnswer GetAnswer()
        {
            return answer;
        }
    }
}
