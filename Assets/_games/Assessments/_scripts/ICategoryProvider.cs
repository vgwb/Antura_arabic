namespace EA4S.Assessment
{
    public interface ICategoryProvider
    {
        int GetCategories();
        string Category(int i);
        UnityEngine.GameObject SpawnCustomObject(int currentCategory);
    }
}
