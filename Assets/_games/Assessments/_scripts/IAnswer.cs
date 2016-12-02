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
        /// Compare content of the answer
        /// </summary>
        /// <param name="other"> other answer content</param>
        bool Equals( IAnswer other);

        /// <summary>
        /// The data of the living letter
        /// </summary>
        ILivingLetterData Data();
    }
}
