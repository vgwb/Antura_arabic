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

        public TextRender dayTextUI;

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
            lockUI.gameObject.SetActive(true);
        }

        public void SetUnlocked()
        {
            lockUI.gameObject.SetActive(false);
        }

        public void SetDay(int day)
        {
            dayTextUI.text = "Day " + day;
        }
    }

}