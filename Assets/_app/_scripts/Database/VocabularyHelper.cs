using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using EA4S.Helpers;
using EA4S.Teacher;

namespace EA4S.Database
{
    public struct DiacriticComboFix
    {
        public string Unicode1;
        public string Unicode2;
        public float DeltaX;
        public float DeltaY;

        public DiacriticComboFix(string _unicode1, string _unicode2, float _deltaX, float _deltaY)
        {
            Unicode1 = _unicode1;
            Unicode2 = _unicode2;
            DeltaX = _deltaX;
            DeltaY = _deltaY;
        }
    }

    /// <summary>
    /// Provides helpers to get correct letter/word/phrase data according to the teacher's logic and based on the player's progression
    /// </summary>
    public class VocabularyHelper
    {
        private DatabaseManager dbManager;

        // HACK: this is needed for some games where LamAlef behaves differently
        public bool ForceUnseparatedLetters { get; set; }
        public List<string> ProblematicWordIds = new List<string>() { "won", "went", "sat", "studied", "laughed", "played", "flapped", "caught", "released", "carried", "understood" };

        public List<DiacriticComboFix> DiacriticCombos2Fix = new List<DiacriticComboFix>();

        public VocabularyHelper(DatabaseManager _dbManager)
        {
            dbManager = _dbManager;
            buildDiacriticCombos2Fix();
        }

