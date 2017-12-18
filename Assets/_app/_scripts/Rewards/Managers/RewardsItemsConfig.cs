using System;
using System.Collections.Generic;

namespace Antura.Rewards
{
    /// <summary>
    /// Defines the relationship between unlocks and 
    /// </summary>
    [Serializable]
    public class RewardsUnlocksConfig
    {
        public List<PlaySessionRewardUnlock> PlaySessionRewardsUnlock; 
    }


    /// <summary>
    /// Defines all the possible items that can be unlocked.
    /// </summary>
    [Serializable]
    public class RewardsItemsConfig
    {
        // A Reward is made of 1 of each of the following PARTS
        // the combinations of 2 of these is a REWARD

        // These are all the different models
        public List<RewardProp> PropBases;                    // model
        public List<RewardColor> PropColors;             // model color

        // decals are just a different type of Reward (so they should be MERGED)
        public List<RewardDecal> DecalBases;          // decal
        public List<RewardColor> DecalColors;     // decal color

        // textures are just a different type of Reward (so they should be MERGED)
        public List<RewardTexture> TextureBases;            // tiled texture
        public List<RewardColor> TextureColors;      // tiled texture color

        // TODO: separate from the above stuff
        public List<PlaySessionRewardUnlock> PlaySessionRewardsUnlock;  // unlocks at which PS?

        public RewardsItemsConfig GetClone()
        {
            return MemberwiseClone() as RewardsItemsConfig;
        }
    }
}