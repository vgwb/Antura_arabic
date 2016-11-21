using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EA4S.HideAndSeek
{
	public class HideAndSeekQuestionsProvider : MonoBehaviour, IQuestionProvider {

	List<HideAndSeekQuestionsPack> questions = new List<HideAndSeekQuestionsPack>();

	string description;
    int currentQuestion;

	public void Start()
	{
		currentQuestion = 0;

		int numbersOfLetters = 3;
		
		description = "Hide and Seek Description";

            float difficulty = HideAndSeekConfiguration.Instance.Difficulty;
			Debug.Log ("Difficulty " + difficulty);

            if (difficulty <= 0.4f && difficulty > 0.2f)
                numbersOfLetters = 4;
            else if (difficulty <= 0.6f)
                numbersOfLetters = 5;
            else if (difficulty <= 0.8f)
                numbersOfLetters = 6;
            else if (difficulty <= 1.0f)
                numbersOfLetters = 7;
            
			for (int i = 0; i < 20; i++)
		{
			List<ILivingLetterData> listOfLetters = new List<ILivingLetterData>();

				// i don't like it , can loop forever
				for(int j = 0; j < numbersOfLetters;) 
				{
					if (j == 0) 
					{
						listOfLetters.Add (AppManager.Instance.Teacher.GetRandomTestLetterLL());
						j++;
					} 
					else 
					{
						var lett = AppManager.Instance.Teacher.GetRandomTestLetterLL();
						if (!CheckIfContains (listOfLetters, lett)) 
						{
							listOfLetters.Add (lett);
							j++;
						}
					}

				}
                
				for (int j = 0; j < numbersOfLetters; j++) {
					List<ILivingLetterData> correctAnswer = new List<ILivingLetterData>();
					correctAnswer.Add (listOfLetters [j]);
					var currentPack = new HideAndSeekQuestionsPack (listOfLetters,correctAnswer);
					questions.Add (currentPack);
				}
		    }
	    }

	    static bool CheckIfContains(List<ILivingLetterData> list, ILivingLetterData letter)
	    {
	    	for (int i = 0, count = list.Count; i < count; ++i)
	    		if (list[i].Id == letter.Id)
	    			return true;
	    	return false;
	    }

	    public string GetDescription()
	    {
	    	return description;
	    }

	    IQuestionPack IQuestionProvider.GetNextQuestion()
	    {
            return null;
	    }
    
        public IQuestionPack GetQuestion()
        {
            currentQuestion++;

            if (currentQuestion >= questions.Count)
                currentQuestion = 0;

            return questions[currentQuestion];
        }
    }
}