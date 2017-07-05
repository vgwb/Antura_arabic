using Antura.MinigamesCommon;

namespace Antura.Assessment
{
    public interface IAssessmentConfiguration: IGameConfiguration
    {
        int NumberOfRounds { get; set; }

        // Internal use only
        int SimultaneosQuestions { get; }
    }
}
