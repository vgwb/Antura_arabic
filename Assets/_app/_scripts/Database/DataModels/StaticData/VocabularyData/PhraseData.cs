using System;
using EA4S.MinigamesAPI;

namespace EA4S.Database
{
    /// <summary>
    /// Data defining a Phrase.
    /// This is one of the fundamental dictionary (i.e. learning content) elements.
    /// <seealso cref="WordData"/>
    /// <seealso cref="LetterData"/>
    /// </summary>
    [Serializable]
    public class PhraseData : IVocabularyData, IConvertibleToLivingLetterData
    {
        public string Id;
        public bool Active;
        public string English;
        public string Arabic;
        public PhraseDataCategory Category;
        public string Linked;
        public string[] Words;
        public string[] Answers;
        public float Complexity;

        public float GetIntrinsicDifficulty()
        {
            return Complexity;
        }

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