using Kore.Utils;

namespace EA4S.Assessment
{
    /// <summary>
    /// This class is implemented by each Assessment to customize logic and appareance
    /// </summary>
    public interface IAssessment
    {
        ILogicInjector LogicInjector { get; }
        IQuestionPlacer QuestionPlacer { get; }
        IAnswerPlacer AnswerPlacer { get; }
        IQuestionGenerator QuestionGenerator { get; }
        IAssessmentConfiguration Configuration { get; }
        IGameContext GameContext { get; }

        void StartGameSession( KoreCallback gameEndedCallback);
    }
}
