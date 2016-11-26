using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EA4S.DancingDots
{
	public class DancingDotsQuestionProvider : IQuestionProvider
	{

		public IQuestionPack GetNextQuestion()
		{
			LL_LetterData newLetter;
			List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
			List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

			newLetter = AppManager.Instance.Teacher.GetRandomTestLetterLL();

			correctAnswers.Add(newLetter);
			return new SampleQuestionPack(newLetter, wrongAnswers, correctAnswers);
		}

//		public DancingDotsQuestionsPack DancingDotsGetNextQuestion()
//		{
//			LL_LetterData newLetter;
//
//			newLetter = AppManager.Instance.Teacher.GetRandomTestLetterLL();
//
//			DancingDotsQuestionsPack dataPack = new DancingDotsQuestionsPack(newLetter);
//			return dataPack;
//		}
//
//		public ILivingLetterData GetNextQuestion_Temp()
//		{
//			LL_LetterData newLetter;
//
//			newLetter = AppManager.Instance.Teacher.GetRandomTestLetterLL();
//
//			return newLetter;
//		}

	}
}