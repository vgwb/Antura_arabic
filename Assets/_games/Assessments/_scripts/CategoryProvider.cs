using System;
using UnityEngine;
using EA4S.Db;

namespace EA4S.Assessment
{
    public enum CategoryType
    {
        SunMoon,
        SingularPlural,
    }

    public class CategoryProvider : ICategoryProvider
    {
        CategoryType categoryType;
        ILivingLetterData sun;
        ILivingLetterData moon;

        public CategoryProvider( CategoryType type)
        {
            categoryType = type;
        }

        public string Category( int i)
        {
            return null;
        }

        public int GetCategories()
        {
            switch (categoryType)
            {
                case CategoryType.SunMoon:
                    return 2;
                default:
                    throw new NotImplementedException();
            }
        }

        public GameObject SpawnCustomObject( int currentCategory)
        {
            throw new NotImplementedException();
        }
    }
}
