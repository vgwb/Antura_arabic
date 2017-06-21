namespace EA4S.Core
{
    /// <summary>
    /// Static class that helps in localizing strings.
    /// </summary>
    public class LocalizationManager
    {
        public static string GetTranslation(Database.LocalizationDataId id)
        {
            return GetLocalizationData(id).GetLocalizedText(CurrentPlayerGender);
        }

        public static string GetTranslation(string id)
        {
            return GetLocalizationData(id).GetLocalizedText(CurrentPlayerGender);
        }

        public static string GetLocalizedAudioFileName(string id)
        {
            return GetLocalizationData(id).GetLocalizedAudioFileName(CurrentPlayerGender);
        }
        public static string GetLocalizedAudioFileName(string id, PlayerGender forcedGender)
        {
            return GetLocalizationData(id).GetLocalizedAudioFileName(forcedGender);
        }

        private static PlayerGender CurrentPlayerGender
        {
            get
            {
                if (AppManager.I.Player == null) return PlayerGender.M;
                return AppManager.I.Player.Gender;
            }
        }

        public static Database.LocalizationData GetLocalizationData(Database.LocalizationDataId id)
        {
            return AppManager.I.DB.GetLocalizationDataById(id.ToString());
        }

        public static Database.LocalizationData GetLocalizationData(string id)
        {
            return AppManager.I.DB.GetLocalizationDataById(id);
        }

        public static Database.LocalizationData GetWordCategoryData(Database.WordDataCategory cat)
        {
            Database.LocalizationDataId loc = Database.LocalizationDataId.UI_None;
            switch (cat) {
                case Database.WordDataCategory.Adjectives: loc = Database.LocalizationDataId.UI_WordCat_Adjectives; break;
                case Database.WordDataCategory.Animal: loc = Database.LocalizationDataId.UI_Animals; break;
                case Database.WordDataCategory.BodyPart: loc = Database.LocalizationDataId.UI_BodyParts; break;
                case Database.WordDataCategory.Clothes: loc = Database.LocalizationDataId.UI_Clothes; break;
                case Database.WordDataCategory.Color: loc = Database.LocalizationDataId.UI_Colors; break;
                case Database.WordDataCategory.Conjunctions: loc = Database.LocalizationDataId.UI_Conjunctions; break;
                case Database.WordDataCategory.Direction: loc = Database.LocalizationDataId.UI_Directions; break;
                case Database.WordDataCategory.Expressions: loc = Database.LocalizationDataId.UI_WordCat_Expressions; break;
                case Database.WordDataCategory.FamilyMember: loc = Database.LocalizationDataId.UI_FamilyMembers; break;
                case Database.WordDataCategory.Feeling: loc = Database.LocalizationDataId.UI_Feelings; break;
                case Database.WordDataCategory.Food: loc = Database.LocalizationDataId.UI_Food; break;
                case Database.WordDataCategory.Furniture: loc = Database.LocalizationDataId.UI_Furniture; break;
                case Database.WordDataCategory.General: loc = Database.LocalizationDataId.UI_General; break;
                case Database.WordDataCategory.Greetings: loc = Database.LocalizationDataId.UI_WordCat_Greetings; break;
                case Database.WordDataCategory.Verbs: loc = Database.LocalizationDataId.UI_WordCat_Verbs; break;
                case Database.WordDataCategory.Job: loc = Database.LocalizationDataId.UI_Jobs; break;
                case Database.WordDataCategory.Names: loc = Database.LocalizationDataId.UI_WordCat_Names; break;
                case Database.WordDataCategory.Nature: loc = Database.LocalizationDataId.UI_Nature; break;
                case Database.WordDataCategory.Number: loc = Database.LocalizationDataId.UI_Numbers; break;
                case Database.WordDataCategory.NumberOrdinal: loc = Database.LocalizationDataId.UI_NumbersOrdinal; break;
                case Database.WordDataCategory.People: loc = Database.LocalizationDataId.UI_People; break;
                case Database.WordDataCategory.Place: loc = Database.LocalizationDataId.UI_Places; break;
                case Database.WordDataCategory.Position: loc = Database.LocalizationDataId.UI_Positions; break;
                case Database.WordDataCategory.Question: loc = Database.LocalizationDataId.UI_Phrases_Questions; break;
                case Database.WordDataCategory.Shape: loc = Database.LocalizationDataId.UI_Shapes; break;
                case Database.WordDataCategory.Size: loc = Database.LocalizationDataId.UI_Size; break;
                case Database.WordDataCategory.Sport: loc = Database.LocalizationDataId.UI_Sports; break;
                case Database.WordDataCategory.Thing: loc = Database.LocalizationDataId.UI_Things; break;
                case Database.WordDataCategory.Time: loc = Database.LocalizationDataId.UI_Time; break;
                case Database.WordDataCategory.Vehicle: loc = Database.LocalizationDataId.UI_Vehicles; break;
                case Database.WordDataCategory.Weather: loc = Database.LocalizationDataId.UI_WordCat_Weather; break;
            }
            return GetLocalizationData(loc);
        }

        public static Database.LocalizationData GetPhraseCategoryData(Database.PhraseDataCategory cat)
        {
            Database.LocalizationDataId loc = Database.LocalizationDataId.UI_None;
            switch (cat) {
                case Database.PhraseDataCategory.Question: loc = Database.LocalizationDataId.UI_Phrases_Questions; break;
                case Database.PhraseDataCategory.Reply: loc = Database.LocalizationDataId.UI_Phrases_Replies; break;
                case Database.PhraseDataCategory.Greetings: loc = Database.LocalizationDataId.UI_Phrases_Greetings; break;
                case Database.PhraseDataCategory.Year: loc = Database.LocalizationDataId.UI_Phrases_Years; break;
                case Database.PhraseDataCategory.Sentence: loc = Database.LocalizationDataId.UI_Phrases_Sentences; break;
                case Database.PhraseDataCategory.Expression: loc = Database.LocalizationDataId.UI_Phrases_Expressions; break;
            }
            return GetLocalizationData(loc);
        }
    }
}
