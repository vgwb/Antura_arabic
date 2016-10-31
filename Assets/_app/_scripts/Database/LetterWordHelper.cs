using System.Collections;
using System.Collections.Generic;

namespace EA4S.Db
{
    public class LetterWordHelper
    {
        private DatabaseManager dbManager;

        public LetterWordHelper(DatabaseManager _dbManager)
        {
            this.dbManager = _dbManager;
        }

        #region Letter -> Letter

        public List<LetterData> GetLettersNotIn(params string[] tabooArray)
        {
            var tabooList = new List<string>(tabooArray);
            return dbManager.FindLetterData(x => !tabooList.Contains(x.Id));
        }

        public List<LetterData> GetLettersByKind(LetterDataKind choice)
        {
            return dbManager.FindLetterData(x => x.Kind == choice);
        }

        public List<LetterData> GetLettersBySunMoon(LetterDataSunMoon choice)
        {
            return dbManager.FindLetterData(x => x.SunMoon == choice);
        }

        public List<LetterData> GetLettersByType(LetterDataType choice)
        {
            return dbManager.FindLetterData(x => x.Type == choice);
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

        public List<LetterData> GetLettersInWord(string wordId)
        {
            WordData wordData = dbManager.GetWordDataById(wordId);
            var letter_ids_list = new List<string>(wordData.Letters);
            List<LetterData> list = dbManager.FindLetterData(x => letter_ids_list.Contains(x.Id));
            return list;

        }

        public List<LetterData> GetLettersNotInWord(string wordId)
        {
            WordData wordData = dbManager.GetWordDataById(wordId);
            var letter_ids_list = new List<string>(wordData.Letters);
            List<LetterData> list = dbManager.FindLetterData(x => !letter_ids_list.Contains(x.Id));
            return list;

        }

        #endregion


        #region Word -> Word

        public List<WordData> GetWordsNotIn(params string[] tabooArray)
        {
            var tabooList = new List<string>(tabooArray);
            return dbManager.FindWordData(x => !tabooList.Contains(x.Id));
        }

        public List<WordData> GetWordsByCategory(WordDataCategory choice)
        {
            return dbManager.FindWordData(x => x.Category == choice);
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
                foreach(var letter_id in x.Letters)
                {
                    if (okLetters.Count > 0 && !okLetters.Contains(letter_id))
                    {
                        return false;
                    }
                    if (tabooLetters.Count > 0 && tabooLetters.Contains(letter_id))
                    {
                        return false;
                    }
                }
                return true;
                }
            );
            return list;

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

        #region LearningBlock -> Letter

        public List<LetterData> GetLettersInLearningBlock(string lbId, bool pastSessionsToo = false)
        {
            var lbData = dbManager.GetLearningBlockDataById(lbId);

            List<string> ids_list = new List<string>();
            ids_list.AddRange(lbData.Letters);
            if (pastSessionsToo) ids_list.AddRange(this.GetAllLetterIdsFromPreviousLearningBlocks(lbData));

            return ids_list.ConvertAll(x => dbManager.GetLetterDataById(x));
        }

        public string[] GetAllLetterIdsFromPreviousLearningBlocks(Db.LearningBlockData current_lb)
        {
            // @note: this assumes that all learning blocks are correctly ordered
            var all_lb_list = dbManager.GetAllLearningBlockData();
            int index_current_lb = all_lb_list.IndexOf(current_lb);

            List<string> all_ids = new List<string>();
            for (int prev_lb_id = 0; prev_lb_id < index_current_lb; prev_lb_id++)
            {
                all_ids.AddRange(all_lb_list[prev_lb_id].Letters);
            }

            return all_ids.ToArray();
        }
        #endregion

        #region LearningBlock -> Word

        public List<WordData> GetWordsInLearningBlock(string lbId, bool previousToo = false, bool pastSessionsToo = false)
        {
            var lbData = dbManager.GetLearningBlockDataById(lbId);

            List<string> ids_list = new List<string>();
            ids_list.AddRange(lbData.Words);
            if (previousToo) ids_list.AddRange(lbData.Words_previous);
            if (pastSessionsToo) ids_list.AddRange(this.GetAllWordIdsFromPreviousLearningBlocks(lbData));

            return ids_list.ConvertAll(x => dbManager.GetWordDataById(x));
        }

        public string[] GetAllWordIdsFromPreviousLearningBlocks(Db.LearningBlockData current_lb)
        {
            // @note: this assumes that all learning blocks are correctly ordered
            var all_lb_list = dbManager.GetAllLearningBlockData();
            int index_current_lb = all_lb_list.IndexOf(current_lb);

            List<string> all_ids = new List<string>();
            for (int prev_lb_id = 0; prev_lb_id < index_current_lb; prev_lb_id++)
            {
                all_ids.AddRange(all_lb_list[prev_lb_id].Words);
                all_ids.AddRange(all_lb_list[prev_lb_id].Words_previous);
            }

            return all_ids.ToArray();
        }

        #endregion

        #region LearningBlock -> Phrase

        public List<PhraseData> GetPhrasesInLearningBlock(string lbId, bool previousToo = false, bool pastSessionsToo = false)
        {
            var lbData = dbManager.GetLearningBlockDataById(lbId);

            List<string> ids_list = new List<string>();
            ids_list.AddRange(lbData.Phrases);
            if (previousToo) ids_list.AddRange(lbData.Phrases_previous);
            if (pastSessionsToo) ids_list.AddRange(this.GetAllPhraseIdsFromPreviousLearningBlocks(lbData));

            return ids_list.ConvertAll(x => dbManager.GetPhraseDataById(x));
        }

        public string[] GetAllPhraseIdsFromPreviousLearningBlocks(Db.LearningBlockData current_lb)
        {
            // @note: this assumes that all learning blocks are correctly ordered
            var all_lb_list = dbManager.GetAllLearningBlockData();
            int index_current_lb = all_lb_list.IndexOf(current_lb);

            List<string> all_ids = new List<string>();
            for (int prev_lb_id = 0; prev_lb_id < index_current_lb; prev_lb_id++)
            {
                all_ids.AddRange(all_lb_list[prev_lb_id].Phrases);
                all_ids.AddRange(all_lb_list[prev_lb_id].Phrases_previous);
            }

            return all_ids.ToArray();
        }
        #endregion


    }
}
