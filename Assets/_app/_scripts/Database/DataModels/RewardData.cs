using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class RewardData
    {
        public string Id;
        public string Title;
        public RewardCategory Category;
    }

    public enum RewardCategory
    {
        DogProp,
        DogSkin,
        PlayerTitle
    }

}