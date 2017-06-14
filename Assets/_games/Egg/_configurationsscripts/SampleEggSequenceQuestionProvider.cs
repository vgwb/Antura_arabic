using System.Collections.Generic;
using EA4S.Core;
using EA4S.MinigamesAPI;
using EA4S.MinigamesAPI.Sample;

namespace EA4S.Minigames.Egg
{
    public class SampleEggSequenceQuestionProvider : IQuestionProvider
    {
        public SampleEggSequenceQuestionProvider()
        {
        }

        public IQuestionPack GetNextQuestion()
        {
            ILivingLetterData questionSentence = null;

            List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

            while (correctAnswers.Count < 8)
            {
                var letter = (AppManager.Instance as AppManager).Teacher.GetRandomTestLetterLL();

                if (!CheckIfContains(correctAnswers, letter))
                {
                    correctAnswers.Add(letter);
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
