namespace EA4S.Assessment
{
    public interface IPlaceholder
    {

        bool IsAnswered();

        bool IsAnswerCorrect();

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
