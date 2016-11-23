using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EA4S.SickLetters
{
    public class SickLettersQuestionProvider : MonoBehaviour, IQuestionProvider
    {
        string dotlessLetters = "ﻻ لأ ﺉ آ ٶ ى ر س ل ص ع ه ح د م ك ط ئ ء ؤ و -", prevLetter="", newLetterString="X";

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
            while (newLetterString == "" || dotlessLetters.Contains(newLetterString) || newLetterString == prevLetter);

            Debug.Log(newLetterString);
            //SickLettersQuestionsPack dataPack = new SickLettersQuestionsPack(newLetter);

            correctAnswers.Add(newLetter);
            return new SampleQuestionPack(newLetter, wrongAnswers, correctAnswers);

            //return dataPack;
        }

        public SickLettersQuestionsPack SickLettersGetNextQuestion()
        {
            LL_LetterData newLetter;

            prevLetter = newLetterString;
            do
            {
                newLetter = AppManager.Instance.Teacher.GetRandomTestLetterLL();
                newLetterString = newLetter.TextForLivingLetter.ToString();

            }
            while (newLetterString == "" || dotlessLetters.Contains(newLetterString) || newLetterString == prevLetter);

            SickLettersQuestionsPack dataPack = new SickLettersQuestionsPack(newLetter);
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

            SickLettersQuestionsPack dataPack = new SickLettersQuestionsPack(newLetter);
            return newLetter;
        }

    }
}