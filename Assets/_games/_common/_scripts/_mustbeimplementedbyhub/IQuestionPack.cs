using System.Collections.Generic;
using EA4S.Core;

namespace EA4S.MinigamesAPI
{
    /// <summary>
    /// Interface for learning content data in the form:
    /// - a question
    /// - a set of correct answers
    /// - a set of wrong answers
    /// </summary>
    public interface IQuestionPack : IGameData
    {
        ILivingLetterData GetQuestion();
        IEnumerable<ILivingLetterData> GetQuestions();
        IEnumerable<ILivingLetterData> GetWrongAnswers();
        IEnumerable<ILivingLetterData> GetCorrectAnswers();
    }
}
