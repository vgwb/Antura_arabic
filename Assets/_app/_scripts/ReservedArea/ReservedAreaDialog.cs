using UnityEngine;
using System.Collections.Generic;
using EA4S.UI;
using EA4S.Helpers;

namespace EA4S.ReservedArea
{
    /// <summary>
    /// Pop-up that allows access to the reserved area with a parental lock.
    /// </summary>
    public class ReservedAreaDialog : MonoBehaviour
    {
        public TextRender englishTextUI;
        public TextRender arabicTextUI;

        private int firstButtonClickCounter;

        private const int nButtons = 4;

        private int firstButtonIndex;
        private int secondButtonIndex;
        private int firstButtonClicksTarget;

        void OnEnable()
        {
            firstButtonClickCounter = 0;

            // Selecting two buttons at random
            var availableIndices = new List<int>();
            for (var i = 0; i < nButtons; i++)
                availableIndices.Add(i);
            var selectedIndices = availableIndices.RandomSelect(2);
            firstButtonIndex = selectedIndices[0];
            secondButtonIndex = selectedIndices[1];

            // Number of clicks at random
            const int min_number = 4;
            const int max_number = 8;
            firstButtonClicksTarget = Random.Range(min_number, max_number);

            // Update text
            string[] numberWords = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            string[] buttonWords = { "green", "red", "blue", "yellow" };
            string[] buttonWordsArabic = { "الأخضر", "الأحمر", "الأزرق", "الأصفر" };

            string numberWord = numberWords[firstButtonClicksTarget - 1];
            string firstButtonWord = buttonWords[firstButtonIndex];
            string secondButtonWord = buttonWords[secondButtonIndex];

            englishTextUI.text =
                "<b>RESERVED AREA</b>" +
                "\n\nTo unlock the section, press " + numberWord + " times the " + firstButtonWord + " button, then press the " + secondButtonWord + " one once." +
                "\n\nIf you make an error, retry by re - accessing this panel";

            string numberWordArabic = AppManager.I.DB.GetWordDataById("number_0" + firstButtonClicksTarget).Arabic;
            string firstButtonWordArabic = buttonWordsArabic[firstButtonIndex];
            string secondButtonWordArabic = buttonWordsArabic[secondButtonIndex];

            string arabicIntroduction = "";
            arabicIntroduction += "المحجوز\n\n";
            arabicIntroduction += string.Format("لفتح الباب، اضغط {1} مرات على الزر {0}.", firstButtonWordArabic, numberWordArabic);
            arabicIntroduction += string.Format("\nثم، اضغط على الزر {0} مرة واحدة.", secondButtonWordArabic);
            arabicIntroduction += "\n\n إذا قمت بإجراء خطأ، إعادة المحاولة من خلال إعادة الوصول إلى هذا الفريق";

            Debug.Log(arabicIntroduction);
            arabicTextUI.text = arabicIntroduction;

            /*              + numberWordArabic + "اضغط\n\n" +
                          " مرات على الزر "
                          + firstButtonWordArabic +
                          "، ثم اضغط على واحد "+ secondButtonWordArabic +" مرة واحدة.";
     // arabicTextUI.text += "\n \n إذا جعل خطأ، إعادة المحاولة خلال إعادة - الوصول إلى هذا الفريق";
     */
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void OnButtonClick(int buttonIndex)
        {
            if (buttonIndex == firstButtonIndex) {
                firstButtonClickCounter++;
            } else if (buttonIndex == secondButtonIndex) {
                if (firstButtonClickCounter == firstButtonClicksTarget) {
                    UnlockReservedArea();
                } else {
                    firstButtonClickCounter = firstButtonClicksTarget + 1; // disabling
                }
            }
        }

        void UnlockReservedArea()
        {
            AppManager.I.NavigationManager.GoToReservedArea();
        }
    }
}