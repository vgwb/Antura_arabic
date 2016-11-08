using System.Collections.Generic;

namespace EA4S
{
    /// <summary>
    /// A question pack: which includes a question (under the form of a letter, word or image),
    /// and a set of wrong answers and correct andwers of the same format.
    /// </summary>
   public interface IQuestionPack : IGameData {
        ILivingLetterData GetQuestion();
        IEnumerable<ILivingLetterData> GetQuestions();
        IEnumerable<ILivingLetterData> GetWrongAnswers();
        IEnumerable<ILivingLetterData> GetCorrectAnswers();
    }
}
