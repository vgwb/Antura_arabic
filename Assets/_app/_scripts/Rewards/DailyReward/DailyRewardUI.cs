using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Antura.Rewards
{
    public class DailyRewardUI : MonoBehaviour
    {
        public Sprite bonesSprite;

        public Image imageUI;
        public TextMeshProUGUI textUI;

        public Image lockUI;

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
                default:
                    throw new ArgumentOutOfRangeException("rewardType", rewardType, null);
            }
        }

        private void SetRewardAmount(int amount)
        {
            textUI.text = amount.ToString();
        }

        public void SetLocked()
        {
            lockUI.gameObject.SetActive(true);
        }

        public void SetUnlocked()
        {
            lockUI.gameObject.SetActive(false);
        }
    }

}