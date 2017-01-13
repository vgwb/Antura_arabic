using UnityEngine;

namespace EA4S
{

    /// <summary>
    /// Common interface for data that can appear on a LivingLetter object.
    /// </summary>
    public interface ILivingLetterData
    {
        LivingLetterDataType DataType { get; }
        string TextForLivingLetter { get; }
        [System.Obsolete("Use DrawingCharForLivingLetter instead of this.")]
        Sprite DrawForLivingLetter { get; }
        string DrawingCharForLivingLetter { get; }
        string Id { get; set; }
        bool Equals(ILivingLetterData data);
    }

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
