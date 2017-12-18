using System;

namespace Antura.Rewards
{
    /// <summary>
    /// A single piece of reward.
    /// </summary>
    [Serializable]
    public class RewardData
    {
        public string ID;
    }

    [Serializable]
    public class RewardProp : RewardData
    {
        public string RewardName;
        public string BoneAttach;
        public string Material1;
        public string Material2;
        public string Category;
    }

    [Serializable]
    public class RewardDecal : RewardData
    {
    }

    [Serializable]
    public class RewardTexture : RewardData 
    {
    }
}