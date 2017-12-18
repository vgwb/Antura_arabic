using System;
using System.Collections.Generic;
using Antura.Database;

namespace Antura.Rewards
{
    /// <summary>
    /// Defines the relationship between unlocks and 
    /// </summary>
    [Serializable]
    public class RewardsUnlocksConfig
    {
        public List<RewardUnlocksAtJourneyPosition> PlaySessionRewardsUnlock; 
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


        public IEnumerable<RewardBase> GetBasesForType(RewardBaseType type) 
        {
            switch (type)
            {
                case RewardBaseType.Prop:
                    foreach (var b in PropBases) yield return b;
                    break;
                case RewardBaseType.Decal:
                    foreach (var b in DecalBases) yield return b;
                    break;
                case RewardBaseType.Texture:
                    foreach (var b in TextureBases) yield return b;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public IEnumerable<RewardColor> GetColorsForType(RewardBaseType type)
        {
            switch (type)
            {
                case RewardBaseType.Prop:
                    foreach (var col in PropColors) yield return col;
                    break;
                case RewardBaseType.Decal:
                    foreach (var col in DecalColors) yield return col;
                    break;
                case RewardBaseType.Texture:
                    foreach (var col in TextureColors) yield return col;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        // TODO: separate from the above stuff
        public List<RewardUnlocksAtJourneyPosition> PlaySessionRewardsUnlock;  // unlocks at which PS?

        public RewardsItemsConfig GetClone()
        {
            return MemberwiseClone() as RewardsItemsConfig;
        }
    }

    /// <summary>
    /// A single reward pack (i.e. an effective rewards that can be obtained)
    /// </summary>
    [Serializable]
    public class RewardPack
    {
        public string baseId;
        public string colorId;
        public RewardBaseType baseType;

        public string UniqueId
        {
            get { return baseId + "_" + colorId; }
        }

        public RewardPackUnlockData unlockData;

        public override string ToString()
        {
            return UniqueId;
        }
    }
}