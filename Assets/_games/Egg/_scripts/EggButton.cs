using ArabicSupport;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace EA4S.Egg
{
    public class EggButton : MonoBehaviour
    {
        public TextMeshProUGUI buttonText;
        public Image buttonImage;
        public Button button;

        public Color colorStandard;
        public Color colorLightUp;

        Tween colorTweener;

        public ILivingLetterData livingLetterData { get; private set; }

        Action<ILivingLetterData> onButtonPressed;

        public int positionIndex { get; set; }

        Action lightUpCallback;
        IAudioManager audioManager;

        bool inputEnabled = false;

        Action endMoveCallback;
        Action endScaleCallback;

        Tween moveTweener;
        Tween scaleTweener;

        public void Initialize(IAudioManager audioManager, Action<ILivingLetterData> onButtonPressed)
        {
            button.onClick.AddListener(OnButtonPressed);

            this.audioManager = audioManager;
            this.onButtonPressed = onButtonPressed;
        }

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

        public void LightUp(bool playAudio, float duration, float delay = 0f, Action callback = null)
        {
            lightUpCallback = callback;

            if (colorTweener != null)
                colorTweener.Kill();

            colorTweener = DOTween.To(() => buttonImage.color, x => buttonImage.color = x, colorLightUp, duration / 2f).OnComplete(delegate ()
            {
                colorTweener = DOTween.To(() => buttonImage.color, x => buttonImage.color = x, colorStandard, duration / 2f).OnComplete(delegate ()
                {
                    if (lightUpCallback != null)
                    {
                        lightUpCallback();
                    }
                });
            }).OnStart(delegate ()
            {
                if (livingLetterData.DataType == LivingLetterDataType.Letter)
                {
                    audioManager.PlayLetter(((LetterData)livingLetterData));
                }
                else if (livingLetterData.DataType == LivingLetterDataType.Word)
                {
                    audioManager.PlayWord(((WordData)livingLetterData));
                }
            }).SetDelay(delay);
        }

        void OnButtonPressed()
        {
            if (inputEnabled)
            {
                if (onButtonPressed != null)
                {
                    onButtonPressed(livingLetterData);
                }
            }
        }

        public void EnableInput()
        {
            inputEnabled = true;
        }

        public void DisableInput()
        {
            inputEnabled = false;
        }

        public void ScaleTo(float scale, float duration, float delay = 0f, Action endCallback = null)
        {
            endScaleCallback = endCallback;

            if(scaleTweener != null)
            {
                scaleTweener.Kill();
            }

            scaleTweener = transform.DOScale(scale, duration).SetDelay(delay).OnComplete(delegate () { if (endScaleCallback != null) endScaleCallback(); });
        }

        public void MoveTo(Vector3 position, float duration, float delay = 0f, Action endCallback = null)
        {
            endMoveCallback = endCallback;

            if(moveTweener != null)
            {
                moveTweener.Kill();
            }

            moveTweener = transform.DOLocalMove(position, duration).SetDelay(delay).OnComplete(delegate () { if (endMoveCallback != null) endMoveCallback(); });
        }

        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
        }
    }
}