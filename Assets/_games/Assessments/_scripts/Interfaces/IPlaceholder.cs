namespace EA4S.Assessment
{
    public interface IPlaceholder
    {
        void SetQuestion( IQuestion question);

        IQuestion GetQuestion();
    }
}
