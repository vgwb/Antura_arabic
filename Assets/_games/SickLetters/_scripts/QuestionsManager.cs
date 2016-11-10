using UnityEngine;
using System.Collections;
namespace EA4S.SickLetters
{
    public class QuestionsManager
    {

        public void StartNewQuestion()
        {
            var nextQuestionPack = SickLettersConfiguration.Instance.Questions.GetNextQuestion();


        }
    }
}
