namespace EA4S.Database
{
    /// <summary>
    /// Interface for a vocabulary data element (i.e. letters, words, etc.)
    /// </summary>
    public interface IVocabularyData : IData
    {
        float GetIntrinsicDifficulty();
    }

}