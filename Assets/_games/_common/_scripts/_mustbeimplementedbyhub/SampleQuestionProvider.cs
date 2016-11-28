using System;
using System.Collections.Generic;
using System.Linq;

namespace EA4S
{
    /// <summary>
    /// This sample class generates 32 quizzes of type "I give you a word, you say what letters it contains"
    /// </summary>
    public class SampleQuestionProvider : IQuestionProvider
    {
        List<SampleQuestionPack> questions = new List<SampleQuestionPack>();
        string description;

        int currentQuestion;

        public SampleQuestionProvider()
        {
            currentQuestion = 0;

            description = "Questions description";

            // 10 QuestionPacks
            for (int i = 0; i < 32; i++)
            {
                List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
                List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

                LL_WordData newWordData = AppManager.I.Teacher.GetRandomTestWordDataLL();
                //LL_WordData newWordData = AppManager.I.Teacher.GetRandomTestWordDataLL(new WordFilters(requireDiacritics: true));

                //LL_WordData newWordData = new LL_WordData(AppManager.I.DB.GetWordDataById("welcome"));
                

                if (newWordData == null)
                    return;

                foreach (var letterData in ArabicAlphabetHelper.ExtractLetterDataFromArabicWord(newWordData.Data.Arabic))
                {
                    correctAnswers.Add(letterData);
                }

                correctAnswers = correctAnswers.Distinct().ToList();

                // At least 4 wrong letters
                while (wrongAnswers.Count < 4)
                {
                    var letter = AppManager.I.Teacher.GetRandomTestLetterLL();

                    if (!CheckIfContains(correctAnswers, letter) && !CheckIfContains(wrongAnswers, letter))
                    {
                        wrongAnswers.Add(letter);
                    }
                }

                var currentPack = new SampleQuestionPack(newWordData, wrongAnswers, correctAnswers);
                questions.Add(currentPack);
            }
        }

        static bool CheckIfContains(List<ILivingLetterData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Id == letter.Id)
                    return true;
            return false;
        }

        public string GetDescription()
        {
            return description;
        }

        IQuestionPack IQuestionProvider.GetNextQuestion()
        {
            currentQuestion++;

            if (currentQuestion >= questions.Count)
                currentQuestion = 0;

            return questions[currentQuestion];
        }
    }
}
