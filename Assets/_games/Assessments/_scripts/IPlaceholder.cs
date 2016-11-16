using UnityEngine;

namespace EA4S.Assessment
{
    public interface IPlaceholder
    {

        bool IsAnswered();

        bool IsAnswerCorrect();

        void SetAnswer(int i);

        void LinkAnswer(int i);
    }
}
