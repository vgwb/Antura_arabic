namespace EA4S.Tobogan
{
    interface IQuestionProvider
    {
        IQuestion GetNextQuestion();
        string GetDescription();
    }
}