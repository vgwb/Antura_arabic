using UnityEngine;

namespace EA4S.MinigamesAPI
{
    /// <summary>
    /// Common interface for data that can appear on a LivingLetter object.
    /// This represents a piece of learning content (dictionary data) as viewed through a LivingLetter character.
    /// </summary>
    public interface ILivingLetterData
    {
        LivingLetterDataType DataType { get; }

        /// <summary>
        /// Text to display on the living letter
        /// </summary>
        string TextForLivingLetter { get; }

        [System.Obsolete("Use DrawingCharForLivingLetter instead of this.")]
        Sprite DrawForLivingLetter { get; }

        /// <summary>
        /// Character to display on the living letter (using a custom font)
        /// </summary>
        string DrawingCharForLivingLetter { get; }

        /// <summary>
        /// Identification string for the specific data.
        /// </summary>
        // TODO refactor: this should be part of the inner data
        string Id { get; set; }

        bool Equals(ILivingLetterData data);
    }
}

namespace EA4S
{
    /// <summary>
    /// Type of data that can appear on a LivingLetter object.
    /// </summary>
    public enum LivingLetterDataType
    {
        Letter,
        Word,
        Image,
        Phrase,
    }
}
