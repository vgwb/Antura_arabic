using System.Collections.Generic;

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

        public List<LetterData> GetAllRealLetters()
        {
            return dbManager.FindLetterData(x => x.IsRealLetter());
        }

        private List<LetterData> GetRealLettersNotIn(List<string> tabooList)
        {
            return dbManager.FindLetterData(x => !tabooList.Contains(x.Id) && x.IsRealLetter()
            );
        }

        public List<LetterData> GetRealLettersNotIn(params LetterData[] tabooArray)
        {
            var tabooList = new List<LetterData>(tabooArray);
            return GetRealLettersNotIn(tabooList.ConvertAll(x => x.Id));
        }

        public List<LetterData> GetRealLettersByKind(LetterDataKind choice)
        {
            return dbManager.FindLetterData(x => x.Kind == choice && x.IsRealLetter());
        }

        public List<LetterData> GetRealLettersBySunMoon(LetterDataSunMoon choice)
        {
            return dbManager.FindLetterData(x => x.SunMoon == choice && x.IsRealLetter());
        }

        public List<LetterData> GetRealLettersByType(LetterDataType choice)
        {
            return dbManager.FindLetterData(x => x.Type == choice && x.IsRealLetter());
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
            List<LetterData> list = dbManager.FindLetterData(x => letter_ids_list.Contains(x.Id));
            return list;
        }
        public List<LetterData> GetLettersInWord(string wordId)
        {
            WordData wordData = dbManager.GetWordDataById(wordId);
            return GetLettersInWord(wordData);
        }

        public List<LetterData> GetRealLettersNotInWords(params WordData[] tabooArray)
        {
            var letter_ids_list = new HashSet<string>();
            foreach (var tabooWordData in tabooArray)
            {
                letter_ids_list.UnionWith(tabooWordData.Letters);
            }
            List<LetterData> list = dbManager.FindLetterData(x => !letter_ids_list.Contains(x.Id) && x.IsRealLetter());
            return list;
        }

        public List<LetterData> GetRealLettersNotInWord(string wordId)
        {
            WordData wordData = dbManager.GetWordDataById(wordId);
            var letter_ids_list = new List<string>(wordData.Letters);
            List<LetterData> list = dbManager.FindLetterData(x => !letter_ids_list.Contains(x.Id) && x.IsRealLetter());
            return list;
        }

        #endregion

        #region Word -> Word


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
            if (withDrawing)
            {
                return dbManager.FindWordData(x => x.Category == choice && x.HasDrawing());
            }
            else
            {
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

                if (tabooLetters.Count > 0)
                {
                    foreach (var letter_id in x.Letters)
                    {
                        if (tabooLetters.Contains(letter_id))
                        {
                            return false;
                        }
                    }
                }

                if (okLetters.Count > 0)
                {
                    bool hasAllOkLetters = true;
                    foreach(var okLetter in okLetters)
                    {
                        bool hasThisLetter = false;
                        foreach(var letter_id in x.Letters)
                        {
                            if (letter_id == okLetter)
                            {
                                hasThisLetter = true;
                                break;
                            }
                        }
                        if (!hasThisLetter)
                        {
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

        public List<WordData> GetWordOfPhrase(string phraseId)
        {
            // @todo: implement
            return null;
        }

        #endregion

        #region Phrase -> Phrase

        public PhraseData GetLinkedPhraseOf(string startPhraseId)
        {
            var data = dbManager.GetPhraseDataById(startPhraseId);
            if (data.Linked == "") return null;
            return dbManager.FindPhraseData(x => x.Id == data.Linked)[0];
        }

        #endregion

        #region Word -> Phrase

        public List<PhraseData> GetPhrasesWithWord(string wordId)
        {
            // @todo: implement
            return null;
        }

        #endregion

    }
}
