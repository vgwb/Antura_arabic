using UnityEngine;
using System.Collections.Generic;


namespace EA4S.HideAndSeek
{
	public class HideAndSeekQuestionsPack : IQuestionPack
	{
		IEnumerable<ILivingLetterData> correctAnswer;
		 IEnumerable<ILivingLetterData> listOfLetters;

        public ILivingLetterData answer;

		public HideAndSeekQuestionsPack (IEnumerable<ILivingLetterData> listOfLetters,IEnumerable<ILivingLetterData> correctAnswer)

		{
			this.correctAnswer = correctAnswer;
			this.listOfLetters = listOfLetters;


            //answer = (ILivingLetterData)correctAnswer;
          
            
		}

		IEnumerable<ILivingLetterData> IQuestionPack.GetCorrectAnswers()
		{
			return correctAnswer;
		}


		ILivingLetterData IQuestionPack.GetQuestion()
		{
			return null;
		}

		IEnumerable<ILivingLetterData> IQuestionPack.GetWrongAnswers()
		{
			return listOfLetters;
			
		}

        public ILivingLetterData GetAnswer()
        {
            return answer;
        }

	}
}