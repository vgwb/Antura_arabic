using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class LocalizationData : IData
    {
        public string Id;
        public string Character;
        public string Context;
        public string English;
        public string Arabic;
        public string AudioFile;
        public string EnglishOld;

        public string GetId()
        {
            return Id;
        }
    }
}