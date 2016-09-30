using System;
using System.Collections.Generic;

namespace EA4S
{
    /// <summary>
    /// This sample class generates 10 quizzes of type "I give you a word, you say what words it contains"
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
            for (int i = 0; i < 10; i++)
            {
                List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
                List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

                WordData newWordData = AppManager.Instance.Teacher.GimmeAGoodWordData();
                foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(newWordData.Word, AppManager.Instance.Letters))
                {
                    correctAnswers.Add(letterData);
                }


                // At least 4 wrong letters
                while (wrongAnswers.Count < 4)
                {
                    var letter = AppManager.Instance.Teacher.GimmeARandomLetter();

                    if (!correctAnswers.Contains(letter))
                    {
                        wrongAnswers.Add(letter);
                    }
                }

                var currentPack = new SampleQuestionPack(newWordData, wrongAnswers, correctAnswers);
                questions.Add(currentPack);
            }
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
