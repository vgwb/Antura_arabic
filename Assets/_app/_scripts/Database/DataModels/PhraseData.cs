using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class PhraseData : IData, IConvertibleToLivingLetterData
    {
        public string Id;
        public bool Active;
        public string English;
        public string Arabic;
        public PhraseDataCategory Category;
        public string Linked;
        public string[] Words;
        public string[] Answers;

        public override string ToString()
        {
            return Id + ": " + English;
        }

        public string GetId()
        {
            return Id;
        }

        public ILivingLetterData ConvertToLivingLetterData()
        {
            return new LL_PhraseData(GetId(), this);
        }
    }
}