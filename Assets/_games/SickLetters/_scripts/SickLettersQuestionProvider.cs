using UnityEngine;
using System.Collections;
using System;

namespace EA4S.SickLetters
{
    public class SickLettersQuestionProvider : IQuestionProvider
    {
        string dotlessLetters = "أ ا ى ر س ل ص ع ه ح د م ك ط ئ ء ؤ و", prevLetter="", newLetterString="X";

        public IQuestionPack GetNextQuestion()
        {
            return null;
        }

        public SickLettersQuestionsPack SickLettersGetNextQuestion()
        {
            LL_LetterData newLetter;

            prevLetter = newLetterString;
            do
            {
                newLetter = AppManager.Instance.Teacher.GimmeARandomLetter();
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
                newLetter = AppManager.Instance.Teacher.GimmeARandomLetter();
                newLetterString = newLetter.TextForLivingLetter.ToString();

            }
            while (newLetterString == "" || dotlessLetters.Contains(newLetterString) || newLetterString == prevLetter);

            SickLettersQuestionsPack dataPack = new SickLettersQuestionsPack(newLetter);
            return newLetter;
        }

    }
}