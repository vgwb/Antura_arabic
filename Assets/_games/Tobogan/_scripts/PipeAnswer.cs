using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S.Tobogan
{
    public class PipeAnswer : MonoBehaviour
    {
        public TMP_Text answerText;
        public Image answerImage;

        public bool IsCorrectAnswer { get; private set; }

        public void SetAnswer(ILivingLetterData livingLetterData, bool correct)
        {
            if(livingLetterData.DataType == LivingLetterDataType.Letter)
            {
                answerText.gameObject.SetActive(true);
                //answerImage.gameObject.SetActive(false);

                answerText.text = ArabicAlphabetHelper.GetLetterFromUnicode(((LetterData)livingLetterData).Isolated_Unicode);
            }
            else if (livingLetterData.DataType == LivingLetterDataType.Letter)
            {
                answerText.gameObject.SetActive(true);
                //answerImage.gameObject.SetActive(false);

                answerText.text = ((WordData)livingLetterData).Word;
            }
            //else
            //{
            //    answerImage.gameObject.SetActive(true);
            //    answerImage.sprite = livingLetterData.DrawForLivingLetter;

            //    answerText.gameObject.SetActive(false);
            //}
            IsCorrectAnswer = correct;
        }
    }
}