using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class WordsInPhraseQuestionBuilder : IQuestionBuilder
    {
        // Focus: Words & Phrases
        // pack history filter: TODO
        // journey: TODO

        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private bool useAllCorrectWords;

        public WordsInPhraseQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, bool useAllCorrectWords = false)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.useAllCorrectWords = useAllCorrectWords;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            for (int pack_i = 0; pack_i < nPacks; pack_i++)
            {
                packs.Add(CreateSingleQuestionPackData());
            }
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;

            // Get a phrase
            var question = teacher.wordHelper.GetAllPhrases().RandomSelectOne();

            // Get words of that phrases
            var phraseWords = teacher.wordHelper.GetWordsInPhrase(question);

            var correctAnswers = new List<Db.WordData>(phraseWords);
            if (!useAllCorrectWords) correctAnswers = phraseWords.RandomSelect(nCorrect);

            var wrongAnswers = teacher.wordHelper.GetWordsNotIn(phraseWords.ToArray()).RandomSelect(nWrong);

            if (ConfigAI.verboseTeacher)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nQuestion: " + question;
                debugString += "\nCorrect Answers: " + correctAnswers.Count;
                foreach (var l in correctAnswers) debugString += " " + l;
                debugString += "\nWrong Answers: " + wrongAnswers.Count;
                foreach (var l in wrongAnswers) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.Create(question, correctAnswers, wrongAnswers);
        }


    }
}