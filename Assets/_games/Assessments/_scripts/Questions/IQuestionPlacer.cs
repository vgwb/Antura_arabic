using Kore.Coroutines;

namespace EA4S.Assessment
{
    public interface IQuestionPlacer
    {
        void Place(IQuestion[] question, bool playQuestionSound);
        bool IsAnimating();
        void RemoveQuestions();
        IYieldable PlayQuestionSound();
    }
}
