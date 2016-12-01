namespace EA4S
{
    public class LocalizationManager
    {
        public static string GetTranslation(Db.LocalizationDataId id)
        {
            return GetLocalizationData(id).Arabic;
        }

        public static string GetTranslation(string id)
        {
            return GetLocalizationData(id).Arabic;
        }

        public static Db.LocalizationData GetLocalizationData(Db.LocalizationDataId id)
        {
            return AppManager.I.DB.GetLocalizationDataById(id.ToString());
        }

        public static Db.LocalizationData GetLocalizationData(string id)
        {
            return AppManager.I.DB.GetLocalizationDataById(id);
        }
    }
}
