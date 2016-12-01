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

        public static string GetWordCategoryTitle(Db.WordDataCategory cat)
        {
            Db.LocalizationDataId loc = Db.LocalizationDataId.UI_None;
            switch (cat) {
                case Db.WordDataCategory.People: loc = Db.LocalizationDataId.UI_People; break;
                case Db.WordDataCategory.Conjunctions: loc = Db.LocalizationDataId.UI_Conjunctions; break;
                case Db.WordDataCategory.Feeling: loc = Db.LocalizationDataId.UI_Feelings; break;
                case Db.WordDataCategory.Food: loc = Db.LocalizationDataId.UI_Food; break;
                case Db.WordDataCategory.FamilyMember: loc = Db.LocalizationDataId.UI_FamilyMembers; break;
                case Db.WordDataCategory.BodyPart: loc = Db.LocalizationDataId.UI_BodyParts; break;
                case Db.WordDataCategory.Thing: loc = Db.LocalizationDataId.UI_Things; break;
                case Db.WordDataCategory.Place: loc = Db.LocalizationDataId.UI_Places; break;
                case Db.WordDataCategory.Sport: loc = Db.LocalizationDataId.UI_Sports; break;
                case Db.WordDataCategory.Animal: loc = Db.LocalizationDataId.UI_Animals; break;
                case Db.WordDataCategory.General: loc = Db.LocalizationDataId.UI_General; break;
                case Db.WordDataCategory.Furniture: loc = Db.LocalizationDataId.UI_Furniture; break;
                case Db.WordDataCategory.Size: loc = Db.LocalizationDataId.UI_Size; break;
                case Db.WordDataCategory.Nature: loc = Db.LocalizationDataId.UI_Nature; break;
                case Db.WordDataCategory.Vehicle: loc = Db.LocalizationDataId.UI_Vehicles; break;
                case Db.WordDataCategory.Job: loc = Db.LocalizationDataId.UI_Jobs; break;
                case Db.WordDataCategory.Clothes: loc = Db.LocalizationDataId.UI_Clothes; break;
                case Db.WordDataCategory.Color: loc = Db.LocalizationDataId.UI_Colors; break;
                case Db.WordDataCategory.Time: loc = Db.LocalizationDataId.UI_Time; break;
                case Db.WordDataCategory.Direction: loc = Db.LocalizationDataId.UI_Directions; break;
                case Db.WordDataCategory.Position: loc = Db.LocalizationDataId.UI_Positions; break;
                case Db.WordDataCategory.Number: loc = Db.LocalizationDataId.UI_Numbers; break;
                case Db.WordDataCategory.NumberOrdinal: loc = Db.LocalizationDataId.UI_NumbersOrdinal; break;
                case Db.WordDataCategory.Shape: loc = Db.LocalizationDataId.UI_Shapes; break;
            }
            return GetTranslation(loc);
        }

    }
}
