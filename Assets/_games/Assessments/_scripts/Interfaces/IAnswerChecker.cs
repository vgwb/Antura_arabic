using System.Collections.Generic;

namespace EA4S.Assessment
{
    public interface IAnswerChecker
    {
        bool IsAnimating();
        bool AreAllAnswered( List< PlaceholderBehaviour> placeholders);

        void Check( List< PlaceholderBehaviour> placeholders, 
                    List< IQuestion> questions,
                    IDragManager dragManager);
        bool AllCorrect();
    }
}
