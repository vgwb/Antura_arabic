using EA4S.Helpers;
using UnityEngine;

namespace EA4S.UI
{
    public static class ArabicTextUtilities
    {
        public enum MarkType
        {
            SingleLetter,
            FromStartToLetter,
            FromLetterToEnd
        }

        /// <summary>
        /// Return a string of a word with the "color" tag enveloping a character. The word is already reversed and fixed for rendering.
        /// </summary>
        public static string GetWordWithMarkedLetterText(Database.WordData arabicWord, ArabicAlphabetHelper.ArabicStringPart letterToMark, Color color, MarkType type)
        {
            string tagStart = "<color=#" + GenericHelper.ColorToHex(color) + ">";
            string tagEnd = "</color>";

            string text = ArabicAlphabetHelper.ProcessArabicString(arabicWord.Arabic);

            string startText = text.Substring(0, letterToMark.fromCharacterIndex);
            string letterText = text.Substring(letterToMark.fromCharacterIndex, letterToMark.toCharacterIndex - letterToMark.fromCharacterIndex + 1);
            string endText = (letterToMark.toCharacterIndex >= text.Length - 1 ? "" : text.Substring(letterToMark.toCharacterIndex + 1));

            if (type == MarkType.SingleLetter)
                return startText + tagStart + letterText + tagEnd + endText;
            else if (type == MarkType.FromStartToLetter)
                return tagStart + startText + letterText + tagEnd + endText;
            else
                return startText + tagStart + letterText + endText + tagEnd;
        }
    }
}
