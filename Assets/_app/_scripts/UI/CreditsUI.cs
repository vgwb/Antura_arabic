// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/29

using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CreditsUI : MonoBehaviour
    {
        [Tooltip("Units x second for the scroll animation")]
        public float ScrollAnimationSpeed = 100;
        public float ScrollAnimationDelay = 1.5f;
        [Header("References")]
        public RectTransform CreditsContainer;
        public UIButton BtBack;
        public TMPro.TextMeshProUGUI CreditsText;

        public bool HasAwoken { get; private set; }
        RectTransform rectT;
        Vector2 defCreditsContainerPos;
        Tween showTween, scrollTween;

        #region Unity

        void Awake()
        {
            HasAwoken = true;
            defCreditsContainerPos = CreditsContainer.anchoredPosition;
            rectT = this.GetComponent<RectTransform>();

            showTween = this.GetComponent<CanvasGroup>().DOFade(0, 0.4f).From().SetEase(Ease.Linear).SetUpdate(true).SetAutoKill(false).Pause()
                .OnRewind(() => this.gameObject.SetActive(false));

            this.gameObject.SetActive(false);

            // Listeners
            BtBack.Bt.onClick.AddListener(OnClick);

            CreditsText.text = (Resources.Load("Credits") as TextAsset).text;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) StopScrollLoop();
            else if (Input.GetMouseButtonUp(0) && showTween.IsComplete()) StartScrollLoop();
        }

        void OnDestroy()
        {
            this.StopAllCoroutines();
            showTween.Kill();
            scrollTween.Kill();
            BtBack.Bt.onClick.RemoveAllListeners();
        }

        #endregion

        #region Public Methods

        public void Show(bool _doShow)
        {
            scrollTween.Kill();
            this.StopAllCoroutines();
            if (_doShow) {
                this.gameObject.SetActive(true);
                CreditsContainer.anchoredPosition = defCreditsContainerPos;
                showTween.PlayForward();
                StartScrollLoop();
            } else {
                showTween.PlayBackwards();
            }
        }

        void StartScrollLoop()
        {
            this.StartCoroutine(CO_StartScrollLoop());
        }

        IEnumerator CO_StartScrollLoop()
        {
            yield return null;

            float toY = CreditsContainer.rect.height - rectT.rect.height;
            scrollTween = CreditsContainer.DOAnchorPosY(toY, ScrollAnimationSpeed).SetSpeedBased()
                .SetEase(Ease.Linear).SetDelay(ScrollAnimationDelay).SetUpdate(true);
        }

        void StopScrollLoop()
        {
            this.StopAllCoroutines();
            scrollTween.Kill();
        }

        #endregion

        #region Callbacks

        void OnClick()
        {
            Show(false);
        }

        #endregion
    }
}