using System.Collections.Generic;
using EA4S.Core;
using EA4S.MinigamesAPI;
using EA4S.MinigamesAPI.Sample;

namespace EA4S.Minigames.Egg
{
    public class SampleEggSingleQuestionProvider : IQuestionProvider
    {
        public SampleEggSingleQuestionProvider()
        {
        }

        public IQuestionPack GetNextQuestion()
        {
            ILivingLetterData questionSentence = null;

            List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

            correctAnswers.Add(AppManager.I.Teacher.GetRandomTestLetterLL());

            while (wrongAnswers.Count < 8)
            {
                var letter = AppManager.I.Teacher.GetRandomTestLetterLL();

                if (!CheckIfContains(correctAnswers, letter) && !CheckIfContains(wrongAnswers, letter))
                {
                    wrongAnswers.Add(letter);
                }
            }

            return new SampleQuestionPack(questionSentence, wrongAnswers, correctAnswers);
        }

        static bool CheckIfContains(List<ILivingLetterData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Id == letter.Id)
                    return true;
            return false;
        }
    }
}
