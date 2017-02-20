using EA4S.Helpers;
using System.Collections;
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

        /// <summary>
        /// Returns a coroutine which creates a string with text that flashes over frames, with an option to mark some of the text.
        /// </summary>
        public static IEnumerator GetWordWithFlashingText(Database.WordData arabicWord, int indexToFlash, Color flashColor, float cycleDuration, int numCycles, System.Action<string> callback, Color markColor, bool markPrecedingLetters = false)
        {
            string text = ArabicAlphabetHelper.ProcessArabicString(arabicWord.Arabic);

            string markTagStart = "<color=#" + GenericHelper.ColorToHex(markColor) + ">";
            string markTagEnd = "</color>";

            float timeElapsed = 0f;
            int numCompletedCycles = 0;

            float halfDuration = cycleDuration * 0.5f;

            while (numCompletedCycles < numCycles)
            {
                float interpolant = timeElapsed < halfDuration ? timeElapsed / halfDuration : 1 - ((timeElapsed - halfDuration) / halfDuration);
                string flashTagStart = "<color=#" + GenericHelper.ColorToHex(Color.Lerp(Color.black, flashColor, interpolant)) + ">";
                string flashTagEnd = "</color>";

                string resultOfThisFrame = "";

                if (markPrecedingLetters)
                {
                    resultOfThisFrame += markTagStart;
                }
                resultOfThisFrame += text.Substring(0, indexToFlash);
                if (markPrecedingLetters)
                {
                    resultOfThisFrame += markTagEnd;
                }
                resultOfThisFrame += flashTagStart;
                resultOfThisFrame += text.Substring(indexToFlash, 1);
                resultOfThisFrame += flashTagEnd;
                if (indexToFlash + 1 < text.Length)
                {
                    resultOfThisFrame += text.Substring(indexToFlash + 1);
                }

                callback(resultOfThisFrame);

                timeElapsed += Time.fixedDeltaTime;
                if (timeElapsed >= cycleDuration)
                {
                    numCompletedCycles++;
                    timeElapsed = 0f;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        public static string GetWordWithMarkedText(Database.WordData arabicWord, Color color)
        {
            string tagStart = "<color=#" + GenericHelper.ColorToHex(color) + ">";
            string tagEnd = "</color>";

            string text = ArabicAlphabetHelper.ProcessArabicString(arabicWord.Arabic);

            return tagStart + text + tagEnd;
        }
    }
}
