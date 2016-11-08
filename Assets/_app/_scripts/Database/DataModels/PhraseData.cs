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
        public string Linked;
        public string[] Words; // TODO @Michele : parse list of words tht are in the phrase

        public override string ToString()
        {
            return Id + ": " + English;
        }

        public string GetId()
        {
            return Id;
        }
    }
}