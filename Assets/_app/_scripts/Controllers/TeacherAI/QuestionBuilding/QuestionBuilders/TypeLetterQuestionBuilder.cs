using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{
    public class TypeLetterQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;

        public TypeLetterQuestionBuilder(int nPacks)
        {
            this.nPacks = nPacks;
        }

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            List<QuestionPackData> packs = new List<QuestionPackData>();
            var teacher = AppManager.Instance.Teacher;

            var db = AppManager.Instance.DB;

            var consonantWord = db.GetWordDataById("consonant");
            var vowelWord = db.GetWordDataById("vowel");

            int nPerType = nPacks / 2;
            var wordsConsonant = teacher.wordHelper.GetConsonantLetter().RandomSelect(nPerType);
            var wordsVowel = teacher.wordHelper.GetVowelLetter().RandomSelect(nPerType);

            foreach (var word in wordsConsonant)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(consonantWord);
                wrongWords.Add(vowelWord);

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

            foreach (var word in wordsVowel)
            {
                var correctWords = new List<Db.WordData>();
                var wrongWords = new List<Db.WordData>();
                correctWords.Add(vowelWord);
                wrongWords.Add(consonantWord);

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