using System;

namespace EA4S.Database
{
    /// <summary>
    /// Data defining a Reward that can be earned for customization of Antura.
    /// </summary>
    [Serializable]
    public class RewardData : IData
    {
        public string Id;
        public string Title;
        public RewardDataCategory Category;
        public int Weight;

        public override string ToString()
        {
            return Id + ": " + Title;
        }

        public string GetId()
        {
            return Id;
        }
    }

}