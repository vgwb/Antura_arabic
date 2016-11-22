using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    public enum QuestionType
    {
        LivingLetter,
        LivingLetterAndImage,
        Sun,            // Arabic only
        Moon,           // Arabic only
        Singular,
        Dual,           // Arabic only
        Plural,
        Vowel,
        Consonant
    }

    /// <summary>
    /// Interface to abstract away questions in assessments
    /// </summary>
    public interface IQuestion
    {
        /// <summary>
        /// Information about the displayed's question symbol
        /// </summary>
        QuestionType Type();

        /// <summary>
        /// Returns LivingLetter data if QuestionType is LivingLetter, otherwise null
        /// </summary>
        ILivingLetterData LetterData();

        /// <summary>
        /// Returns the living letter with the Image (this image is for questions
        /// that have both image and some text), if you have only 1 image you should
        /// use LetterData instead.
        /// </summary>
        ILivingLetterData Image();

        /// <summary>
        /// Number of Placeholders  for correct answers
        /// </summary>
        /// <returns> size in LL units (1 LL is 3 world's unit)</returns>
        int PlaceholdersCount();

        /// <summary>
        /// Returns the letter size (which varies: 1, 1.5 or 2)
        /// </summary>
        /// <returns>Size in world units</returns>
        float LetterSize();

        /// <summary>
        /// Access the GameObject of this question
        /// </summary>
        GameObject gameObject { get; }

        /// <summary>
        /// Add a placeholder to this question
        /// </summary>
        void TrackPlaceholder( GameObject gameObject);

        /// <summary>
        /// Get all tracked placeholders
        /// </summary>
        /// <returns> Enumerable to placeholders set</returns>
        IEnumerable< GameObject> GetPlaceholders();

    }
}
