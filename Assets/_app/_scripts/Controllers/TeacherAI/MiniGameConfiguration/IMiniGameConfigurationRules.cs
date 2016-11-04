
namespace EA4S.MiniGameConfiguration
{
    /// <summary>
    /// Defines how question packs are generated for a specific mini game
    /// </summary>
    public interface IMiniGameConfigurationRules
    {
        int GetQuestionPackCount();
        IQuestionPack CreateQuestionPack(); // DEPRECATED
        QuestionPackData CreateQuestionPackData();
    }

}