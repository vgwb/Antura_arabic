using System.Collections.Generic;

namespace EA4S.Egg
{
    public class QuestionManager
    {
        EggGame game;

        List<ILivingLetterData> corrects = new List<ILivingLetterData>();
        List<ILivingLetterData> wrongs = new List<ILivingLetterData>();

        public QuestionManager(EggGame game)
        {
            this.game = game;
        }

        public void StartNewQuestion()
        {
            ILivingLetterData lLetterData = EggConfiguration.Instance.QuestionProvider.GetNextData();
        }
    }
}