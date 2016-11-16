using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class FormWordsQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;

        public FormWordsQuestionBuilder(int nPacks)
        {
            this.nPacks = nPacks;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = AppManager.Instance.Teacher;

            var db = AppManager.Instance.DB;

            var singularWord = db.GetWordDataById("with_article");
            var pluralWord = db.GetWordDataById("without_article");
            var dualWord = db.GetWordDataById("without_article");

            int nPerType = nPacks / 3;
            var wordsSingular = teacher.wordHelper.GetWordsByForm(Db.WordDataForm.Singular).RandomSelect(nPerType);
            var wordsPlural = teacher.wordHelper.GetWordsByForm(Db.WordDataForm.Plural).RandomSelect(nPerType);
            var wordsDual = teacher.wordHelper.GetWordsByForm(Db.WordDataForm.Dual).RandomSelect(nPerType);

            foreach (var word in wordsSingular)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(singularWord);
                wrongWords.Add(pluralWord);
                wrongWords.Add(dualWord);

                if (ConfigAI.verboseTeacher)
                {
                    string debugString = "--------- TEACHER: question pack result ---------";
                    debugString += "\nQuestion: " + word;
                    debugString += "\nCorrect Word: " + correctWords.Count;
                    foreach (var l in correctWords) debugString += " " + l;
                    debugString += "\nWrong Word: " + wrongWords.Count;
                    foreach (var l in wrongWords) debugString += " " + l;
                    UnityEngine.Debug.Log(debugString);
                }

                var pack = QuestionPackData.Create(word, correctWords, wrongWords);
                packs.Add(pack);
            }

            foreach (var word in wordsPlural)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(pluralWord);
                wrongWords.Add(singularWord);
                wrongWords.Add(dualWord);

                if (ConfigAI.verboseTeacher)
                {
                    string debugString = "--------- TEACHER: question pack result ---------";
                    debugString += "\nQuestion: " + word;
                    debugString += "\nCorrect Word: " + correctWords.Count;
                    foreach (var l in correctWords) debugString += " " + l;
                    debugString += "\nWrong Word: " + wrongWords.Count;
                    foreach (var l in wrongWords) debugString += " " + l;
                    UnityEngine.Debug.Log(debugString);
                }

                var pack = QuestionPackData.Create(word, correctWords, wrongWords);
                packs.Add(pack);
            }

            foreach (var word in wordsDual)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(dualWord);
                wrongWords.Add(singularWord);
                wrongWords.Add(pluralWord);

                if (ConfigAI.verboseTeacher)
                {
                    string debugString = "--------- TEACHER: question pack result ---------";
                    debugString += "\nQuestion: " + word;
                    debugString += "\nCorrect Word: " + correctWords.Count;
                    foreach (var l in correctWords) debugString += " " + l;
                    debugString += "\nWrong Word: " + wrongWords.Count;
                    foreach (var l in wrongWords) debugString += " " + l;
                    UnityEngine.Debug.Log(debugString);
                }

                var pack = QuestionPackData.Create(word, correctWords, wrongWords);
                packs.Add(pack);
            }

            return packs;
        }

    }
}