using UnityEngine;
using System.Collections;

namespace EA4S.HideAndSeek
{
    public class HideAndSeekQuestionsManager
    {
	    public HideAndSeekQuestionsManager()
        {
			Debug.Log("Init Question Manager");
            
            var question = HideAndSeekConfiguration.Instance.Questions.GetNextQuestion();
            currentQuestion = (HideAndSeekQuestionsPack)question;
        }

        public void GetNextQuestion()
        {
            var question = HideAndSeekConfiguration.Instance.Questions.GetNextQuestion();
            currentQuestion = (HideAndSeekQuestionsPack)question;
        }
        

        public HideAndSeekQuestionsPack currentQuestion;
		private HideAndSeekGame game;
    }
}