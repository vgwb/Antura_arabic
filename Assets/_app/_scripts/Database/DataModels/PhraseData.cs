using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class PhraseData : IData
    {
        public string Id { get; set; }
        public string English { get; set; }
        public string Arabic { get; set; }

        public string GetId()
        {
            return Id;
        }
    }
}