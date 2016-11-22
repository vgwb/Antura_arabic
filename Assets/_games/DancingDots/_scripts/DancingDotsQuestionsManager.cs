using UnityEngine;
using System.Collections;
namespace EA4S.DancingDots
{
	public class DancingDotsQuestionsManager
	{
        string dotlessLetters = /*"ض ث ق ف غ خ ج ش ي ب ت ن ة ظ"*/ " - ﻻ لأ ﺉ آ إ ٶ أ ا ى ر س ل ص ع ه ح د م ك ط ئ ء ؤ و", prevLetter = "";

		public ILivingLetterData getNewLetter()
		{

			ILivingLetterData newLetter = null;

			do
			{
				if(newLetter!=null)
					prevLetter = newLetter.TextForLivingLetter;
				IQuestionProvider newQuestionProvider = DancingDotsConfiguration.Instance.Questions;
				IQuestionPack nextQuestionPack = newQuestionProvider.GetNextQuestion(); 

				foreach (ILivingLetterData letterData in nextQuestionPack.GetCorrectAnswers())
				{
					newLetter = letterData;

				}
			}
			while (!dotlessLetters.Contains(newLetter.TextForLivingLetter) || newLetter.TextForLivingLetter == prevLetter);

			//ILivingLetterData newLetter = ((SampleQuestionPack)nextQuestionPack).GetQuestions();

			return newLetter;
		}
	}
}
