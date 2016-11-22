using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EA4S.DancingDots
{
	public class DancingDotsQuestionProvider : IQuestionProvider
	{
        string dotlessLetters = /*"ض ث ق ف غ خ ج ش ي ب ت ن ة ظ"*/ " - ﻻ لأ ﺉ آ إ ٶ أ ا ى ر س ل ص ع ه ح د م ك ط ئ ء ؤ و", prevLetter = "", newLetterString="X";

		public IQuestionPack GetNextQuestion()
		{
			LL_LetterData newLetter;
			List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
			List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

			prevLetter = newLetterString;
			do
			{
				newLetter = AppManager.Instance.Teacher.GetRandomTestLetterLL();
				newLetterString = newLetter.TextForLivingLetter;

			}
			while (newLetterString == "" || !dotlessLetters.Contains(newLetterString) || newLetterString == prevLetter);

			Debug.Log(newLetterString);
			//DancingDotsQuestionsPack dataPack = new DancingDotsQuestionsPack(newLetter);

			correctAnswers.Add(newLetter);
			return new SampleQuestionPack(newLetter, wrongAnswers, correctAnswers);

			//return dataPack;
		}

		public DancingDotsQuestionsPack DancingDotsGetNextQuestion()
		{
			LL_LetterData newLetter;

			prevLetter = newLetterString;
			do
			{
				newLetter = AppManager.Instance.Teacher.GetRandomTestLetterLL();
				newLetterString = newLetter.TextForLivingLetter.ToString();

			}
			while (newLetterString == "" || dotlessLetters.Contains(newLetterString) || newLetterString == prevLetter);

			DancingDotsQuestionsPack dataPack = new DancingDotsQuestionsPack(newLetter);
			return dataPack;
		}

		public ILivingLetterData GetNextQuestion_Temp()
		{
			LL_LetterData newLetter;

			prevLetter = newLetterString;
			do
			{
				newLetter = AppManager.Instance.Teacher.GetRandomTestLetterLL();
				newLetterString = newLetter.TextForLivingLetter.ToString();

			}
			while (newLetterString == "" || dotlessLetters.Contains(newLetterString) || newLetterString == prevLetter);

//			DancingDotsQuestionsPack dataPack = new DancingDotsQuestionsPack(newLetter);
			return newLetter;
		}

	}
}