using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    public class WalkieTalkie : MonoBehaviour
    {
        public GameObject Button, ButtonPressed;

        public bool isShown { get; private set; }

        bool makePulse;
        Tween showTween, pulseTween, btTween;

        void Awake()
        {
            RectTransform rt = this.GetComponent<RectTransform>();

            const float pulseDuration = 0.3f;
            pulseTween = DOTween.Sequence().SetUpdate(true).SetAutoKill(false).SetLoops(2, LoopType.Yoyo).Pause()
                .Append(rt.DOScale(1.05f, pulseDuration).SetEase(Ease.InOutQuad))
                .Join(rt.DORotate(new Vector3(0, 0, 12), pulseDuration, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.InOutSine));
            pulseTween.OnComplete(() => {
                if (makePulse) pulseTween.Restart();
            });

            const float showDuration = 0.3f;
            showTween = DOTween.Sequence().SetUpdate(true).SetAutoKill(false).Pause()
                .Append(rt.DOScale(0.1f, showDuration).From().SetEase(Ease.OutBack))
                .Join(rt.DORotate(new Vector3(0, 0, 45f), showDuration, RotateMode.FastBeyond360).From().SetEase(Ease.OutBack))
                .OnPlay(() => this.gameObject.SetActive(true))
                .OnRewind(() => {
                    if (pulseTween != null) pulseTween.Rewind();
                    this.gameObject.SetActive(false);
                });

            btTween = DOTween.Sequence().SetUpdate(true).SetAutoKill(false).Pause()
                .AppendCallback(() => {
                    Button.gameObject.SetActive(false);
                    ButtonPressed.gameObject.SetActive(true);
                })
                .InsertCallback(0.2f, () => {
                    Button.gameObject.SetActive(true);
                    ButtonPressed.gameObject.SetActive(false);
                });

            this.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            showTween.Kill();
            pulseTween.Kill();
            btTween.Kill();
        }

        public void Show(bool doShow)
        {
            if (isShown == doShow) return;

            isShown = doShow;
            pulseTween.Pause();
            if (doShow) showTween.PlayForward();
            else showTween.PlayBackwards();
        }

        public void StartPulsing(bool pressButton = false)
        {
            if (!isShown) return;

            if (!showTween.IsComplete()) showTween.OnComplete(DOStartPulsing);
            else DOStartPulsing();
            if (pressButton) btTween.Restart();
        }
        void DOStartPulsing()
        {
            makePulse = true;
            pulseTween.Restart();
        }

        public void StopPulsing()
        {
            if (!isShown) return;

            makePulse = false;
            showTween.OnComplete(null);
        }
    }
}