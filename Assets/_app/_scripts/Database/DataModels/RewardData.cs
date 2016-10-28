using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class RewardData : IData
    {
        public string Id;
        public string Title;
        public RewardCategory Category;

        public override string ToString()
        {
            return Id + ": " + Title;
        }

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