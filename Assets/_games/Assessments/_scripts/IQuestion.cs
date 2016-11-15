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
        /// Returns the living letter with the Image
        /// </summary>
        ILivingLetterData Image();

        /// <summary>
        /// Number of Placeholders  for correct answers
        /// </summary>
        /// <returns> size in LL units (1 LL is 3 world's unit)</returns>
        int PlaceholdersCount();

        /// <summary>
        /// Links the question GameObject
        /// </summary>
        void SetGameObject( GameObject gameObject);

        /// <summary>
        /// Access the GameObject of this question
        /// </summary>
        GameObject gameObject { get; }
    }
}
