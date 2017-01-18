namespace EA4S.Assessment
{
    public interface IAnswerPlacer
    {
        void Place(Answer[] answer);
        bool IsAnimating();
        void RemoveAnswers();
    }
}
