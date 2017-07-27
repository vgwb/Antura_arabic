using System.Collections.Generic;

namespace Antura.Rewards
{
    public class DailyRewardManager
    {
        public class DailyReward
        {
            public DailyRewardType rewardType;
            public int amount;

            public DailyReward(DailyRewardType rewardType, int amount)
            {
                this.rewardType = rewardType;
                this.amount = amount;
            }
        }

        private List<DailyReward> rewards;
        public int MaxComboDays { get; set; }

        public IEnumerable<DailyReward> GetRewards()
        {
            foreach (var reward in rewards)
            {
                yield return reward;
            }
        }

        public DailyReward GetReward(int i)
        {
            return rewards[i];
        }

        public DailyRewardManager()
        {
            rewards = new List<DailyReward>();
            MaxComboDays = 3;
            rewards.Add(new DailyReward(DailyRewardType.Bones, 5));
            rewards.Add(new DailyReward(DailyRewardType.Bones, 10));
            rewards.Add(new DailyReward(DailyRewardType.Bones, 20));
        }

    }
}