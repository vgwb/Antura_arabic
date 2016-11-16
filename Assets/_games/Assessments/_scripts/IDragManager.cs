using System.Collections.Generic;

namespace EA4S.Assessment
{
    public interface IDragManager
    {
        void ResetRound();
        bool AllAnswered();
        void Enable();
        void AddElements(   List< PlaceholderBehaviour> placeholders,
                            List< AnswerBehaviour> answers);
    }
}
