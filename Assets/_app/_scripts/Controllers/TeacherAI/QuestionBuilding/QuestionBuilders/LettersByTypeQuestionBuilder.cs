using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class LettersByTypeQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters
        // pack history filter: by default, all different
        // journey: enabled

        private int nPacks;
        SelectionSeverity severity;

        public LettersByTypeQuestionBuilder(int nPacks, SelectionSeverity severity = SelectionSeverity.AsManyAsPossible)
        {
            this.nPacks = nPacks;
            this.severity = severity;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = AppManager.Instance.Teacher;

            var db = AppManager.Instance.DB;
            var choice1 = db.GetWordDataById("consonant");
            var choice2 = db.GetWordDataById("vowel");

            int nPerType = nPacks / 2;

            var list_choice1 = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetConsonantLetter(new LetterFilters()),
                new SelectionParameters(severity, nPerType) 
                );

            var list_choice2 = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetVowelLetter(new LetterFilters()),
                new SelectionParameters(severity, nPerType)
                );

            foreach (var data in list_choice1)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(choice1);
                wrongWords.Add(choice2);

                if (ConfigAI.verboseTeacher)
                {
                    string debugString = "--------- TEACHER: question pack result ---------";
                    debugString += "\nQuestion: " + data;
                    debugString += "\nCorrect Word: " + correctWords.Count;
                    foreach (var l in correctWords) debugString += " " + l;
                    debugString += "\nWrong Word: " + wrongWords.Count;
                    foreach (var l in wrongWords) debugString += " " + l;
                    UnityEngine.Debug.Log(debugString);
                }

                var pack = QuestionPackData.Create(data, correctWords, wrongWords);
                packs.Add(pack);
            }

            foreach (var data in list_choice2)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(choice2);
                wrongWords.Add(choice1);

                if (ConfigAI.verboseTeacher)
                {
                    string debugString = "--------- TEACHER: question pack result ---------";
                    debugString += "\nQuestion: " + data;
                    debugString += "\nCorrect Word: " + correctWords.Count;
                    foreach (var l in correctWords) debugString += " " + l;
                    debugString += "\nWrong Word: " + wrongWords.Count;
                    foreach (var l in wrongWords) debugString += " " + l;
                    UnityEngine.Debug.Log(debugString);
                }

                var pack = QuestionPackData.Create(data, correctWords, wrongWords);
                packs.Add(pack);
            }

            return packs;
        }

    }
}