using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{

    public class RandomLettersQuestionBuilder : IQuestionBuilder
    {
        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private bool firstCorrectIsQuestion;
        private PackListHistory packListHistory;

        public RandomLettersQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, bool firstCorrectIsQuestion = false,
            PackListHistory packListHistory = PackListHistory.NoFilter)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.firstCorrectIsQuestion = firstCorrectIsQuestion;
            this.packListHistory = packListHistory;
        }

        private List<string> previousPacksIDs = new List<string>();

        public List<QuestionPackData> CreateAllQuestionPacks()
        {
            previousPacksIDs.Clear();

            List<QuestionPackData> packs = new List<QuestionPackData>();
            for (int pack_i = 0; pack_i < nPacks; pack_i++)
            {
                var pack = CreateSingleQuestionPackData();
                packs.Add(pack);
            }
            return packs;
        }

        private QuestionPackData CreateSingleQuestionPackData()
        {
            var teacher = AppManager.Instance.Teacher;

            var correctLetters = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetAllLetters(),
                    new SelectionParameters(SelectionSeverity.AsManyAsPossible, nCorrect,
                        packListHistory: this.packListHistory, filteringIds: previousPacksIDs)
                );
            previousPacksIDs.AddRange(correctLetters.ConvertAll(x => x.GetId()).ToArray());

            var wrongLetters = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetLettersNotIn(correctLetters.ToArray()),
                    new SelectionParameters(SelectionSeverity.AsManyAsPossible, nWrong, ignoreJourney:true)
                );

            var question = firstCorrectIsQuestion ? correctLetters[0] : null;

            // Debug
            if (ConfigAI.verboseTeacher)
            {
                string debugString = "--------- TEACHER: question pack result ---------";
                debugString += "\nCorrect Letters: " + correctLetters.Count;
                foreach (var l in correctLetters) debugString += " " + l;
                debugString += "\nWrong Letters: " + wrongLetters.Count;
                foreach (var l in wrongLetters) debugString += " " + l;
                UnityEngine.Debug.Log(debugString);
            }

            return QuestionPackData.Create(question, correctLetters, wrongLetters);
        }

    }
}