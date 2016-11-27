using System.Collections.Generic;

namespace EA4S
{
    /// <summary>
    /// Defines how question packs are generated for a specific mini game
    /// </summary>
    public interface IQuestionBuilder
    {
        List<QuestionPackData> CreateAllQuestionPacks();
    }

}