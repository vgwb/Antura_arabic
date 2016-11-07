namespace EA4S
{
    public interface IGameConfiguration
    {
        IGameContext Context { get; }
        IQuestionBuilder SetupBuilder();
    }
}