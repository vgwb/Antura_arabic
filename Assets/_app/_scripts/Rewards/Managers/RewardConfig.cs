using System;
using System.Collections.Generic;

namespace Antura.Rewards
{
    [Serializable]
    public class RewardConfig
    {
        // A Reward is made of 1 of each of the following PARTS
        // the combinations of 2 of these is a REWARD

        // These are all the different models
        public List<Reward> Rewards;                    // model
        public List<RewardColor> RewardsColorPairs;     // model color

        // decals are just a different type of Reward (so they should be MERGED)
        public List<RewardDecal> RewardsDecal;          // decal
        public List<RewardColor> RewardsDecalColor;     // decal color

        // textures are just a different type of Reward (so they should be MERGED)
        public List<RewardTile> RewardsTile;            // tiled texture
        public List<RewardColor> RewardsTileColor;      // tiled texture color


        public List<PlaySessionRewardUnlock> PlaySessionRewardsUnlock;  // unlocks at which PS?

        public RewardConfig GetClone()
        {
            return MemberwiseClone() as RewardConfig;
        }
    }
}