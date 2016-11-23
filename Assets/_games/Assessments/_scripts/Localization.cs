namespace EA4S.Assessment
{
    public static class Localization
    {
        public static Db.LocalizationDataId Random( params Db.LocalizationDataId[] ids)
        {
            return ids[UnityEngine.Random.Range( 0, ids.Length)];
        }
    }
}
