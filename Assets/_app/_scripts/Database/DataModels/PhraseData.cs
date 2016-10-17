using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class PhraseData : IData
    {
        public string Id;
        public string English;
        public string Arabic;

        public string GetID()
        {
            return Id;
        }
    }
}