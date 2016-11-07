
namespace EA4S
{
    /// <summary>
    /// Defines how question packs are generated for a specific mini game
    /// </summary>
    public interface IQuestionBuilder
    {
        // @todo: add a local history of packs (so we do not pick the same elements multiple times)
        int GetQuestionPackCount();
        IQuestionPack CreateQuestionPack(); // DEPRECATED
        QuestionPackData CreateQuestionPackData();
    }

}