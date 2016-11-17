using System.Collections.Generic;

namespace EA4S.Assessment
{
    public interface IAnswerChecker
    {
        bool IsAnimating();
        void Check( List< PlaceholderBehaviour> placeholders, IDragManager dragManager);
        bool AllCorrect();
    }
}
