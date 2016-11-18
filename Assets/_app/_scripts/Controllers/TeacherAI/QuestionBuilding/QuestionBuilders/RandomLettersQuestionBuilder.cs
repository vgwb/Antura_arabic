using EA4S.Teacher;
using System.Collections.Generic;

namespace EA4S
{

    public class RandomLettersQuestionBuilder : IQuestionBuilder
    {
        // focus: Letters
        // pack history filter: parameterized
        // journey: enabled

        private int nPacks;
        private int nCorrect;
        private int nWrong;
        private bool firstCorrectIsQuestion;
        private PackListHistory correctChoicesHistory;
        private PackListHistory wrongChoicesHistory;
        private bool wrongIgnoreJourney;

        public RandomLettersQuestionBuilder(int nPacks, int nCorrect = 1, int nWrong = 0, bool firstCorrectIsQuestion = false,
            PackListHistory correctChoicesHistory = PackListHistory.NoFilter,
            PackListHistory wrongChoicesHistory = PackListHistory.NoFilter,
            bool wrongIgnoreJourney = false)
        {
            this.nPacks = nPacks;
            this.nCorrect = nCorrect;
            this.nWrong = nWrong;
            this.firstCorrectIsQuestion = firstCorrectIsQuestion;
            this.correctChoicesHistory = correctChoicesHistory;
            this.wrongChoicesHistory = wrongChoicesHistory;
            this.wrongIgnoreJourney = wrongIgnoreJourney;
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
                        packListHistory: correctChoicesHistory, filteringIds: previousPacksIDs)
                );

            var wrongLetters = teacher.wordAI.SelectData(
                () => teacher.wordHelper.GetLettersNotIn(correctLetters.ToArray()),
                    new SelectionParameters(SelectionSeverity.AsManyAsPossible, nWrong, ignoreJourney: wrongIgnoreJourney,
                     packListHistory: wrongChoicesHistory, filteringIds: previousPacksIDs)
                );

            var question = firstCorrectIsQuestion ? correctLetters[0] : null;

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