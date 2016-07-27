using UnityEngine;
using System.Collections;
namespace EA4S {
    /// <summary>
    /// Common interface for living letter data.
    /// </summary>
    public interface ILivingLetterData {
        LivingLetterDataType DataType { get; }
        string TextForLivingLetter { get; }
        Sprite DrawForLivingLetter { get; }
        string Key { get; set; }
    }

    public enum LivingLetterDataType {
        Letter,
        Word,
    }
}
