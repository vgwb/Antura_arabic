using EA4S.MinigamesAPI;

namespace EA4S.Minigames.SickLetters
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
