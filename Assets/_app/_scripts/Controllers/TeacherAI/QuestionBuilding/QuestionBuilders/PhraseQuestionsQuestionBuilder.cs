using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class PhraseQuestionsQuestionBuilder : IQuestionBuilder
    {
        // Focus: Phrases
        // pack history filter: TODO
        // journey: TODO

        private int nPacks;
        private int nCorrect;
        private int nWrong;

        public PhraseQuestionsQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
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

            // Get a question phrase at random
            var question = teacher.wordHelper.GetPhrasesByCategory(Db.PhraseDataCategory.Question).RandomSelectOne();

            // Get the linked reply phrase
            var reply = teacher.wordHelper.GetLinkedPhraseOf(question);

            var correctAnswers = new List<Db.PhraseData>();
            correctAnswers.Add(reply);

            // Get random wrong phrases
            var wrongAnswers = teacher.wordHelper.GetPhrasesNotIn(question, reply).RandomSelect(nWrong);

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