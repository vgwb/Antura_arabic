using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class RewardData : IData
    {
        public string Id;
        public string Title;
        public RewardDataCategory Category;
        public int Weight; // TODO @Michele

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