        void buildDiacriticCombos2Fix()
        {
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0627", "064B", 0, 70));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE8E", "064B", -20, 80));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0623", "064E", 0, 200));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE84", "064E", -20, 200));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0639", "0650", 20, -300));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FECA", "0650", 20, -300));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0628", "0650", 130, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE91", "0650", 0, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE92", "0650", 0, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE90", "0650", 130, -120));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0630", "064E", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEAC", "064E", 0, 80));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED3", "064E", 200, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED4", "064E", 200, 50));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED3", "0652", 200, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED4", "0652", 200, 60));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEEC", "0650", 0, -120));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("062D", "0650", 0, -350));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEA2", "0650", 0, -350));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEDB", "064F", 0, 70));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEDC", "064F", 0, 70));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEFB", "064B", 0, 70));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEFC", "064B", 0, 70));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEDF", "064C", 40, 120));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEE0", "064C", 40, 120));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEDF", "064E", 40, 120));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEE0", "064E", 40, 120));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0644", "0650", 0, -100));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEDE", "0650", 0, -100));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0644", "064D", 30, -90));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEDE", "064D", 30, -90));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEDF", "0652", 60, 140));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEE0", "0652", 60, 140));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0645", "0650", 60, -100));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEE2", "0650", 60, -100));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0646", "064D", 40, -130));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEE6", "064D", 40, -130));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEE7", "0652", 30, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEE8", "0652", 30, 50));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0642", "064E", 50, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED7", "064E", 0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED8", "064E", 0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED6", "064E", 50, 0));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0631", "0650", 50, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEAE", "0650", 50, -200));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0637", "064C", 0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC3", "064C", 0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC4", "064C", 0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC2", "064C", 0, 100));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("0637", "064E", 0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC3", "064E", 0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC4", "064E", 0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC2", "064E", 0, 100));

            DiacriticCombos2Fix.Add(new DiacriticComboFix("062A", "064F", 60, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE97", "064F", 0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE98", "064F", 0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE96", "064F", 60, 0));
            // teh_dammah_tanwin
            DiacriticCombos2Fix.Add(new DiacriticComboFix("062A", "064C", 60, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE97", "064C", 0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE98", "064C", 0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE96", "064C", 60, 0));
            // teh_fathah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("062A", "064E", 70, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE96", "064E", 70, 0));
            // teh_marbuta_dammah_tanwin
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0629", "064C", -10, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE97", "064C", 0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE98", "064C", 0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE96", "064C", 60, 0));
            // teh_marbuta_fathah_tanwin
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0629", "064B", -10, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE97", "064B", 0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE98", "064B", 0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE96", "064B", 60, 0));
            // Diacritic Song
            // alef_hamza_hi_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0623", "064F", -20, 130));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE84", "064F", 0, 130));
            // alef_hamza_low_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0625", "0650", 0, -80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE88", "0650", 0, -80));
            // theh_fathah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("062B", "064E", 60, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE9B", "064E", 0, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE9C", "064E", 0, 60));
            // theh_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("062B", "064F", 60, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE9B", "064F", 0, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE9C", "064F", 0, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE9A", "064F", 60, 40));
            // jeem_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("062C", "0650", 50, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE9F", "0650", 20, -90));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEA0", "0650", 20, -90));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FE9E", "0650", 50, -200));
            // khah_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("062E", "0650", 50, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEA6", "0650", 50, -200));
            // thal_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0630", "064F", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEAC", "064F", 0, 80));
            // zain_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0632", "0650", 70, -180));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEB0", "0650", 70, -180));
            // seen_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0633", "0650", 50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEB2", "0650", 50, -120));
            // sheen_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0634", "0650", 50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEB6", "0650", 50, -120));
            // sad_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0635", "0650", 50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEBA", "0650", 50, -120));
            // dad_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0636", "0650", 50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEBE", "0650", 50, -120));
            // tah_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0637", "064F", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC3", "064F", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC4", "064F", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC2", "064F", 0, 80));
            // zah_fathah 
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0638", "064E", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC7", "064E", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC8", "064E", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC6", "064E", 0, 80));
            // zah_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0638", "064F", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC7", "064F", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC8", "064F", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEC6", "064F", 0, 80));
            // ghain_fathah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("063A", "064E", 0, 50));
            // ghain_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("063A", "064F", 0, 50));
            // ghain_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("063A", "0650", 0, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FECE", "0650", 0, -200));
            // feh_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0641", "064F", 100, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED3", "064F", 0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED4", "064F", 0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED2", "064F", 100, 0));
            // qaf_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0642", "064F", 80, 20));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED7", "064F", 0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED8", "064F", 0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED6", "064F", 80, 20));
            // qaf_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0642", "0650", 50, -140));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FED6", "0650", 50, -140));
            // lam_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0644", "064F", 80, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEDF", "064F", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEE0", "064F", 0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEDE", "064F", 80, 40));
            // noon_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0646", "0650", 50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEE6", "0650", 50, -120));
            // waw_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboFix("0648", "0650", 50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEEE", "0650", 50, -120));
            // yeh_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboFix("064A", "0650", 120, -230));
            DiacriticCombos2Fix.Add(new DiacriticComboFix("FEF2", "0650", 100, -230));

            //List<LetterData> list = AppManager.I.DB.FindLetterData((x) => (x.Symbol_DeltaY != 0));
            //foreach (var letter in list) {
            //    DiacriticCombos2Fix.Add(new DiacriticComboFix(letter.Isolated_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    if (letter.Initial_Unicode != "") {
            //        DiacriticCombos2Fix.Add(new DiacriticComboFix(letter.Initial_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    }
            //    if (letter.Medial_Unicode != "") {
            //        DiacriticCombos2Fix.Add(new DiacriticComboFix(letter.Medial_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    }
            //    if (letter.Final_Unicode != "") {
            //        DiacriticCombos2Fix.Add(new DiacriticComboFix(letter.Final_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    }
            //}
        }

        public Vector2 FindDiacriticCombo2Fix(string Unicode1, string Unicode2)
        {
            Vector2 newDelta = new Vector2(0, 0);
            foreach (var combo in DiacriticCombos2Fix) {
                if (combo.Unicode1 == Unicode1 && combo.Unicode2 == Unicode2) {
                    newDelta.x = combo.DeltaX;
                    newDelta.y = combo.DeltaY;
                    break;
                }
            }
            return newDelta;
        }

        /// <summary>
        /// Adjusts the diacritic positions.
        /// </summary>
        /// <returns><c>true</c>, if diacritic positions was adjusted, <c>false</c> otherwise.</returns>
        /// <param name="textInfo">Text info.</param>
        public bool FixDiacriticPositions(TMPro.TMP_TextInfo textInfo)
        {
            //Debug.Log("FixDiacriticPositions " + textInfo.characterCount);
            int characterCount = textInfo.characterCount;
            bool changed = false;

            if (characterCount > 1) {

                // output unicodes for DiacriticCombos2Fix
                string combo = "";
                for (int i = 0; i < characterCount; i++) {
                    combo += '"' + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[i].character) + '"' + ',';
                }
                Debug.Log("DiacriticCombos2Fix.Add(new DiacriticComboFix(" + combo + " 0, 0));");

                Vector2 modificationDelta = new Vector2(0, 0);
                for (int charPosition = 0; charPosition < characterCount - 1; charPosition++) {
                    modificationDelta = FindDiacriticCombo2Fix(
                        ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[charPosition].character),
                        ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[charPosition + 1].character)
                    );

                    if (modificationDelta.sqrMagnitude > 0f) {
                        changed = true;
                        int materialIndex = textInfo.characterInfo[charPosition + 1].materialReferenceIndex;
                        int vertexIndex = textInfo.characterInfo[charPosition + 1].vertexIndex;
                        Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;

                        float charsize = (sourceVertices[vertexIndex + 2].y - sourceVertices[vertexIndex + 0].y);
                        float dx = charsize * modificationDelta.x / 100f;
                        float dy = charsize * modificationDelta.y / 100f;
                        Vector3 offset = new Vector3(dx, dy, 0f);

                        Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
                        destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + offset;
                        destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + offset;
                        destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + offset;
                        destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + offset;

#if UNITY_EDITOR
                        Debug.Log("DIACRITIC FIX: "
                                  + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[charPosition].character)
                                  + " + "
                                  + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[charPosition + 1].character)
                                  + " by " + modificationDelta);
