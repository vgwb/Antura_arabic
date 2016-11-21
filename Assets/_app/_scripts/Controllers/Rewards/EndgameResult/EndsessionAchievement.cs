// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/21

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class EndsessionAchievement : MonoBehaviour
    {
        public Color LockedColor = Color.red;
        [Header("References")]
        public Transform Star;
        public Image RewardBg;
        public RawImage Reward;
        public Image Lock;
        public Sprite UnlockedSprite;

        public bool IsAchieved { get; private set; }
        bool hasReward;
        Sprite lockedSprite;
        Color defColor;
        Sequence achieveTween;

        #region Unity

        void Awake()
        {
            hasReward = RewardBg.gameObject.activeSelf;
            lockedSprite = Lock.sprite;
            defColor = RewardBg.color;
            RewardBg.color = LockedColor;
            Star.gameObject.SetActive(false);

            achieveTween = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(Star.DOPunchScale(Vector3.one * 1.25f, 0.4f));
            if (hasReward) {
                achieveTween.InsertCallback(0.2f, () => Lock.sprite = UnlockedSprite)
                    .Join(Lock.transform.DOPunchScale(Vector3.one * 1.3f, 0.2f))
                    .Insert(0.4f, Lock.transform.DOScale(0.0001f, 0.25f).SetEase(Ease.InQuad).OnComplete(() => Lock.gameObject.SetActive(false)))
                    .Join(Lock.transform.DORotate(new Vector3(0, 0, 220), 0.25f, RotateMode.FastBeyond360))
                    .Join(RewardBg.DOColor(defColor, 0.25f).SetEase(Ease.Linear));
            }
        }

        #endregion

        #region Public Methods

        internal void Achieve(bool _doAchieve)
        {
            if (IsAchieved == _doAchieve) return;

            IsAchieved = _doAchieve;
            if (hasReward) {
                Lock.sprite = lockedSprite;
                Lock.gameObject.SetActive(true);
            }
            if (_doAchieve) {
                Star.gameObject.SetActive(true);
                achieveTween.Restart();
            } else {
                Star.gameObject.SetActive(false);
                achieveTween.Rewind();
            }
        }

        #endregion
    }
}