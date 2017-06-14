using System.Collections.Generic;
using System.Linq;
using EA4S.Core;
using EA4S.Helpers;
using EA4S.MinigamesAPI;
using EA4S.MinigamesAPI.Sample;

namespace EA4S.Minigames.ReadingGame
{
    public class SampleReadingGameQuestionProvider : IQuestionProvider
    {
        public SampleReadingGameQuestionProvider()
        {

        }

        IQuestionPack IQuestionProvider.GetNextQuestion()
        {
            var answerData = (AppManager.Instance as AppManager).DB.GetWordDataByRandom();
            LL_WordData randomWord = new LL_WordData(answerData.Id, answerData);

            StringTestData fakeData = new StringTestData(
                 ArabicAlphabetHelper.ProcessArabicString(
                     "منذ لم نرك منذ مدة " + randomWord.Data.Arabic + " منذ مدة" +
                      "منذ لم نرك منذ مدة " +
                        "منذ لم نرك منذ مدة "));

            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();
            while (wrongAnswers.Count < 6)
            {
                var randomData = (AppManager.Instance as AppManager).DB.GetWordDataByRandom();

                if (randomData.Id != answerData.Id && !wrongAnswers.Any((a) => { return a.Id == randomData.Id; }))
                {
                    wrongAnswers.Add(randomData.ConvertToLivingLetterData());
                }
            }
            
            return new SampleQuestionPack(fakeData, wrongAnswers, new ILivingLetterData[] { randomWord });
        }
    }
}
