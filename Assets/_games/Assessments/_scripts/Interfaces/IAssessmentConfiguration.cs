namespace EA4S.Assessment
{
    public interface IAssessmentConfiguration: IGameConfiguration
    {
        int Rounds { get; set; }
        int SimultaneosQuestions { get; set; }
    }
}