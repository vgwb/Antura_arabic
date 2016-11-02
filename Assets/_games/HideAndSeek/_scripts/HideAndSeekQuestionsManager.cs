using UnityEngine;
using System.Collections;


namespace EA4S.HideAndSeek
{

    public class HideAndSeekQuestionsManager
    {



	    // Use this for initialization
	    public HideAndSeekQuestionsManager()
        {
			Debug.Log("Init Question Manager");

			//questionsProvider = new HideAndSeekQuestionsProvider();
            var question = HideAndSeekConfiguration.Instance.Questions.GetNextQuestion();// (HideAndSeekQuestionsPack)questionsProvider.GetQuestion(); //better a static cast
            currentQuestion = (HideAndSeekQuestionsPack)question;
        }

        public void GetNextQuestion()
        {
            var question = HideAndSeekConfiguration.Instance.Questions.GetNextQuestion();
            currentQuestion = (HideAndSeekQuestionsPack)question;
        }
        

        public HideAndSeekQuestionsPack currentQuestion;
		//private ILivingLetterData correct;
		//private ILivingLetterData question;
		private HideAndSeekGame game;
		//private HideAndSeekQuestionsProvider questionsProvider;

    }

}