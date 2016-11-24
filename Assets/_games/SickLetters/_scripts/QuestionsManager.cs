using UnityEngine;
using System.Collections;
namespace EA4S.SickLetters
{
    public class QuestionsManager
    {
        public ILivingLetterData getNewLetter()
        {
            ILivingLetterData newLetter = null;

            IQuestionProvider newQuestionProvider = SickLettersConfiguration.Instance.Questions;
            IQuestionPack nextQuestionPack = newQuestionProvider.GetNextQuestion();

            foreach (ILivingLetterData letterData in nextQuestionPack.GetCorrectAnswers())
            {
                newLetter = letterData;
            }

            return newLetter;
        }
    }
}
