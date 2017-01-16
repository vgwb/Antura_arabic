namespace EA4S.Assessment
{
    public interface IAssessmentConfiguration: IGameConfiguration
    {
        int Rounds { get; }
        int SimultaneosQuestions { get; }
    }
}
