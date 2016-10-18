using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class LocalizationData : IData
    {
        public string Id { get; set; }
        public string Character { get; set; }
        public string Context { get; set; }
        public string English { get; set; }
        public string Arabic { get; set; }
        public string AudioFile { get; set; }
        public string EnglishOld { get; set; }

        public string GetId()
        {
            return Id;
        }
    }
}