using System.Linq;
using EA4S.MinigamesAPI;


namespace EA4S.Minigames.DancingDots
{
	public class DancingDotsQuestionsManager
	{
		private IQuestionProvider provider;

		public DancingDotsQuestionsManager()
		{
		    provider = DancingDotsConfiguration.Instance.Questions;
		}

		public ILivingLetterData getNewLetter()
		{
			var question = provider.GetNextQuestion();

			return question.GetCorrectAnswers().First();

		}
	}
}
