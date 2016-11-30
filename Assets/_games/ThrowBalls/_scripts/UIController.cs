using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;

namespace EA4S.ThrowBalls
{
    public class UIController : MonoBehaviour
    {
        public static UIController instance;

        private const float CRACK_FADE_DELAY = 0.5f;
        private const float CRACK_FADE_DURATION = 1.5f;

        public GameObject letterHint;
        public WordFlexibleContainer wordFlexibleContainer;

        public GameObject crack;

        private Image crackImage;
        private Color crackImageColor;

        void Awake()
        {
            instance = this;

            crackImage = crack.GetComponent<Image>();
            crackImageColor = crackImage.color;
        }

        public void SetLetterHint(ILivingLetterData _data)
        {
            wordFlexibleContainer.SetText(_data);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void OnScreenCracked()
        {
            StartCoroutine(CrackAnimationCoroutine());
        }

        private IEnumerator CrackAnimationCoroutine()
        {
            AudioManager.I.PlaySfx(Sfx.ScreenHit);

            crackImageColor.a = 1;
            crackImage.color = crackImageColor;

            yield return new WaitForSeconds(CRACK_FADE_DELAY);

            float crackFadeStartTime = Time.time;
            float sinFactor = 2 * Mathf.PI * Mathf.Pow(CRACK_FADE_DURATION, -1);

            while (crackImageColor.a > 0)
            {
                crackImageColor.a = Mathf.Cos(sinFactor * (Time.time - crackFadeStartTime));
                crackImage.color = crackImageColor;

                yield return new WaitForFixedUpdate();
            }

            crackImageColor.a = 0;
            crackImage.color = crackImageColor;
        }
        
        public void Reset()
        {
            crackImageColor.a = 0;
            crackImage.color = crackImageColor;
        }
    }
}

