using System.Collections.Generic;

namespace EA4S.Db
{
    public class WordHelper
    {
        private DatabaseManager dbManager;
        //private TeacherAI teacher;

        public WordHelper(DatabaseManager _dbManager, TeacherAI _teacher)
        {
            this.dbManager = _dbManager;
            //this.teacher = _teacher;
        }

        #region Letter -> Letter

        public List<LetterData> GetAllRealLetters()
        {
            return dbManager.FindLetterData(x => x.IsRealLetter());
        }

        private List<LetterData> GetRealLettersNotIn(List<string> tabooList)
        {
            return dbManager.FindLetterData(
                x => !tabooList.Contains(x.Id)
                && x.IsRealLetter()
            );
        }
        public List<LetterData> GetRealLettersNotIn(params LetterData[] tabooArray)
        {
            var tabooList = new List<LetterData>(tabooArray);
            return GetRealLettersNotIn(tabooList.ConvertAll(x => x.Id));
        }

        public List<LetterData> GetLettersByKind(LetterDataKind choice)
        {
            return dbManager.FindLetterData(x => x.Kind == choice);
        }

        public List<LetterData> GetRealLettersBySunMoon(LetterDataSunMoon choice)
        {
            return dbManager.FindLetterData(
                x => x.SunMoon == choice
                    && x.IsRealLetter());
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
            List<LetterData> list = dbManager.FindLetterData(
                x => !letter_ids_list.Contains(x.Id)
                    && x.IsRealLetter());
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
                    bool hasAtLeastOneOk = false;
                    foreach (var letter_id in x.Letters)
                    {
                        if (okLetters.Contains(letter_id))
                        {
                            hasAtLeastOneOk = true;
                            break;
                        }
                    }
                    if (!hasAtLeastOneOk) return false;
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

        #region LearningBlock / PlaySession -> Letter

        public List<LetterData> GetLettersInLearningBlock(string lbId, bool pastBlocksToo = false)
        {
            var lbData = dbManager.GetLearningBlockDataById(lbId);
            var psData_list = dbManager.GetPlaySessionsOfLearningBlock(lbData);

            HashSet<LetterData> letterData_set = new HashSet<LetterData>();
            foreach (var psData in psData_list)
            {
                var ps_letterData = GetLettersInPlaySession(psData.Id, pastBlocksToo);
                letterData_set.UnionWith(ps_letterData);
            }
            return new List<LetterData>(letterData_set);
        }

        public List<LetterData> GetLettersInPlaySession(string psId, bool pastSessionsToo = false)
        {
            var psData = dbManager.GetPlaySessionDataById(psId);

            HashSet<string> ids_set = new HashSet<string>();
            ids_set.UnionWith(psData.Letters);
            if (pastSessionsToo) ids_set.UnionWith(this.GetAllLetterIdsFromPreviousPlaySessions(psData));

            List<string> ids_list = new List<string>(ids_set);
            return ids_list.ConvertAll(x => dbManager.GetLetterDataById(x));
        }

        public string[] GetAllLetterIdsFromPreviousPlaySessions(PlaySessionData current_ps)
        {
            // @note: this assumes that all play sessions are correctly ordered
            var all_ps_list = dbManager.GetAllPlaySessionData();
            int current_id = all_ps_list.IndexOf(current_ps);

            List<string> all_ids = new List<string>();
            for (int prev_id = 0; prev_id < current_id; prev_id++)
            {
                all_ids.AddRange(all_ps_list[prev_id].Letters);
            }

            return all_ids.ToArray();
        }
        #endregion

        #region LearningBlock / PlaySession -> Word

        public List<WordData> GetWordsInLearningBlock(string lbId, bool previousToo = true, bool pastBlocksToo = false)
        {
            var lbData = dbManager.GetLearningBlockDataById(lbId);
            var psData_list = dbManager.GetPlaySessionsOfLearningBlock(lbData);

            HashSet<WordData> wordData_set = new HashSet<WordData>();
            foreach(var psData in psData_list)
            {
                var ps_wordData = GetWordsInPlaySession(psData.Id, previousToo, pastBlocksToo);
                wordData_set.UnionWith(ps_wordData);
            }
            return new List<WordData>(wordData_set);
        }

        public List<WordData> GetWordsInPlaySession(string psId, bool previousToo = false, bool pastSessionsToo = false)
        {
            var psData = dbManager.GetPlaySessionDataById(psId);

            HashSet<string> ids_set = new HashSet<string>();
            ids_set.UnionWith(psData.Words);
            if (previousToo) ids_set.UnionWith(psData.Words_previous);
            if (pastSessionsToo) ids_set.UnionWith(this.GetAllWordIdsFromPreviousPlaySessions(psData));

            List<string> ids_list = new List<string>(ids_set);
            return ids_list.ConvertAll(x => dbManager.GetWordDataById(x));
        }

        public string[] GetAllWordIdsFromPreviousPlaySessions(PlaySessionData current_ps)
        {
            // @note: this assumes that all play sessions are correctly ordered
            var all_ps_list = dbManager.GetAllPlaySessionData();
            int current_id = all_ps_list.IndexOf(current_ps);

            List<string> all_ids = new List<string>();
            for (int prev_id = 0; prev_id < current_id; prev_id++)
            {
                all_ids.AddRange(all_ps_list[prev_id].Words);
                all_ids.AddRange(all_ps_list[prev_id].Words_previous);
            }

            return all_ids.ToArray();
        }

        #endregion

        #region LearningBlock / PlaySession -> Phrase

        public List<PhraseData> GePhrasesInLearningBlock(string lbId, bool previousToo = true, bool pastBlocksToo = false)
        {
            var lbData = dbManager.GetLearningBlockDataById(lbId);
            var psData_list = dbManager.GetPlaySessionsOfLearningBlock(lbData);

            HashSet<PhraseData> phraseData_set = new HashSet<PhraseData>();
            foreach (var psData in psData_list)
            {
                var ps_phraseData = GetPhrasesInPlaySession(psData.Id, previousToo, pastBlocksToo);
                phraseData_set.UnionWith(ps_phraseData);
            }
            return new List<PhraseData>(phraseData_set);
        }

        public List<PhraseData> GetPhrasesInPlaySession(string lbId, bool previousToo = false, bool pastSessionsToo = false)
        {
            var psData = dbManager.GetPlaySessionDataById(lbId);

            HashSet<string> ids_set = new HashSet<string>();
            ids_set.UnionWith(psData.Phrases);
            if (previousToo) ids_set.UnionWith(psData.Phrases_previous);
            if (pastSessionsToo) ids_set.UnionWith(this.GetAllPhraseIdsFromPreviousPlaySessions(psData));

            List<string> ids_list = new List<string>(ids_set);
            return ids_list.ConvertAll(x => dbManager.GetPhraseDataById(x));
        }

        public string[] GetAllPhraseIdsFromPreviousPlaySessions(PlaySessionData current_ps)
        {
            // @note: this assumes that all play sessions are correctly ordered
            var all_ps_list = dbManager.GetAllPlaySessionData();
            int current_id = all_ps_list.IndexOf(current_ps);

            List<string> all_ids = new List<string>();
            for (int prev_id = 0; prev_id < current_id; prev_id++)
            {
                all_ids.AddRange(all_ps_list[prev_id].Phrases);
                all_ids.AddRange(all_ps_list[prev_id].Phrases_previous);
            }

            return all_ids.ToArray();
        }
        #endregion


    }
}
