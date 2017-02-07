using EA4S.MinigamesCommon;

namespace EA4S.Assessment
{
    public interface IAssessmentConfiguration: IGameConfiguration
    {
        int NumberOfRounds { get; set; }

        // Internal use only
        int Rounds { get;}

        // Internal use only
        int SimultaneosQuestions { get; }

    }
}
