using EA4S.API;
using System.Collections.Generic;

namespace EA4S.MiniGameConfiguration
{
    public class Egg_MiniGameConfigurationRules : IMiniGameConfigurationRules
    {
        // Configuration
        private int packsCount = 4;

        private int nWrongToSelect = 7;
        private Db.WordDataCategory wordDataCategory = Db.WordDataCategory.BodyPart;

        public int GetQuestionPackCount()
        {
            return packsCount;
        }

        public IQuestionPack CreateQuestionPack()
        {
            var teacher = AppManager.Instance.Teacher;

            QuestionPackData questionPackData = new QuestionPackData(); // TODO: use this instead of the below lists
            Db.WordData question;
            List<Db.LetterData> correctAnswers = new List<Db.LetterData>();
            List<Db.LetterData> wrongAnswers = new List<Db.LetterData>();

            // Logic
            question = teacher.wordHelper.GetWordsByCategory(wordDataCategory).RandomSelectOne();
            correctAnswers.AddRange(teacher.wordHelper.GetLettersInWord(question.Id));  // @todo: UNTIL THIS WORKS (must still be implemented in the parser) IT WON'T WORK!!!
            wrongAnswers.AddRange(teacher.wordHelper.GetLettersNotIn(correctAnswers.ToArray()).RandomSelect(nWrongToSelect));

            // Conversion (TO BE MOVED OUTSIDE OF HERE, AS THE LAST STEWP OF THE CONFIGURATION GENERATOR)
            ILivingLetterData ll_question = teacher.BuildWordData_LL(question);
            List<ILivingLetterData> ll_wrongAnswers = teacher.BuildLetterData_LL_Set(wrongAnswers);
            List<ILivingLetterData> ll_correctAnswers = teacher.BuildLetterData_LL_Set(correctAnswers);
            IQuestionPack questionPack = new FindRightDataQuestionPack(ll_question, ll_wrongAnswers, ll_correctAnswers);

            // Test debug log
            var debugString = "";
            debugString += (question) +"\n";
            debugString += "WRONG: " + (wrongAnswers.Count) + "\n";
            foreach (var ans in wrongAnswers) debugString += (ans) + "\n";
            debugString += "CORRECT: " + (correctAnswers.Count) + "\n";
            foreach (var ans in correctAnswers) debugString += (ans) + "\n";
            UnityEngine.Debug.Log(debugString);

            // TODO: The AI in definitive version must check the difficulty threshold (0.5f in example) to determine gameplayType without passing wrongAnswers
            //if (difficulty < 0.5f) {

            return questionPack;
        }

    }
}