using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class WordsByFormQuestionBuilder : IQuestionBuilder
    {
        // focus: Words
        // pack history filter: by default, all different
        // journey: enabled

        private int nPacks;
        SelectionSeverity severity;

        public WordsByFormQuestionBuilder(int nPacks, SelectionSeverity severity = SelectionSeverity.AsManyAsPossible)
        {
            this.nPacks = nPacks;
            this.severity = severity;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = AppManager.Instance.Teacher;

            var db = AppManager.Instance.DB;
            var choice1 = db.GetWordDataById("singular");
            var choice2 = db.GetWordDataById("plural");
            var choice3 = db.GetWordDataById("dual");

            int nPerType = nPacks / 3;

            var wordFilters = new WordFilters();
            wordFilters.excludePluralDual = false;

            var list_choice1 = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetWordsByForm(Db.WordDataForm.Singular, wordFilters),
                new SelectionParameters(severity, nPerType)
                );

            var list_choice2 = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetWordsByForm(Db.WordDataForm.Plural, wordFilters),
                new SelectionParameters(severity, nPerType)
                );

            var list_choice3 = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetWordsByForm(Db.WordDataForm.Dual, wordFilters),
                new SelectionParameters(severity, nPerType)
                );

            foreach (var word in list_choice1)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(choice1);
                wrongWords.Add(choice2);
                wrongWords.Add(choice3);

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

            foreach (var word in list_choice2)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(choice2);
                wrongWords.Add(choice1);
                wrongWords.Add(choice3);

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

            foreach (var word in list_choice3)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(choice3);
                wrongWords.Add(choice1);
                wrongWords.Add(choice2);

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