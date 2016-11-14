using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModularFramework.Helpers;

namespace EA4S
{
    /// <summary>
    /// This sample class generates 10 quizzes of type "I give you 2 words, you find the common letter(s)"
    /// </summary>
    public class SampleWordsWithCommonLettersProvider : IQuestionProvider
    {
        List<SampleWordsWithCommonLettersPack> questions = new List<SampleWordsWithCommonLettersPack>();
        string description;

        int currentQuestion;
        readonly int quizzesCount = 10;

        public SampleWordsWithCommonLettersProvider()
        {
            currentQuestion = 0;
            description = "Questions description";

            List <ILivingLetterData> correctAnswers;
            List <ILivingLetterData> wrongAnswers;

            LL_WordData newWordData1;
            LL_WordData newWordData2;
            List<ILivingLetterData> wordLetters1 = new List<ILivingLetterData>();
            List<ILivingLetterData> wordLetters2 = new List<ILivingLetterData>();
            List<ILivingLetterData> commonLetters = new List<ILivingLetterData>();
            List<ILivingLetterData> uncommonLetters = new List<ILivingLetterData>();

            for (int iteration = 0; iteration < quizzesCount; iteration++)
            {
                // Get 2 words with at least 1 common letter
                do
                {
                    newWordData1 = null;
                    newWordData2 = null;
                    wordLetters1.Clear();
                    wordLetters2.Clear();
                    commonLetters.Clear();
                    uncommonLetters.Clear();

                    newWordData1 = AppManager.Instance.Teacher.GetRandomTestWordDataLL();
                    foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(newWordData1.Data.Arabic, AppManager.Instance.Teacher.GetAllTestLetterDataLL()))
                    {
                        wordLetters1.Add(letterData);
                    }

                    do
                    {
                        newWordData2 = AppManager.Instance.Teacher.GetRandomTestWordDataLL();
                    } while(newWordData2.Key == newWordData1.Key);

                    foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(newWordData2.Data.Arabic, AppManager.Instance.Teacher.GetAllTestLetterDataLL()))
                    {
                        wordLetters2.Add(letterData);
                    }


                    // Find common letter(s) (without repetition)
                    for (int i = 0; i < wordLetters1.Count; i++)
                    {
                        var letter = wordLetters1[i];

                        if (wordLetters2.Contains(letter))
                        {
                            if (!commonLetters.Contains(letter))
                            {
                                commonLetters.Add(letter);
                            }
                        }
                    }

                    // Find uncommon letters (without repetition)
                    for (int i = 0; i < wordLetters1.Count; i++)
                    {
                        var letter = wordLetters1[i];

                        if (!wordLetters2.Contains(letter))
                        {
                            if (!uncommonLetters.Contains(letter))
                            {
                                uncommonLetters.Add(letter);
                            }
                        }
                    }
                    for (int i = 0; i < wordLetters2.Count; i++)
                    {
                        var letter = wordLetters2[i];

                        if (!wordLetters1.Contains(letter))
                        {
                            if (!uncommonLetters.Contains(letter))
                            {
                                uncommonLetters.Add(letter);
                            }
                        }
                    }
                } while(commonLetters.Count == 0);

                commonLetters.Shuffle();
                uncommonLetters.Shuffle();

                correctAnswers = new List<ILivingLetterData>(commonLetters);
                wrongAnswers = new List<ILivingLetterData>(uncommonLetters);

                var currentPack = new SampleWordsWithCommonLettersPack(newWordData1, newWordData2, wrongAnswers, correctAnswers);
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

