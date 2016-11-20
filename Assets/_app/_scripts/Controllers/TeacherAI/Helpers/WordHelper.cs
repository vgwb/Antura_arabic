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
        //private WordSelectionAI wordSelectionAI;
        //private TeacherAI teacher;

        public WordHelper(DatabaseManager _dbManager, TeacherAI _teacher)// Teacher.WordSelectionAI _wordSelectionAI)
        {
            this.dbManager = _dbManager;
            //this.wordSelectionAI = _wordSelectionAI;
            //this.teacher = _teacher;
        }

        #region Letter -> Letter

        public List<LetterData> GetAllBaseLetters()
        {
            return GetAllLetters(LetterKindCategory.Base);
        }

        public List<LetterData> GetAllLetters(LetterKindCategory category = LetterKindCategory.Real)
        {
            return dbManager.FindLetterData(x => x.IsOfKindCategory(category));
        }

        private List<LetterData> GetLettersNotIn(List<string> tabooList, LetterKindCategory category = LetterKindCategory.Real)
        {
            return dbManager.FindLetterData(x => !tabooList.Contains(x.Id) && x.IsOfKindCategory(category));
        }


        public List<LetterData> GetLettersNotIn(params LetterData[] tabooArray)
        {
            return GetLettersNotIn(LetterKindCategory.Real, tabooArray);
        }
        public List<LetterData> GetLettersNotIn(LetterKindCategory category = LetterKindCategory.Real, params LetterData[] tabooArray)
        {
            var tabooList = new List<LetterData>(tabooArray);
            return GetLettersNotIn(tabooList.ConvertAll(x => x.Id), category);
        }

        public List<LetterData> GetLettersByKind(LetterDataKind choice)
        {
            return dbManager.FindLetterData(x => x.Kind == choice);
        }

        public List<LetterData> GetLettersBySunMoon(LetterDataSunMoon choice, LetterKindCategory category = LetterKindCategory.Real)
        {
            return dbManager.FindLetterData(x => x.SunMoon == choice && x.IsOfKindCategory(category));
        }

        public List<LetterData> GetConsonantLetter(LetterKindCategory category = LetterKindCategory.Real)
        {
            return dbManager.FindLetterData(x => x.Type == LetterDataType.Consonant || x.Type == LetterDataType.Powerful && x.IsOfKindCategory(category));
        }

        public List<LetterData> GetVowelLetter(LetterKindCategory category = LetterKindCategory.Real)
        {
            return dbManager.FindLetterData(x => x.Type == LetterDataType.LongVowel && x.IsOfKindCategory(category));
        }

        public List<LetterData> GetLettersByType(LetterDataType choice, LetterKindCategory category = LetterKindCategory.Real)
        {
            return dbManager.FindLetterData(x => x.Type == choice && x.IsOfKindCategory(category));
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

        #region Word -> Word

        /// <summary>
        /// tranformsf the hex string of the glyph into the corresponding char
        /// </summary>
        /// <returns>The drawing string</returns>
        /// <param name="word">WordData.</param>
        public string GetWordDrawing(WordData word)
        {
            //Debug.Log("the int of hex:" + word.Drawing + " is " + int.Parse(word.Drawing, NumberStyles.HexNumber));
            if (word.Drawing != "") {
                return ((char)int.Parse(word.Drawing, NumberStyles.HexNumber)).ToString();
            }
            return "";
        }

        public List<WordData> GetAllWords()
        {
            return dbManager.GetAllWordData();
        }

        private List<WordData> GetWordsNotIn(List<string> tabooList)
        {
            return dbManager.FindWordData(x => !tabooList.Contains(x.Id));
        }
        public List<WordData> GetWordsNotIn(params WordData[] tabooArray)
        {
            var tabooList = new List<WordData>(tabooArray);
            return GetWordsNotIn(tabooList.ConvertAll(x => x.Id));
        }

        public List<WordData> GetWordsByCategory(WordDataCategory choice, bool withDrawing = false)
        {
            if (choice == WordDataCategory.None) return dbManager.GetAllWordData();
            if (withDrawing) {
                return dbManager.FindWordData(x => x.Category == choice && x.HasDrawing());
            } else {
                return dbManager.FindWordData(x => x.Category == choice);
            }
        }

        public List<WordData> GetWordsByArticle(WordDataArticle choice)
        {
            return dbManager.FindWordData(x => x.Article == choice);
        }

        public List<WordData> GetWordsByForm(WordDataForm choice)
        {
            return dbManager.FindWordData(x => x.Form == choice);
        }

        public List<WordData> GetWordsByKind(WordDataKind choice)
        {
            return dbManager.FindWordData(x => x.Kind == choice);
        }

        #endregion

        #region Letter -> Word

        public List<WordData> GetWordsWithLetter(string okLetter)
        {
            return GetWordsByLetters(new string[] { okLetter }, null);
        }

        public List<WordData> GetWordsWithLetters(params string[] okLetters)
        {
            return GetWordsByLetters(okLetters, null);
        }

        public List<WordData> GetWordsWithoutLetter(string tabooLetter)
        {
            return GetWordsByLetters(null, new string[] { tabooLetter });
        }

        public List<WordData> GetWordsWithoutLetters(params string[] tabooLetters)
        {
            return GetWordsByLetters(null, tabooLetters);
        }

        public List<WordData> GetWordsByLetters(string[] okLettersArray, string[] tabooLettersArray)
        {
            if (okLettersArray == null) okLettersArray = new string[] { };
            if (tabooLettersArray == null) tabooLettersArray = new string[] { };

            var okLetters = new HashSet<string>(okLettersArray);
            var tabooLetters = new HashSet<string>(tabooLettersArray);

            List<WordData> list = dbManager.FindWordData(x => {

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

        public List<WordData> GetWordsInPhrase(string phraseId)
        {
            PhraseData data = dbManager.GetPhraseDataById(phraseId);
            return GetWordsInPhrase(data);
        }

        public List<WordData> GetWordsInPhrase(PhraseData phraseData)
        {
            var words_ids_list = new List<string>(phraseData.Words);
            List<WordData> list = dbManager.FindWordData(x => words_ids_list.Contains(x.Id));
            return list;
        }

        public List<WordData> GetAnswersToPhrase(PhraseData phraseData)
        {
            var words_ids_list = new List<string>(phraseData.Answers);
            List<WordData> list = dbManager.FindWordData(x => words_ids_list.Contains(x.Id));
            return list;
        }


        #endregion

        #region Phrase -> Phrase

        public List<PhraseData> GetAllPhrases()
        {
            return dbManager.GetAllPhraseData();
        }

        public List<PhraseData> GetPhrasesWithAnswers()
        {
            return dbManager.FindPhraseData(x => x.Answers.Length > 0);
        }

        public List<PhraseData> GetPhrasesByCategory(PhraseDataCategory choice)
        {
            return dbManager.FindPhraseData(x => x.Category == choice);
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

        public List<PhraseData> GetPhrasesNotIn(params PhraseData[] tabooArray)
        {
            var tabooList = new List<PhraseData>(tabooArray);
            return dbManager.FindPhraseData(x => !tabooList.Contains(x));
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
