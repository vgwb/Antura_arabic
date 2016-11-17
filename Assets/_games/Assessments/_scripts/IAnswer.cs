using UnityEngine;

namespace EA4S.Assessment
{
    public interface IAnswer
    {
        /// <summary>
        /// Access the GameObject of this answer
        /// </summary>
        GameObject gameObject { get; }

        /// <summary>
        /// Is this a correct answer?
        /// </summary>
        bool IsCorrect();

        /// <summary>
        /// This answer match the question number "set"
        /// </summary>
        /// <param name="set">which set </param>
        void SetAnswerSet( int set);

        /// <summary>
        /// Get answer set
        /// </summary>
        int GetAnswerSet();
    }
}
