using System.Collections.Generic;

namespace EA4S.Assessment
{
    internal class MatchLettersToWordProvider_Tester : IQuestionProvider
    {
        private int correct;
        private int rounds;
        private int simultaneos;
        private int wrong;
        private IQuestionBuilder builder;

        private List<IQuestionPack> questions;

        public MatchLettersToWordProvider_Tester(int rounds, int simultaneos, int correct, int wrong)
        {
            this.rounds = rounds;
            this.simultaneos = simultaneos;
            this.correct = correct;
            this.wrong = wrong;
            currentQuestion = 0;
            builder = new LettersInWordQuestionBuilder(nPacks: rounds*simultaneos, useAllCorrectLetters: true, nWrong: 2, nCorrect:1);
            QuestionPacksGenerator packGenerator = new QuestionPacksGenerator();
            questions = packGenerator.GenerateQuestionPacks(builder);
        }

        int currentQuestion;
        IQuestionPack IQuestionProvider.GetNextQuestion()
        {
            currentQuestion++;

            if (currentQuestion >= questions.Count)
                currentQuestion = 0;

            return questions[currentQuestion];
        }
    }
}
