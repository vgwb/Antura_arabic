// Modified by: Daniele Giardini - 2016/11/15

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace EA4S
{
    [RequireComponent(typeof(RectTransform))]
    public class ActionFeedbackComponent : MonoBehaviour
    {
        public Sprite YesSprite, NoSprite;

        Image img;
        Tween showTween;

        void Awake()
        {
            img = this.GetComponent<Image>();

            showTween = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(this.transform.DOPunchScale(Vector3.one * 0.4f, 0.5f, 12))
                .AppendInterval(0.45f)
                .Append(this.transform.DOScale(0.001f, 0.3f).SetEase(Ease.InQuart))
                .OnComplete(() => this.gameObject.SetActive(false))
                .OnPlay(() => this.gameObject.SetActive(true));

            showTween.Complete();
        }

        void OnDestroy()
        {
            showTween.Kill();
        }

        /// <summary>
        /// Show feedback positive or negative.
        /// </summary>
        /// <param name="_feedback"></param>
        public void Show(bool _feedback)
        {
            img.sprite = _feedback ? YesSprite : NoSprite;
            showTween.Restart();
        }
    }
}