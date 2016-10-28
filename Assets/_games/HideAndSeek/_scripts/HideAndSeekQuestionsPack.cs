using UnityEngine;
using System.Collections.Generic;


namespace EA4S.HideAndSeek
{
	public class HideAndSeekQuestionsPack : IQuestionPack
	{
		IEnumerable<ILivingLetterData> correctAnswer;
		 IEnumerable<ILivingLetterData> listOfLetters;

        public ILivingLetterData answer;
        public List<ILivingLetterData> list;

		public HideAndSeekQuestionsPack (IEnumerable<ILivingLetterData> listOfLetters,IEnumerable<ILivingLetterData> correctAnswer)

		{
			this.correctAnswer = correctAnswer;
			this.listOfLetters = listOfLetters;

            foreach( ILivingLetterData x in correctAnswer)
            {
                answer = x;
            }
            //correctAnswer.GetEnumerator().MoveNext();
            //answer = correctAnswer.GetEnumerator().Current;
            //list = (List <ILivingLetterData> )listOfLetters;

            list = new List<ILivingLetterData>();

            foreach (ILivingLetterData x in listOfLetters)
            {
                list.Add(x);
            }


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

        public List<ILivingLetterData> GetLetters()
        {
            return list;
        }

    }
}