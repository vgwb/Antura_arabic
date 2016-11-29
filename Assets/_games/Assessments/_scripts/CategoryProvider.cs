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
        const string sunString = "the_sun";
        const string moonString = "the_moon";

        public CategoryProvider( CategoryType type)
        {
            categoryType = type;
            sun = GatherData( sunString);
            moon = GatherData( moonString);
        }

        private ILivingLetterData GatherData( string id)
        {
            var db = AppManager.I.DB;
            return db.GetWordDataById( id).ConvertToLivingLetterData();
        }

        public string Category(int currentCategory)
        {
            switch (categoryType)
            {
                case CategoryType.SunMoon:
                    if (currentCategory == 0)
                        return sun.TextForLivingLetter;
                    else
                        return moon.TextForLivingLetter;

                default:
                    throw new NotImplementedException();
            }
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

        private GameObject QuestionAnswer( ILivingLetterData data, bool question)
        {
            if (question)
                return LivingLetterFactory.Instance.SpawnQuestion( data).gameObject;
            else
                return LivingLetterFactory.Instance.SpawnAnswer( data).gameObject;
        }

        public GameObject SpawnCustomObject( int currentCategory, bool question)
        {

            switch (categoryType)
            {
                case CategoryType.SunMoon:
                    if (currentCategory == 0)
                        return QuestionAnswer( sun, question);
                    else
                        return QuestionAnswer( moon, question);

                default:
                    throw new NotImplementedException();
            }

        }
    }
}
