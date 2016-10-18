using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class RewardData : IData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public RewardCategory Category { get; set; }

        public string GetId()
        {
            return Id;
        }
    }

    public enum RewardCategory
    {
        DogProp = 1,
        DogSkin = 2,
        PlayerTitle = 3
    }

}