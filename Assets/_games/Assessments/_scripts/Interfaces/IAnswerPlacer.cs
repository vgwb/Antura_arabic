namespace EA4S.Assessment
{
    public interface IAnswerPlacer
    {
        void Place(IAnswer[] answer);
        bool IsAnimating();
        void RemoveAnswers();
    }
}