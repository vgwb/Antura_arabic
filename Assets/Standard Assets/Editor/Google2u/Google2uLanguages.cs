// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    public static class Language
    {
        public enum Code
        {
            /* Commonly supported languages
			 * When translating from English, these languages
			 * flow left to right, and contain mostly the same
			 * character set, requiring the least amount of
			 * effort for the biggest gain
			 */
            EN, //	English
            FR, //	French
            IT, //	Italian
            DE, //	German
            ES, //	Spanish

            /* Uncommonly supported languages
			 * These languages are very popular, but require more
			 * effort to translate. They have unique character sets
			 * and flow, or the native speakers commonly also speak
			 * a more popular language.
			 */
            AR, //	Arabic
            ZHCN, //	Chinese (Simplified)
            JA, //	Japanese
            KO, //	Korean
            VI, //	Vietnamese
            RU, //	Russian
            NL, //	Dutch
            PT, //	Portuguese

            INVALID
        }

        // Selection Box String - Language Code - Language Name
        public static string[,] languageStrings =
        {
            {"(EN) 	English", "EN", "English"},
            {"(FR) 	French", "FR", "French"},
            {"(IT) 	Italian", "IT", "Italian"},
            {"(DE) 	German", "DE", "German"},
            {"(ES) 	Spanish", "ES", "Spanish"},
            {"(AR)	Arabic", "AR", "Arabic"},
            {"(ZHCN)	Chinese (Simplified)", "ZH-CN", "Chinese (Simplified)"},
            {"(JA) 	Japanese", "JA", "Japanese"},
            {"(KO) 	Korean", "KO", "Korean"},
            {"(VI) 	Vietnamese", "VI", "Vietnamese"},
            {"(RU) 	Russian", "RU", "Russian"},
            {"(NL) 	Dutch", "NL", "Dutch"},
            {"(PT) 	Portuguese", "PT", "Portuguese"}
        };

        public static string GetLanguageCodeAsString(Code languageCode)
        {
            switch (languageCode)
            {
                case Code.EN:
                    return "EN";
                case Code.FR:
                    return "FR";
                case Code.IT:
                    return "IT";
                case Code.DE:
                    return "DE";
                case Code.ES:
                    return "ES";
                case Code.AR:
                    return "AR";
                case Code.ZHCN:
                    return "ZH-CN";
                case Code.JA:
                    return "JA";
                case Code.KO:
                    return "KO";
                case Code.VI:
                    return "VI";
                case Code.RU:
                    return "RU";
                case Code.NL:
                    return "NL";
                case Code.PT:
                    return "PT";
            }

            return "Invalid Language";
        }

        public static string GetLanguageString(Code languageCode)
        {
            switch (languageCode)
            {
                case Code.EN:
                    return "ENGLISH";
                case Code.FR:
                    return "FRENCH";
                case Code.IT:
                    return "ITALIAN";
                case Code.DE:
                    return "GERMAN";
                case Code.ES:
                    return "SPANISH";
                case Code.AR:
                    return "ARABIC";
                case Code.ZHCN:
                    return "CHINESE (SIMPLIFIED)";
                case Code.JA:
                    return "JAPANESE";
                case Code.KO:
                    return "KOREAN";
                case Code.VI:
                    return "VIETNAMESE";
                case Code.RU:
                    return "RUSSIAN";
                case Code.NL:
                    return "DUTCH";
                case Code.PT:
                    return "PORTUGUESE";
            }

            return "Invalid Language";
        }

        public static Code GetLanguageCode(string languageString)
        {
            switch (languageString.ToUpper())
            {
                case "EN":
                case "ENGLISH":
                    return Code.EN;
                case "FR":
                case "FRENCH":
                    return Code.FR;
                case "IT":
                case "ITALIAN":
                    return Code.IT;
                case "DE":
                case "GERMAN":
                    return Code.DE;
                case "ES":
                case "SPANISH":
                    return Code.ES;
                case "AR":
                case "ARABIC":
                    return Code.AR;
                case "ZHCN":
                case "ZH-CN":
                case "CHINESE":
                case "CHINESE SIMPLIFIED":
                case "CHINESE (SIMPLIFIED)":
                    return Code.ZHCN;
                case "JA":
                case "JAPANESE":
                    return Code.JA;
                case "KO":
                case "KOREAN":
                    return Code.KO;
                case "VI":
                case "VIETNAMESE":
                    return Code.VI;
                case "RU":
                case "RUSSIAN":
                    return Code.DE;
                case "NL":
                case "DUTCH":
                    return Code.NL;
                case "PT":
                case "PORTUGUESE":
                    return Code.PT;
            }

            return Code.INVALID;
        }
    }
}