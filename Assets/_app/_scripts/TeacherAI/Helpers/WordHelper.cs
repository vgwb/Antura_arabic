using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

namespace EA4S.Db
{
    /// <summary>
    /// Provides helpers to get correct letter/word/phrase data according to the teacher's logic and based on the player's progression
    /// </summary>
    public class WordHelper
    {
        private DatabaseManager dbManager;

        public WordHelper(DatabaseManager _dbManager)
        {
            this.dbManager = _dbManager;
        }

        #region Letter Utilities


        List<string> mainDiacriticsIds = new List<string>() {"fathah","dammah","kasrah","sukun"}; // HACK: this is just for dancing dots, to be removed later on
        private bool CheckFilters(LetterFilters filters, LetterData data)
        {
            if (filters.requireDiacritics && !data.IsOfKindCategory(LetterKindCategory.DiacriticCombo)) return false;
            if (filters.excludeDiacritics && data.IsOfKindCategory(LetterKindCategory.DiacriticCombo)) return false;
            if (filters.excludeDiacritics_keepMain && data.IsOfKindCategory(LetterKindCategory.DiacriticCombo)
                && !mainDiacriticsIds.Contains(data.Symbol)) return false;
            if (filters.excludeLetterVariations && data.IsOfKindCategory(LetterKindCategory.LetterVariation)) return false;
            if (data.IsOfKindCategory(LetterKindCategory.Symbol)) return false; // always skip symbols
            return true;
        }

        #endregion

        #region Letter -> Letter

        public List<LetterData> GetAllBaseLetters()
        {
            var p = new LetterFilters(excludeDiacritics: true, excludeLetterVariations: true);
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

        public List<LetterData> GetLettersInWord(WordData wordData)
        {
            var letter_ids_list = new List<string>(wordData.Letters);
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
                letter_ids_list.UnionWith(tabooWordData.Letters);
            }
            List<LetterData> list = dbManager.FindLetterData(x => !letter_ids_list.Contains(x.Id) && x.IsOfKindCategory(category));
            return list;
        }

        public List<LetterData> GetLettersNotInWord(string wordId, LetterKindCategory category = LetterKindCategory.Real)
        {
            WordData wordData = dbManager.GetWordDataById(wordId);
            var letter_ids_list = new List<string>(wordData.Letters);
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

                if (tabooLetters.Count > 0) {
                    foreach (var letter_id in x.Letters) {
                        if (tabooLetters.Contains(letter_id)) {
                            return false;
                        }
                    }
                }

                if (okLetters.Count > 0) {
                    bool hasAllOkLetters = true;
                    foreach (var okLetter in okLetters) {
                        bool hasThisLetter = false;
                        foreach (var letter_id in x.Letters) {
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

        #endregion

        #region Phrase -> Word

        public List<WordData> GetWordsInPhrase(string phraseId, WordFilters wordFilters = null)
        {
            if (wordFilters == null) wordFilters = new WordFilters();
            PhraseData data = dbManager.GetPhraseDataById(phraseId);
            return GetWordsInPhrase(data, wordFilters);
        }

        public List<WordData> GetWordsInPhrase(PhraseData phraseData, WordFilters wordFilters)
        {
            var words_ids_list = new List<string>(phraseData.Words);
            List<WordData> list = dbManager.FindWordData(x => words_ids_list.Contains(x.Id) && CheckFilters(wordFilters, x));
            return list;
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
