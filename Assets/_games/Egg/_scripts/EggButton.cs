using ArabicSupport;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace EA4S.Egg
{
    public class EggButton : MonoBehaviour
    {
        public TextMeshProUGUI buttonText;
        public Image buttonImage;

        public Color colorStandard;
        public Color colorLightUp;

        Tween colorTweener;

        public ILivingLetterData livingLetterData { get; private set; }

        public void SetAnswer(ILivingLetterData livingLetterData)
        {
            this.livingLetterData = livingLetterData;

            if (livingLetterData.DataType == LivingLetterDataType.Letter)
            {
                buttonText.gameObject.SetActive(true);

                buttonText.text = ArabicAlphabetHelper.GetLetterFromUnicode(((LetterData)livingLetterData).Isolated_Unicode);
            }
            else if (livingLetterData.DataType == LivingLetterDataType.Letter)
            {
                buttonText.gameObject.SetActive(true);

                buttonText.text = ArabicFixer.Fix(((WordData)livingLetterData).Word, false, false);
            }
        }

        public void LightUp()
        {
            if (colorTweener != null)
                colorTweener.Kill();

            colorTweener = DOTween.To(() => buttonImage.color, x => buttonImage.color = x, colorLightUp, 0.2f).OnComplete(delegate ()
            {
                colorTweener = DOTween.To(() => buttonImage.color, x => buttonImage.color = x, colorStandard, 0.2f);
            });
        }
    }
}