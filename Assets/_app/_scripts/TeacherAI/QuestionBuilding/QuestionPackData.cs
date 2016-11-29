using EA4S.API;
using System.Collections.Generic;

namespace EA4S
{

    public class QuestionPackData
    {
        public IConvertibleToLivingLetterData question;
        public List<IConvertibleToLivingLetterData> questions;  // to be used instead 
        public List<IConvertibleToLivingLetterData> correctAnswers;
        public List<IConvertibleToLivingLetterData> wrongAnswers;
        
        public static QuestionPackData CreateFromCorrect<T1>(IConvertibleToLivingLetterData question, List<T1> correctAnswers) where T1 : IConvertibleToLivingLetterData
        {
            QuestionPackData questionPackData = new QuestionPackData();
            questionPackData.question = question;
            questionPackData.correctAnswers = correctAnswers.ConvertAll<IConvertibleToLivingLetterData>(x => (IConvertibleToLivingLetterData)x);
            return questionPackData;
        }

        public static QuestionPackData CreateFromWrong<T2>(IConvertibleToLivingLetterData question, List<T2> wrongAnswers) where T2 : IConvertibleToLivingLetterData
        {
            QuestionPackData questionPackData = new QuestionPackData();
            questionPackData.question = question;
            questionPackData.wrongAnswers = wrongAnswers.ConvertAll<IConvertibleToLivingLetterData>(x => (IConvertibleToLivingLetterData)x);
            return questionPackData;
        }

        public static QuestionPackData Create<T>(IConvertibleToLivingLetterData question, List<T> correctAnswers, List<T> wrongAnswers) where T : IConvertibleToLivingLetterData
        {
            QuestionPackData questionPackData = new QuestionPackData();
            questionPackData.question = question;
            questionPackData.correctAnswers = correctAnswers.ConvertAll<IConvertibleToLivingLetterData>(x => (IConvertibleToLivingLetterData)x);
            questionPackData.wrongAnswers = wrongAnswers.ConvertAll<IConvertibleToLivingLetterData>(x => (IConvertibleToLivingLetterData)x);
            return questionPackData;
        }

        public static QuestionPackData Create<T1,T2>(List<T1> questions, List<T2> correctAnswers, List<T2> wrongAnswers) where T1 : IConvertibleToLivingLetterData where T2 : IConvertibleToLivingLetterData
        {
            QuestionPackData questionPackData = new QuestionPackData();
            questionPackData.questions = questions.ConvertAll<IConvertibleToLivingLetterData>(x => (IConvertibleToLivingLetterData)x);
            questionPackData.correctAnswers = correctAnswers.ConvertAll<IConvertibleToLivingLetterData>(x => (IConvertibleToLivingLetterData)x);
            questionPackData.wrongAnswers = wrongAnswers.ConvertAll<IConvertibleToLivingLetterData>(x => (IConvertibleToLivingLetterData)x);
            return questionPackData;
        }

        public override string ToString()
        {
            var debugString = "";
            debugString += "Question: " + (question) + "\n";
            debugString += "Wrong: " + (wrongAnswers.Count) + "\n";
            foreach (var ans in wrongAnswers) debugString += (ans) + "\n";
            debugString += "Correct: " + (correctAnswers.Count) + "\n";
            foreach (var ans in correctAnswers) debugString += (ans) + "\n";
            return debugString;
        }
    }

}
