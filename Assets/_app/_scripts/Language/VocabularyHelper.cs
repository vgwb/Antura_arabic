using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Antura.Helpers;
using Antura.Teacher;
using Antura.Core;

namespace Antura.Database
{
    public struct DiacriticComboEntry
    {
        public string Unicode1;
        public string Unicode2;

        public DiacriticComboEntry(string _unicode1, string _unicode2)
        {
            Unicode1 = _unicode1;
            Unicode2 = _unicode2;
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

        // @note: these words are problematic as they contain letters that are not available in the journey position they are played at
        // TODO: remove this! Should be handled by the curriculum
        public List<string> ProblematicWordIds = new List<string>();
        /*
        {
            "won",
            "went",
            "sat",
            "studied",
            "laughed",
            "played",
            "flapped",
            "caught",
            "released",
            "carried",
            "understood"
        };*/

        public Dictionary<DiacriticComboEntry, Vector2> DiacriticCombos2Fix = new Dictionary<DiacriticComboEntry, Vector2>();

        public VocabularyHelper(DatabaseManager _dbManager)
        {
            dbManager = _dbManager;
            buildDiacriticCombos2Fix();
        }

        /// <summary>
        /// these are manually configured positions of diacritic symbols relative to the main letter
        /// since TextMesh Pro can't manage these automatically and some letters are too tall, with the symbol overlapping 
        /// </summary>
        void buildDiacriticCombos2Fix()
        {
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0627", "064B"), new Vector2(0, 70));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE8E", "064B"), new Vector2(-20, 80));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0623", "064E"), new Vector2(0, 200));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE84", "064E"), new Vector2(-20, 200));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0639", "0650"), new Vector2(20, -300));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FECA", "0650"), new Vector2(20, -300));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0628", "0650"), new Vector2(130, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE91", "0650"), new Vector2(0, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE92", "0650"), new Vector2(0, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE90", "0650"), new Vector2(130, -120));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0630", "064E"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEAC", "064E"), new Vector2(0, 80));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED3", "064E"), new Vector2(200, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED4", "064E"), new Vector2(200, 50));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED3", "0652"), new Vector2(200, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED4", "0652"), new Vector2(200, 60));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEEC", "0650"), new Vector2(0, -120));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062D", "0650"), new Vector2(0, -350));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEA2", "0650"), new Vector2(0, -350));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDB", "064F"), new Vector2(0, 70));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDC", "064F"), new Vector2(0, 70));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEFB", "064B"), new Vector2(0, 70));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEFC", "064B"), new Vector2(0, 70));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDF", "064C"), new Vector2(40, 120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE0", "064C"), new Vector2(40, 120));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDF", "064E"), new Vector2(40, 120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE0", "064E"), new Vector2(40, 120));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0644", "0650"), new Vector2(0, -100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDE", "0650"), new Vector2(0, -100));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0644", "064D"), new Vector2(30, -90));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDE", "064D"), new Vector2(30, -90));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDF", "0652"), new Vector2(60, 140));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE0", "0652"), new Vector2(60, 140));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0645", "0650"), new Vector2(60, -100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE2", "0650"), new Vector2(60, -100));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0646", "064D"), new Vector2(40, -130));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE6", "064D"), new Vector2(40, -130));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE7", "0652"), new Vector2(30, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE8", "0652"), new Vector2(30, 50));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0642", "064E"), new Vector2(50, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED7", "064E"), new Vector2(0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED8", "064E"), new Vector2(0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED6", "064E"), new Vector2(50, 0));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0631", "0650"), new Vector2(50, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEAE", "0650"), new Vector2(50, -200));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0637", "064C"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC3", "064C"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC4", "064C"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC2", "064C"), new Vector2(0, 100));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0637", "064E"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC3", "064E"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC4", "064E"), new Vector2(0, 100));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC2", "064E"), new Vector2(0, 100));

            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062A", "064F"), new Vector2(60, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE97", "064F"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE98", "064F"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE96", "064F"), new Vector2(60, 0));
            // teh_dammah_tanwin
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062A", "064C"), new Vector2(60, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE97", "064C"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE98", "064C"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE96", "064C"), new Vector2(60, 0));
            // teh_fathah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062A", "064E"), new Vector2(70, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE96", "064E"), new Vector2(70, 0));
            // teh_marbuta_dammah_tanwin
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0629", "064C"), new Vector2(-10, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE94", "064C"), new Vector2(0, 80));
            // teh_marbuta_fathah_tanwin
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0629", "064B"), new Vector2(-10, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE94", "064B"), new Vector2(0, 80));
            // Diacritic Song
            // alef_hamza_hi_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0623", "064F"), new Vector2(-20, 130));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE84", "064F"), new Vector2(-20, 130));
            // alef_hamza_low_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0625", "0650"), new Vector2(0, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE88", "0650"), new Vector2(0, -120));
            // theh_fathah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062B", "064E"), new Vector2(60, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9B", "064E"), new Vector2(0, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9C", "064E"), new Vector2(0, 60));
            // theh_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062B", "064F"), new Vector2(60, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9B", "064F"), new Vector2(0, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9C", "064F"), new Vector2(0, 60));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9A", "064F"), new Vector2(60, 40));
            // jeem_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062C", "0650"), new Vector2(50, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9F", "0650"), new Vector2(20, -90));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEA0", "0650"), new Vector2(20, -90));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FE9E", "0650"), new Vector2(50, -200));
            // khah_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("062E", "0650"), new Vector2(50, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEA6", "0650"), new Vector2(50, -200));
            // thal_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0630", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEAC", "064F"), new Vector2(0, 80));
            // zain_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0632", "0650"), new Vector2(70, -180));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEB0", "0650"), new Vector2(70, -180));
            // seen_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0633", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEB2", "0650"), new Vector2(50, -120));
            // sheen_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0634", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEB6", "0650"), new Vector2(50, -120));
            // sad_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0635", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEBA", "0650"), new Vector2(50, -120));
            // dad_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0636", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEBE", "0650"), new Vector2(50, -120));
            // tah_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0637", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC3", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC4", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC2", "064F"), new Vector2(0, 80));
            // zah_fathah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0638", "064E"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC7", "064E"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC8", "064E"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC6", "064E"), new Vector2(0, 80));
            // zah_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0638", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC7", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC8", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEC6", "064F"), new Vector2(0, 80));
            // ghain_fathah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("063A", "064E"), new Vector2(0, 50));
            // ghain_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("063A", "064F"), new Vector2(0, 50));
            // ghain_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("063A", "0650"), new Vector2(0, -200));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FECE", "0650"), new Vector2(0, -200));
            // feh_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0641", "064F"), new Vector2(100, 0));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED3", "064F"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED4", "064F"), new Vector2(0, 50));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED2", "064F"), new Vector2(100, 0));
            // qaf_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0642", "064F"), new Vector2(80, 20));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED7", "064F"), new Vector2(0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED8", "064F"), new Vector2(0, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED6", "064F"), new Vector2(80, 20));
            // qaf_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0642", "0650"), new Vector2(50, -140));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FED6", "0650"), new Vector2(50, -140));
            // lam_dammah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0644", "064F"), new Vector2(80, 40));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDF", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE0", "064F"), new Vector2(0, 80));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEDE", "064F"), new Vector2(80, 40));
            // noon_kasrah
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0646", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEE6", "0650"), new Vector2(50, -120));
            // waw_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("0648", "0650"), new Vector2(50, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEEE", "0650"), new Vector2(50, -120));
            // yeh_kasrah 
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("064A", "0650"), new Vector2(120, -230));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEF2", "0650"), new Vector2(100, -260));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEF3", "0650"), new Vector2(0, -120));
            DiacriticCombos2Fix.Add(new DiacriticComboEntry("FEF4", "0650"), new Vector2(0, -120));

            //List<LetterData> list = AppManager.I.DB.FindLetterData((x) => (x.Symbol_DeltaY != 0));
            //foreach (var letter in list) {
            //    DiacriticCombos2Fix.Add(new DiacriticComboEntry(letter.Isolated_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    if (letter.Initial_Unicode != "") {
            //        DiacriticCombos2Fix.Add(new DiacriticComboEntry(letter.Initial_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    }
            //    if (letter.Medial_Unicode != "") {
            //        DiacriticCombos2Fix.Add(new DiacriticComboEntry(letter.Medial_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    }
            //    if (letter.Final_Unicode != "") {
            //        DiacriticCombos2Fix.Add(new DiacriticComboEntry(letter.Final_Unicode + letter.Symbol_Unicode, letter.Symbol_DeltaY));
            //    }
            //}
        }


        public Vector2 FindDiacriticCombo2Fix(string Unicode1, string Unicode2)
        {
            Vector2 newDelta = new Vector2(0, 0);
            DiacriticCombos2Fix.TryGetValue(new DiacriticComboEntry(Unicode1, Unicode2), out newDelta);
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
                //string combo = "";
                //for (int i = 0; i < characterCount; i++) {
                //    combo += '"' + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[i].character) + '"' + ',';
                //}
                //Debug.Log("DiacriticCombos2Fix.Add(new DiacriticComboEntry(" + combo + " 0, 0));");

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

                        //Debug.Log("DIACRITIC FIX: "
                        //          + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[charPosition].character)
                        //          + " + "
                        //          + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[charPosition + 1].character)
                        //          + " by " + modificationDelta);
                    }
                }
            }
            return changed;
        }

        #region Letter Utilities

        private bool CheckFilters(LetterFilters filters, LetterData data)
        {
            if (filters.requireDiacritics && !data.IsOfKindCategory(LetterKindCategory.DiacriticCombo)) { return false; }
            if (!FilterByDiacritics(filters.excludeDiacritics, data)) { return false; }
            if (!FilterByLetterVariations(filters.excludeLetterVariations, data)) { return false; }
            if (!FilterByDipthongs(filters.excludeDiphthongs, data)) { return false; }

            // always skip symbols
            if (data.IsOfKindCategory(LetterKindCategory.Symbol)) {
                return false;
            }
            return true;
        }


        public bool FilterByDiacritics(LetterFilters.ExcludeDiacritics excludeDiacritics, LetterData data)
        {
            switch (excludeDiacritics) {
                case LetterFilters.ExcludeDiacritics.All:
                    if (data.IsOfKindCategory(LetterKindCategory.DiacriticCombo)) {
                        return false;
                    }
                    break;
                case LetterFilters.ExcludeDiacritics.AllButMain:
                    var symbol = GetSymbolOf(data.Id);
                    if (symbol != null && data.IsOfKindCategory(LetterKindCategory.DiacriticCombo) &&
                        symbol.Tag != "MainDiacritic") {
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        public bool FilterByLetterVariations(LetterFilters.ExcludeLetterVariations excludeLetterVariations, LetterData data)
        {
            switch (excludeLetterVariations) {
                case LetterFilters.ExcludeLetterVariations.All:
                    if (data.IsOfKindCategory(LetterKindCategory.LetterVariation)) {
                        return false;
                    }
                    break;
                case LetterFilters.ExcludeLetterVariations.AllButAlefHamza:
                    if (data.IsOfKindCategory(LetterKindCategory.LetterVariation) && data.Tag != "AlefHamzaVariation") {
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        public bool FilterByDipthongs(bool excludeDiphthongs, LetterData data)
        {
            if (excludeDiphthongs && data.Kind == LetterDataKind.Diphthong) {
                return false;
            }
            return true;
        }

        #endregion

        #region Letter -> Letter

        public List<LetterData> GetAllBaseLetters()
        {
            var p = new LetterFilters(excludeDiacritics: LetterFilters.ExcludeDiacritics.All,
                excludeLetterVariations: LetterFilters.ExcludeLetterVariations.All, excludeDiphthongs: true);
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
            return dbManager.FindLetterData(x =>
                x.Type == LetterDataType.Consonant || x.Type == LetterDataType.Powerful && CheckFilters(filters, x));
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
            if (data.BaseLetter == "") {
                return null;
            }
            return dbManager.FindLetterData(x => x.Id == data.BaseLetter)[0];
        }

        public LetterData GetSymbolOf(string letterId)
        {
            var data = dbManager.GetLetterDataById(letterId);
            if (data.Symbol == "") {
                return null;
            }
            return dbManager.FindLetterData(x => x.Id == data.Symbol)[0];
        }

        public List<LetterData> GetLettersWithBase(string letterId)
        {
            var baseData = dbManager.GetLetterDataById(letterId);
            return dbManager.FindLetterData(x => x.BaseLetter == baseData.Id);
        }

        public List<LetterData> ConvertToLettersWithForcedForms(LetterData baseForVariation)
        {
            return new List<LetterForm>(baseForVariation.GetAvailableForms()).ConvertAll(f =>
            {
                var l = baseForVariation.Clone();
                l.ForcedLetterForm = f;
                return l;
            });
        }

        public LetterData ConvertToLetterWithForcedForm(LetterData baseForVariation, LetterForm form)
        {
            var l = baseForVariation.Clone();
            l.ForcedLetterForm = form;
            return l;
        }

        public LetterData ForceEqualityStrictness(LetterData data, LetterEqualityStrictness letterEqualityStrictness)
        {
            var clone = data.Clone();
            //clone.EqualityStrictness = letterEqualityStrictness;
            return clone;
        }

        public List<LetterData> ForceEqualityStrictness(List<LetterData> dataList, LetterEqualityStrictness letterEqualityStrictness)
        {
            return dataList.ConvertAll(data => ForceEqualityStrictness(data, letterEqualityStrictness));
        }

        #endregion

        #region Word -> Letter

        private Dictionary<string, List<LetterData>> unseparatedWordsToLetterCache = new Dictionary<string, List<LetterData>>();
        private Dictionary<string, List<LetterData>> separatedWordsToLetterCache = new Dictionary<string, List<LetterData>>();

        /*private List<string> GetLetterIdsInWordData(WordData wordData)
        {
            List<string> letter_ids_list = null;
            if (ForceUnseparatedLetters)
            {
                if (!unseparatedWordsToLetterCache.ContainsKey(wordData.Id))
                {
                    var parts = ArabicAlphabetHelper.AnalyzeData(AppManager.I.DB.StaticDatabase, wordData, separateVariations: false);
                    letter_ids_list = parts.ConvertAll(p => p.letter.Id);
                    unseparatedWordsToLetterCache[wordData.Id] = letter_ids_list;
                }
                letter_ids_list = unseparatedWordsToLetterCache[wordData.Id];
            } else {
                letter_ids_list = new List<string>(wordData.Letters);
            }
            return letter_ids_list;
        }*/

        public List<LetterData> GetLettersInWord(string wordId)
        {
            WordData wordData = dbManager.GetWordDataById(wordId);
            return GetLettersInWord(wordData);
        }
        public List<LetterData> GetLettersInWord(WordData wordData)
        {
            // @note: this will always retrieve all letters with their forms, the strictness will then define whether that has any consequence or not
            List<LetterData> letters = null;
            var dictCache = ForceUnseparatedLetters ? unseparatedWordsToLetterCache : separatedWordsToLetterCache;
            bool separateVariations = !ForceUnseparatedLetters;
            if (!dictCache.ContainsKey(wordData.Id))
            {
                var parts = ArabicAlphabetHelper.AnalyzeData(dbManager.StaticDatabase, wordData, separateVariations: separateVariations);
                letters = parts.ConvertAll(x => ConvertToLetterWithForcedForm(x.letter, x.letterForm));
                dictCache[wordData.Id] = letters;
            }
            letters = dictCache[wordData.Id];
            return letters;
        }


        public List<LetterData> GetLettersNotInWords(params WordData[] tabooArray)
        {
            return GetLettersNotInWords(LetterKindCategory.Real, tabooArray);
        }

        public List<LetterData> GetLettersNotInWords(LetterKindCategory category = LetterKindCategory.Real, params WordData[] tabooArray)
        {
            var lettersInWords = new HashSet<LetterData>();
            foreach (var tabooWordData in tabooArray) {
                var tabooWordDataLetters = GetLettersInWord(tabooWordData);
                lettersInWords.UnionWith(tabooWordDataLetters);
            }
            var lettersNotInWords = dbManager.FindLetterData(x => !lettersInWords.Contains(x) && x.IsOfKindCategory(category));
            return lettersNotInWords;
        }

        public List<LetterData> GetLettersNotInWord(WordData wordData, LetterKindCategory category = LetterKindCategory.Real)
        {
            var lettersInWord = GetLettersInWord(wordData);
            var lettersNotInWord = dbManager.FindLetterData(x => !lettersInWord.Contains(x) && x.IsOfKindCategory(category));
            return lettersNotInWord;
        }

        public List<LetterData> GetCommonLettersInWords(LetterEqualityStrictness letterEqualityStrictness, params WordData[] words)
        {
            // TODO: use letterEqualityStrictness!
            Dictionary<LetterData, int> countDict = new Dictionary<LetterData, int>();
            foreach (var word in words)
            {
                var nonRepeatingLettersOfWord = new HashSet<LetterData>();

                var letters = GetLettersInWord(word);
                foreach (var letter in letters)
                {
                    nonRepeatingLettersOfWord.Add(letter);
                }

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
            if (filters.excludeArticles && data.Article != WordDataArticle.None) {
                return false;
            }
            if (filters.requireDrawings && !data.HasDrawing()) {
                return false;
            }
            if (filters.excludeColorWords && data.Category == WordDataCategory.Color) {
                return false;
            }
            if (filters.excludePluralDual && data.Form != WordDataForm.Singular) {
                return false;
            }
            if (filters.excludeDiacritics && this.WordHasDiacriticCombo(data)) {
                return false;
            }
            if (filters.excludeLetterVariations && this.WordHasLetterVariations(data)) {
                return false;
            }
            if (filters.requireDiacritics && !this.WordHasDiacriticCombo(data)) {
                return false;
            }
            return true;
        }

        private bool WordHasDiacriticCombo(WordData data)
        {
            foreach (var letter in GetLettersInWord(data)) {
                if (letter.IsOfKindCategory(LetterKindCategory.DiacriticCombo)) {
                    return true;
                }
            }
            return false;
        }

        private bool WordHasLetterVariations(WordData data)
        {
            foreach (var letter in GetLettersInWord(data)) {
                if (letter.IsOfKindCategory(LetterKindCategory.LetterVariation)) {
                    return true;
                }
            }
            return false;
        }

        public int WordContainsLetterTimes(WordData selectedWord, LetterData containedLetter, LetterEqualityStrictness letterEqualityStrictness = LetterEqualityStrictness.LetterOnly)
        {
            var wordLetters = GetLettersInWord(selectedWord);
            int count = 0;
            foreach (var letter in wordLetters)
                if (letter.IsSameLetterAs(containedLetter, letterEqualityStrictness))
                    count++;
            return count;
        }

        public bool WordContainsLetterWithForm(WordData selectedWord, LetterData containedLetter, LetterForm selectedForm)
        {
            // TODO: use the basic GetWordsInLetter now
            //if (containedLetter.Id == "lam_alef") Debug.Log("Looking for lam-alef in " + selectedWord);
            foreach (var l in ArabicAlphabetHelper.FindLetter(dbManager, selectedWord, containedLetter))
            {
                //if (l.letter.Id == "lam_alef") Debug.Log("Lam alef form " + l.letterForm + " in word " + selectedWord);
                if (l.letterForm == selectedForm)
                {
                    return true;
                }
            }
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

        private List<WordData> GetWordsNotIn(WordFilters filters, List<WordData> tabooWords)
        {
            return dbManager.FindWordData(word => !tabooWords.Contains(word) && CheckFilters(filters, word));
        }

        public List<WordData> GetWordsNotIn(WordFilters filters, params WordData[] tabooWords)
        {
            var tabooList = new List<WordData>(tabooWords);
            return GetWordsNotIn(filters, tabooList);
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

        public List<WordData> GetWordsWithLetter(WordFilters filters, LetterData okLetter, LetterEqualityStrictness letterEqualityStrictness = LetterEqualityStrictness.LetterOnly)
        {
            return GetWordsByLetters(filters, new[] { okLetter }, null, letterEqualityStrictness);
        }

        public List<WordData> GetWordsWithLetters(WordFilters filters, LetterEqualityStrictness letterEqualityStrictness = LetterEqualityStrictness.LetterOnly, params LetterData[] okLetters)
        {
            return GetWordsByLetters(filters, okLetters, null, letterEqualityStrictness);
        }

        public List<WordData> GetWordsWithoutLetter(WordFilters filters, LetterData tabooLetter, LetterEqualityStrictness letterEqualityStrictness = LetterEqualityStrictness.LetterOnly)
        {
            return GetWordsByLetters(filters, null, new[] { tabooLetter }, letterEqualityStrictness);
        }

        public List<WordData> GetWordsWithoutLetters(WordFilters filters, LetterEqualityStrictness letterEqualityStrictness = LetterEqualityStrictness.LetterOnly, params LetterData[] tabooLetters)
        {
            return GetWordsByLetters(filters, null, tabooLetters, letterEqualityStrictness);
        }

        private List<WordData> GetWordsByLetters(WordFilters filters, LetterData[] okLettersArray, LetterData[] tabooLettersArray, LetterEqualityStrictness letterEqualityStrictness = LetterEqualityStrictness.LetterOnly)
        {
            if (okLettersArray == null) okLettersArray = new LetterData[] { };
            if (tabooLettersArray == null) tabooLettersArray = new LetterData[] { };

            var okLetters = new HashSet<LetterData>(okLettersArray);
            var tabooLetters = new HashSet<LetterData>(tabooLettersArray);

            List<WordData> wordsByLetters = dbManager.FindWordData(word => {
                if (!CheckFilters(filters, word)) { return false; }

                var lettersInWord = GetLettersInWord(word);

                if (tabooLetters.Count > 0) {
                    foreach (var letter in lettersInWord) {
                        if (tabooLetters.Contains(letter)) {
                            return false;
                        }
                    }
                }

                if (okLetters.Count > 0) {
                    bool hasAllOkLetters = true;
                    foreach (var okLetter in okLetters) {
                        bool hasThisLetter = false;
                        foreach (var letter in lettersInWord) {
                            if (letter.IsSameLetterAs(okLetter, letterEqualityStrictness))
                            {
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
            return wordsByLetters;
        }

        public bool WordContainsAnyLetter(WordData word, IEnumerable<LetterData> letters)
        {
            var containedLetters = GetLettersInWord(word);
            foreach (var letter in letters) {
                if (containedLetters.Contains(letter)) {
                    return true;
                }
            }
            return false;
        }

        public bool WordHasAllLettersInCommonWith(WordData word, List<WordData> words)
        {
            var lettersInWord = GetLettersInWord(word);
            foreach (var letter in lettersInWord) {
                if (!IsLetterContainedInAnyWord(letter, words)) {
                    return false;
                }
            }
            return true;
        }

        public bool IsLetterContainedInAnyWord(LetterData letter, List<WordData> words)
        {
            foreach (var word in words) {
                var containedLetters = GetLettersInWord(word);
                if (containedLetters.Contains(letter)) {
                    return true;
                }
            }
            return false;
        }

        public bool AnyWordContainsLetter(LetterData letter, IEnumerable<WordData> words)
        {
            foreach (var word in words) {
                if (GetLettersInWord(word).Contains(letter)) {
                    return true;
                }
            }
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
            if (wordFilters == null) { wordFilters = new WordFilters(); }
            var phraseData = dbManager.GetPhraseDataById(phraseId);
            return GetWordsInPhrase(phraseData, wordFilters);
        }

        public List<WordData> GetWordsInPhrase(PhraseData phraseData, WordFilters wordFilters)
        {
            var words_ids_list = new List<string>(phraseData.Words);
            var inputList = dbManager.FindWordData(x => words_ids_list.Contains(x.Id) && CheckFilters(wordFilters, x));
            var orderedOutputList = new List<WordData>();
            words_ids_list.ForEach(id => {
                var word = inputList.Find(x => x.Id.Equals(id));
                if (word != null) {
                    orderedOutputList.Add(word);
                }
            });
            return orderedOutputList;
        }

        public List<WordData> GetAnswersToPhrase(PhraseData phraseData, WordFilters wordFilters)
        {
            var words_ids_list = new List<string>(phraseData.Answers);
            var list = dbManager.FindWordData(x => words_ids_list.Contains(x.Id) && CheckFilters(wordFilters, x));
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

            if (phraseFilters.requireWords && nOkWords == 0) {
                return false;
            }
            if (phraseFilters.requireAtLeastTwoWords && nOkWords <= 1) {
                return false;
            }
            if (phraseFilters.requireAnswersOrWords && nOkAnswers == 0 && nOkWords == 0) {
                return false;
            }

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
            if (data.Linked == "") { return null; }
            return dbManager.FindPhraseData(x => x.Id == data.Linked)[0];
        }

        #endregion

        #region Word -> Phrase

        public List<PhraseData> GetPhrasesWithWords(params string[] okWordsArray)
        {
            if (okWordsArray == null) { okWordsArray = new string[] { }; }

            var okWords = new HashSet<string>(okWordsArray);

            var phrasesList = dbManager.FindPhraseData(x => {
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
                    if (!hasAllOkWords) { return false; }
                }
                return true;
            }
            );
            return phrasesList;
        }

        #endregion

    }
}