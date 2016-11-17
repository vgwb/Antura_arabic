namespace EA4S.Assessment
{
    public interface IPlaceholder
    {

        bool IsAnswered();

        bool IsAnswerCorrect();

        void SetQuestion( IQuestion question);

        IQuestion GetQuestion();

        /// <summary>
        /// "know-ahead correct answer
        /// </summary>
        void SetAnswer( int i);

        /// <summary>
        /// Value of linked (drag n' dropped) answer
        /// </summary>
        void LinkAnswer( int i);
    }
}
