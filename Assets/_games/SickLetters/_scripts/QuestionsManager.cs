using UnityEngine;
using System.Collections;
namespace EA4S.SickLetters
{
    public class QuestionsManager
    {
        string dotlessLetters = " - ﻻ لأ ﺉ آ ٶ ى ر س ل ص ع ه ح د م ك ط ئ ء ؤ و �ة", prevLetter = "";

        public ILivingLetterData getNewLetter()
        {

            ILivingLetterData newLetter = null;
            
            do
            {
                if(newLetter!=null)
                    prevLetter = newLetter.TextForLivingLetter;
                IQuestionProvider newQuestionProvider = SickLettersConfiguration.Instance.Questions;
                IQuestionPack nextQuestionPack = newQuestionProvider.GetNextQuestion(); 

                foreach (ILivingLetterData letterData in nextQuestionPack.GetCorrectAnswers())
                {
                    newLetter = letterData;
                    
                }
            }
            while (dotlessLetters.Contains(newLetter.TextForLivingLetter) || newLetter.TextForLivingLetter == prevLetter);

            //ILivingLetterData newLetter = ((SampleQuestionPack)nextQuestionPack).GetQuestions();

            return newLetter;
        }
    }
}
