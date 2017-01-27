using System.Collections.Generic;

namespace EA4S.Teacher
{
    /// <summary>
    /// Defines rules on how question packs can be generated for a specific mini game.
    /// </summary>
    public interface IQuestionBuilder
    {
        List<QuestionPackData> CreateAllQuestionPacks();
    }

}