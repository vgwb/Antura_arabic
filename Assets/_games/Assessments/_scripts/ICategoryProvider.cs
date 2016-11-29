namespace EA4S.Assessment
{
    public interface ICategoryProvider
    {
        int GetCategories();
        ILivingLetterData Category(int i);
        UnityEngine.GameObject SpawnCustomObject( int currentCategory, bool question);
    }
}
