namespace EA4S.Assessment
{
    public interface ICategoryProvider
    {
        int GetCategories();
        bool Compare(int i, ILivingLetterData providedByQuestionBuilder);
        UnityEngine.GameObject SpawnCustomObject( int currentCategory);
    }
}