#endif
                    }
                }

            }
            return changed;
        }
        #region Letter Utilities

        private bool CheckFilters(LetterFilters filters, LetterData data)
        {
            if (filters.requireDiacritics && !data.IsOfKindCategory(LetterKindCategory.DiacriticCombo)) return false;

            switch (filters.excludeDiacritics) {
                case LetterFilters.ExcludeDiacritics.All:
                    if (data.IsOfKindCategory(LetterKindCategory.DiacriticCombo)) return false;
                    break;
                case LetterFilters.ExcludeDiacritics.AllButMain:
                    var symbol = GetSymbolOf(data.Id);
                    if (symbol != null && data.IsOfKindCategory(LetterKindCategory.DiacriticCombo) && symbol.Tag != "MainDiacritic") return false;
                    break;
                default:
                    break;
            }

            switch (filters.excludeLetterVariations) {
                case LetterFilters.ExcludeLetterVariations.All:
                    if (data.IsOfKindCategory(LetterKindCategory.LetterVariation)) return false;
                    break;
                case LetterFilters.ExcludeLetterVariations.AllButAlefHamza:
                    if (data.IsOfKindCategory(LetterKindCategory.LetterVariation) && data.Tag != "AlefHamzaVariation") return false;
                    break;
                default:
                    break;
            }

            if (filters.excludeDiphthongs && data.Kind == LetterDataKind.Diphthong) return false;
            if (data.IsOfKindCategory(LetterKindCategory.Symbol)) return false; // always skip symbols
            return true;
        }

        #endregion

        #region Letter -> Letter

        public List<LetterData> GetAllBaseLetters()
        {
            var p = new LetterFilters(excludeDiacritics: LetterFilters.ExcludeDiacritics.All, excludeLetterVariations: LetterFilters.ExcludeLetterVariations.All, excludeDiphthongs: true);
            return GetAllLetters(p);
        }

        public List<LetterData> GetAllLetters(LetterFilters filters)
        {
            return dbManager.FindLetterData(x => CheckFilters(filters, x));
        }

        private List<LetterData> GetLettersNotIn(List<string> tabooList, LetterFilters filters)
        {
            return dbManager.FindLetterData(x => !tabooList.Contains(x.Id) && CheckFilters(filters, x));
        }

        public List<LetterData> GetLettersNotIn(LetterFilters filters, params LetterData[] tabooArray)
        {
            var tabooList = new List<LetterData>(tabooArray);
            return GetLettersNotIn(tabooList.ConvertAll(x => x.Id), filters);
        }

        public List<LetterData> GetLettersByKind(LetterDataKind choice)
        {
            return dbManager.FindLetterData(x => x.Kind == choice); // @note: this does not use filters, special case
        }

        public List<LetterData> GetLettersBySunMoon(LetterDataSunMoon choice, LetterFilters filters)
        {
            return dbManager.FindLetterData(x => x.SunMoon == choice && CheckFilters(filters, x));
        }

        public List<LetterData> GetConsonantLetter(LetterFilters filters)
        {
            return dbManager.FindLetterData(x => x.Type == LetterDataType.Consonant || x.Type == LetterDataType.Powerful && CheckFilters(filters, x));
        }

        public List<LetterData> GetVowelLetter(LetterFilters filters)
        {
            return dbManager.FindLetterData(x => x.Type == LetterDataType.LongVowel && CheckFilters(filters, x));
        }

        public List<LetterData> GetLettersByType(LetterDataType choice, LetterFilters filters)
        {
            return dbManager.FindLetterData(x => x.Type == choice && CheckFilters(filters, x));
        }

        public LetterData GetBaseOf(string letterId)
        {
            var data = dbManager.GetLetterDataById(letterId);
            if (data.BaseLetter == "") return null;
            return dbManager.FindLetterData(x => x.Id == data.BaseLetter)[0];
        }

        public LetterData GetSymbolOf(string letterId)
        {
            var data = dbManager.GetLetterDataById(letterId);
            if (data.Symbol == "") return null;
            return dbManager.FindLetterData(x => x.Id == data.Symbol)[0];
        }

        public List<LetterData> GetLettersWithBase(string letterId)
        {
            var baseData = dbManager.GetLetterDataById(letterId);
            return dbManager.FindLetterData(x => x.BaseLetter == baseData.Id);
        }

        #endregion

        #region Word -> Letter

        private List<string> GetLetterIdsInWordData(WordData wordData)
        {
            List<string> letter_ids_list = null;
            if (ForceUnseparatedLetters) {
                var parts = ArabicAlphabetHelper.AnalyzeData(AppManager.I.DB.StaticDatabase, wordData, separateVariations: false);
                letter_ids_list = parts.ConvertAll(p => p.letter.Id);
            } else {
                letter_ids_list = new List<string>(wordData.Letters);
            }
            return letter_ids_list;
        }

        public List<LetterData> GetLettersInWord(WordData wordData)
        {
            List<string> letter_ids_list = GetLetterIdsInWordData(wordData);
            List<LetterData> list = new List<LetterData>();
            foreach (var letter_id in letter_ids_list) list.Add(dbManager.GetLetterDataById(letter_id));
            return list;
        }
        public List<LetterData> GetLettersInWord(string wordId)
        {
            WordData wordData = dbManager.GetWordDataById(wordId);
            return GetLettersInWord(wordData);
        }

        public List<LetterData> GetLettersNotInWords(params WordData[] tabooArray)
        {
            return GetLettersNotInWords(LetterKindCategory.Real, tabooArray);
        }
        public List<LetterData> GetLettersNotInWords(LetterKindCategory category = LetterKindCategory.Real, params WordData[] tabooArray)
        {
            var letter_ids_list = new HashSet<string>();
            foreach (var tabooWordData in tabooArray) {
                var tabooWordDataLetterIds = GetLetterIdsInWordData(tabooWordData);
                letter_ids_list.UnionWith(tabooWordDataLetterIds);
            }
            List<LetterData> list = dbManager.FindLetterData(x => !letter_ids_list.Contains(x.Id) && x.IsOfKindCategory(category));
            return list;
        }

        public List<LetterData> GetLettersNotInWord(string wordId, LetterKindCategory category = LetterKindCategory.Real)
        {
            WordData wordData = dbManager.GetWordDataById(wordId);
            var letter_ids_list = GetLetterIdsInWordData(wordData);
            List<LetterData> list = dbManager.FindLetterData(x => !letter_ids_list.Contains(x.Id) && x.IsOfKindCategory(category));
            return list;
        }

        public List<LetterData> GetCommonLettersInWords(params WordData[] words)
        {
            Dictionary<LetterData, int> countDict = new Dictionary<LetterData, int>();
            foreach (var word in words) {
                HashSet<LetterData> nonRepeatingLettersOfWord = new HashSet<LetterData>();

                var letters = GetLettersInWord(word);
                foreach (var letter in letters) nonRepeatingLettersOfWord.Add(letter);

                foreach (var letter in nonRepeatingLettersOfWord) {
                    if (!countDict.ContainsKey(letter)) countDict[letter] = 0;
                    countDict[letter] += 1;
                }
            }

            // Get only these letters that are in all words
            var commonLettersList = new List<LetterData>();
            foreach (var letter in countDict.Keys) {
                if (countDict[letter] == words.Length) {
                    commonLettersList.Add(letter);
                }
            }

            return commonLettersList;
        }


        #endregion

        #region Word Utilities

        private bool CheckFilters(WordFilters filters, WordData data)
        {
            if (filters.excludeArticles && data.Article != WordDataArticle.None) return false;
            if (filters.requireDrawings && !data.HasDrawing()) return false;
            if (filters.excludeColorWords && data.Category == WordDataCategory.Color) return false;
            if (filters.excludePluralDual && data.Form != WordDataForm.Singular) return false;
            if (filters.excludeDiacritics && this.WordHasDiacriticCombo(data)) return false;
            if (filters.excludeLetterVariations && this.WordHasLetterVariations(data)) return false;
            if (filters.requireDiacritics && !this.WordHasDiacriticCombo(data)) return false;
            return true;
        }

        private bool WordHasDiacriticCombo(WordData data)
        {
            foreach (var letter in GetLettersInWord(data))
                if (letter.IsOfKindCategory(LetterKindCategory.DiacriticCombo))
                    return true;
            return false;
        }

        private bool WordHasLetterVariations(WordData data)
        {
            foreach (var letter in GetLettersInWord(data))
                if (letter.IsOfKindCategory(LetterKindCategory.LetterVariation))
                    return true;
            return false;
        }


        /// <summary>
        /// tranformsf the hex string of the glyph into the corresponding char
        /// </summary>
        /// <returns>The drawing string</returns>
        /// <param name="word">WordData.</param>
        public string GetWordDrawing(WordData word)
        {
            //Debug.Log("the int of hex:" + word.Drawing + " is " + int.Parse(word.Drawing, NumberStyles.HexNumber));
            if (word.Drawing != "") {
                int result = 0;
                if (int.TryParse(word.Drawing, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out result)) {
                    return ((char)result).ToString();
                }
                return "";
            }
            return "";
        }

        #endregion

        #region Word -> Word

        public List<WordData> GetAllWords(WordFilters filters)
        {
            return dbManager.FindWordData(x => CheckFilters(filters, x));
        }

        private List<WordData> GetWordsNotIn(WordFilters filters, List<string> tabooList)
        {
            return dbManager.FindWordData(x => !tabooList.Contains(x.Id) && CheckFilters(filters, x));
        }

        public List<WordData> GetWordsNotIn(WordFilters filters, params WordData[] tabooArray)
        {
            var tabooList = new List<WordData>(tabooArray);
            return GetWordsNotIn(filters, tabooList.ConvertAll(x => x.Id));
        }

        public List<WordData> GetWordsByCategory(WordDataCategory choice, WordFilters filters)
        {
            if (choice == WordDataCategory.None) return this.GetAllWords(filters);
            return dbManager.FindWordData(x => x.Category == choice && CheckFilters(filters, x));
        }

        public List<WordData> GetWordsByArticle(WordDataArticle choice, WordFilters filters)
        {
            return dbManager.FindWordData(x => x.Article == choice && CheckFilters(filters, x));
        }

        public List<WordData> GetWordsByForm(WordDataForm choice, WordFilters filters)
        {
            return dbManager.FindWordData(x => x.Form == choice && CheckFilters(filters, x));
        }

        public List<WordData> GetWordsByKind(WordDataKind choice, WordFilters filters)
        {
            return dbManager.FindWordData(x => x.Kind == choice && CheckFilters(filters, x));
        }

        #endregion

        #region Letter -> Word

        public List<WordData> GetWordsWithLetter(WordFilters filters, string okLetter)
        {
            return GetWordsByLetters(filters, new string[] { okLetter }, null);
        }

        public List<WordData> GetWordsWithLetters(WordFilters filters, params string[] okLetters)
        {
            return GetWordsByLetters(filters, okLetters, null);
        }

        public List<WordData> GetWordsWithoutLetter(WordFilters filters, string tabooLetter)
        {
            return GetWordsByLetters(filters, null, new string[] { tabooLetter });
        }

        public List<WordData> GetWordsWithoutLetters(WordFilters filters, params string[] tabooLetters)
        {
            return GetWordsByLetters(filters, null, tabooLetters);
        }

        public List<WordData> GetWordsByLetters(WordFilters filters, string[] okLettersArray, string[] tabooLettersArray)
        {
            if (okLettersArray == null) okLettersArray = new string[] { };
            if (tabooLettersArray == null) tabooLettersArray = new string[] { };

            var okLetters = new HashSet<string>(okLettersArray);
            var tabooLetters = new HashSet<string>(tabooLettersArray);

            List<WordData> list = dbManager.FindWordData(x => {

                if (!CheckFilters(filters, x)) return false;

                var letter_ids = GetLetterIdsInWordData(x);

                if (tabooLetters.Count > 0) {
                    foreach (var letter_id in letter_ids) {
                        if (tabooLetters.Contains(letter_id)) {
                            return false;
                        }
                    }
                }

                if (okLetters.Count > 0) {
                    bool hasAllOkLetters = true;
                    foreach (var okLetter in okLetters) {
                        bool hasThisLetter = false;
                        foreach (var letter_id in letter_ids) {
                            if (letter_id == okLetter) {
                                hasThisLetter = true;
                                break;
                            }
                        }
                        if (!hasThisLetter) {
                            hasAllOkLetters = false;
                            break;
                        }
                    }
                    if (!hasAllOkLetters) return false;
                }
                return true;
            }
            );

            return list;
        }

        public bool WordContainsAnyLetter(WordData word, List<string> letter_ids)
        {
            var containedLetters = GetLettersInWord(word).ConvertAll(x => x.Id);
            foreach (var letter_id in letter_ids)
                if (containedLetters.Contains(letter_id))
                    return true;
            return false;
        }

        public bool WordHasAllLettersInCommonWith(WordData word, List<string> word_ids)
        {
            var containedLetters = GetLettersInWord(word);
            foreach (var letter in containedLetters)
                if (!LetterContainedInAnyWord(letter, word_ids))
                    return false;
            return true;
        }

        public bool LetterContainedInAnyWord(LetterData letter, List<string> word_ids)
        {
            foreach (var word_id in word_ids) {
                var containedLetters = GetLettersInWord(word_id);
                if (containedLetters.Contains(letter))
                    return true;
            }
            return false;
        }

        public bool AnyWordContainsLetter(LetterData letter, List<string> word_ids)
        {
            foreach (var word_id in word_ids)
                if (GetLettersInWord(word_id).Contains(letter))
                    return true;
            return false;
        }

        #endregion

        #region Phrase -> Word

        /// <summary>
        /// Gets the words in phrase, taken from field Words of data Pharse. these words are set manually in the db
        /// </summary>
        /// <returns>The words in phrase.</returns>
        /// <param name="phraseId">Phrase identifier.</param>
        /// <param name="wordFilters">Word filters.</param>
        public List<WordData> GetWordsInPhrase(string phraseId, WordFilters wordFilters = null)
        {
            if (wordFilters == null) wordFilters = new WordFilters();
            PhraseData data = dbManager.GetPhraseDataById(phraseId);
            return GetWordsInPhrase(data, wordFilters);
        }

        public List<WordData> GetWordsInPhrase(PhraseData phraseData, WordFilters wordFilters)
        {
            var words_ids_list = new List<string>(phraseData.Words);
            List<WordData> inputList = dbManager.FindWordData(x => words_ids_list.Contains(x.Id) && CheckFilters(wordFilters, x));
            List<WordData> orderedOutputList = new List<WordData>();
            words_ids_list.ForEach(id => {
                var word = inputList.Find(x => x.Id.Equals(id));
                if (word != null) orderedOutputList.Add(word);
            });
            return orderedOutputList;
        }

        public List<WordData> GetAnswersToPhrase(PhraseData phraseData, WordFilters wordFilters)
        {
            var words_ids_list = new List<string>(phraseData.Answers);
            List<WordData> list = dbManager.FindWordData(x => words_ids_list.Contains(x.Id) && CheckFilters(wordFilters, x));
            return list;
        }


        #endregion

        #region Phrase filters

        private bool CheckFilters(WordFilters wordFilters, PhraseFilters phraseFilters, PhraseData data)
        {
            // Words are checked with filters. At least 1 must fulfill the requirement.
            var words = GetWordsInPhrase(data, wordFilters);
            int nOkWords = words.Count;

            var answers = GetAnswersToPhrase(data, wordFilters);
            int nOkAnswers = answers.Count;

            if (phraseFilters.requireWords && (nOkWords == 0)) return false;
            if (phraseFilters.requireAtLeastTwoWords && (nOkWords <= 1)) return false;
            if (phraseFilters.requireAnswersOrWords && (nOkAnswers == 0 && nOkWords == 0)) return false;

            return true;
        }

        #endregion

        #region Phrase -> Phrase

        public List<PhraseData> GetAllPhrases(WordFilters wordFilters, PhraseFilters phraseFilters)
        {
            return dbManager.FindPhraseData(x => CheckFilters(wordFilters, phraseFilters, x));
        }

        public List<PhraseData> GetPhrasesByCategory(PhraseDataCategory choice, WordFilters wordFilters, PhraseFilters phraseFilters)
        {
            return dbManager.FindPhraseData(x => x.Category == choice && CheckFilters(wordFilters, phraseFilters, x));
        }

        public List<PhraseData> GetPhrasesNotIn(WordFilters wordFilters, PhraseFilters phraseFilters, params PhraseData[] tabooArray)
        {
            var tabooList = new List<PhraseData>(tabooArray);
            return dbManager.FindPhraseData(x => !tabooList.Contains(x) && CheckFilters(wordFilters, phraseFilters, x));
        }

        public PhraseData GetLinkedPhraseOf(string startPhraseId)
        {
            var data = dbManager.GetPhraseDataById(startPhraseId);
            return GetLinkedPhraseOf(data);
        }

        public PhraseData GetLinkedPhraseOf(PhraseData data)
        {
            if (data.Linked == "") return null;
            return dbManager.FindPhraseData(x => x.Id == data.Linked)[0];
        }

        #endregion

        #region Word -> Phrase

        public List<PhraseData> GetPhrasesWithWords(params string[] okWordsArray)
        {
            if (okWordsArray == null) okWordsArray = new string[] { };

            var okWords = new HashSet<string>(okWordsArray);

            List<PhraseData> list = dbManager.FindPhraseData(x => {
                if (okWords.Count > 0) {
                    bool hasAllOkWords = true;
                    foreach (var okWord in okWords) {
                        bool hasThisWord = false;
                        foreach (var word_id in x.Words) {
                            if (word_id == okWord) {
                                hasThisWord = true;
                                break;
                            }
                        }
                        if (!hasThisWord) {
                            hasAllOkWords = false;
                            break;
                        }
                    }
                    if (!hasAllOkWords) return false;
                }
                return true;
            }
            );
            return list;
        }

        #endregion

    }
}
