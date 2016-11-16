namespace EA4S.Assessment
{
    public interface IQuestionPlacer
    {
        void Place(IQuestion[] question);
        bool IsAnimating();
        void RemoveQuestions();
    }
}