using System;
using System.Globalization;
using Antura.UI;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Antura.Rewards
{
    public class DailyRewardUI : MonoBehaviour
    {
        public Sprite bonesSprite;
        public Sprite test1Sprite;
        public Sprite test2Sprite;

        public Image imageUI;
        public TextMeshProUGUI amountTextUI;

        public Image lockUI;
        public Image unlockUI;

        public TextRender dayTextUI;

        void Awake()
        {
            // @note: Lock is not used anymore, we hide it
            lockUI.gameObject.SetActive(false);
        }

        public void SetReward(DailyRewardManager.DailyReward reward)
        {
            SetRewardType(reward.rewardType);
            SetRewardAmount(reward.amount);
        }

        private void SetRewardType(DailyRewardType rewardType)
        {
            switch (rewardType)
            {
                case DailyRewardType.Bones:
                    imageUI.sprite = bonesSprite;
                    break;
                case DailyRewardType.Test1:
                    imageUI.sprite = test1Sprite;
                    break;
                case DailyRewardType.Test2:
                    imageUI.sprite = test2Sprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("rewardType", rewardType, null);
            }
        }

        private void SetRewardAmount(int amount)
        {
            amountTextUI.text = amount.ToString();
        }

        public void SetLocked()
        {
            unlockUI.gameObject.SetActive(false);
        }

        public void SetUnlocked()
        {
            unlockUI.gameObject.SetActive(true);
        }

        public void SetDay(int day)
        {
            dayTextUI.text = "Day " + day;
        }

        public void HideDay()
        {
            dayTextUI.gameObject.SetActive(false);
        }
    }

}