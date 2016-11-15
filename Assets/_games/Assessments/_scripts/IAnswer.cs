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
    }
}
