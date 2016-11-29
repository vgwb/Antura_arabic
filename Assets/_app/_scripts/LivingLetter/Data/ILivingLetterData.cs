using UnityEngine;
using System.Collections;

namespace EA4S
{

    /// <summary>
    /// Common interface for living letter data.
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

    public enum LivingLetterDataType
    {
        Letter,
        Word,
        Image,
        Phrase,
    }
}
