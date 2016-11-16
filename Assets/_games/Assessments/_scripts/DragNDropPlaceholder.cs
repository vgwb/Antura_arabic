using System;

namespace EA4S.Assessment
{
    public class DragNDropPlaceholder : IPlaceholder
    {
        public bool IsAnswerCorrect()
        {
            return IsAnswered() && answerNumber == linkedAnswer;
        }

        public bool IsAnswered()
        {
            return linkedAnswer != 0;
        }

        int answerNumber = 0;
        public void SetAnswer( int answ)
        {
            answerNumber = answ;
        }

        int linkedAnswer = 0;
        public void LinkAnswer( int linked)
        {
            linkedAnswer = linked;
        }
    }
}
