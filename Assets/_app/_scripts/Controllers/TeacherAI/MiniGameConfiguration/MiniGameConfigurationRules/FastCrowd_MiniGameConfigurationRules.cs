using EA4S.API;
using System.Collections.Generic;

namespace EA4S.MiniGameConfiguration
{
    public class FastCrowd_MiniGameConfigurationRules : IMiniGameConfigurationRules
    {
        // Configuration
        private int packsCount = 4;

        public int GetQuestionPackCount()
        {
            return packsCount;
        }

        public IQuestionPack CreateQuestionPack()
        {
            var teacher = AppManager.Instance.Teacher;

            QuestionPackData questionPackData = new QuestionPackData();

            Db.WordData question;
            List<Db.LetterData> correctAnswers = new List<Db.LetterData>();
            List<Db.LetterData> wrongAnswers = new List<Db.LetterData>();

            // Logic
            question = teacher.wordHelper.GetWordsByCategory(Db.WordDataCategory.BodyPart).RandomSelectOne();
            correctAnswers.AddRange(teacher.wordHelper.GetLettersInWord(question.Id));  // @todo: UNTIL THIS WORKS (must still be implemented in the parser) IT WON'T WORK!!!
            wrongAnswers.AddRange(teacher.wordHelper.GetLettersNotIn(correctAnswers.ToArray()));

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

            return questionPack;
        }

    }
}