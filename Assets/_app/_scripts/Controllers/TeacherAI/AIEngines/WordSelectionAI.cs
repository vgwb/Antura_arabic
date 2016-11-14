using System.Collections;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S.Teacher
{
    /// <summary>
    /// Handles the selection of what words a minigame should use, given a playsession
    /// </summary>
    public class WordSelectionAI 
    {
        // References
        private DatabaseManager dbManager;
        //private PlayerProfile playerProfile;
        private TeacherAI teacher;
        private WordHelper wordHelper;

        // DEPRECATED
        private List<string> currentAlreadyPickedLetterIds = new List<string>();

        // Innert state
        private HashSet<LetterData> currentPlaySessionLetters = new HashSet<LetterData>();
        private HashSet<string> currentlyAvailableWordData = new HashSet<string>();
        private HashSet<string> currentlyAvailablePhraseData = new HashSet<string>();

        public WordSelectionAI(DatabaseManager _dbManager, PlayerProfile _playerProfile, TeacherAI _teacher, WordHelper _wordHelper)
        {
            this.dbManager = _dbManager;
            //this.playerProfile = _playerProfile;
            this.teacher = _teacher;
            this.wordHelper = _wordHelper;
        }

        public void InitialiseNewPlaySession(string currentPlaySessionId)
        {
            currentAlreadyPickedLetterIds.Clear();

            currentPlaySessionLetters = new HashSet<LetterData>(GetLettersInPlaySession(currentPlaySessionId));
            UnityEngine.Debug.Log("Current play session letters: " + currentPlaySessionLetters.Count);
        }

        public List<Db.WordData> PerformWordSelection(string playSessionId, int numberToSelect)
        {
            var playData = dbManager.GetPlaySessionDataById(playSessionId);
            List<Db.ScoreData> word_scoreData_list = dbManager.FindScoreDataByQuery("SELECT * FROM ScoreData WHERE TableName = 'Words'");

            List<Db.WordData> selectedWordData_list = new List<Db.WordData>();

            int nRemainingToSelect = numberToSelect;

            // TODO: also add some logic when selecting in the builders so that words that are more recent are used MULTIPLE TIMES
            
            // TODO: these 'recent/non-recent' rules should be encoded as WEIGHTS instead!

            // First check current Words
            //UnityEngine.Debug.Log("Selecting " + nRemainingToSelect + " MAIN words");
            SelectWordsFrom(playData.Words, selectedWordData_list, word_scoreData_list, ref nRemainingToSelect);

            // ... if it's not enough, check previous Words
            if (nRemainingToSelect > 0)
            {
                //UnityEngine.Debug.Log("Selecting " + nRemainingToSelect + " PREVIOUS words");
                SelectWordsFrom(playData.Words_previous, selectedWordData_list, word_scoreData_list, ref nRemainingToSelect);
            }

            // ... if that's still not enough, check words from past sessions
            if (nRemainingToSelect > 0)
            {
                //UnityEngine.Debug.Log("Selecting " + nRemainingToSelect + " PAST words");
                SelectWordsFrom(this.GetAllWordIdsFromPreviousPlaySessions(playData), selectedWordData_list, word_scoreData_list, ref nRemainingToSelect);
            }

            // ... if that's still not enough, there is some issue.
            if (nRemainingToSelect > 0)
            {
                UnityEngine.Debug.LogWarning("Warning: could not find enough words for play session " + playSessionId + " (found " + (numberToSelect - nRemainingToSelect) + " out of " + (numberToSelect) + ")");
            }

            return selectedWordData_list;
        }

        void SelectWordsFrom(string[] currentWordIds, List<Db.WordData> selectedWordData_list, List<Db.ScoreData> word_scoreData_list, ref int nRemainingToSelect)
        {

            List<Db.WordData> wordData_list = new List<Db.WordData>();
            List<float> weights_list = new List<float>();
            foreach (var word_Id in currentWordIds)
            {
                if (currentAlreadyPickedLetterIds.Contains(word_Id))
                {
                    continue;
                }

                float cumulativeWeight = 0;
                var word_scoreData = word_scoreData_list.Find(x => x.ElementId == word_Id);
                float currentWordScore = 0;
                int daysSinceLastScore = 0;
                if (word_scoreData != null)
                {
                    var timespanFromLastScoreToNow = GenericUtilities.GetTimeSpanBetween(word_scoreData.LastAccessTimestamp, GenericUtilities.GetTimestampForNow());
                    daysSinceLastScore = timespanFromLastScoreToNow.Days;
                    currentWordScore = word_scoreData.Score;
                }

                // Score Weight [0,1]: higher the lower the score [-1,1] is
                var scoreWeight = 0.5f * (1 - currentWordScore);
                cumulativeWeight += scoreWeight * ConfigAI.word_scoreWeight;

                // Always skip letters that have a score weight of zero
                if (scoreWeight == 0)
                {
                    continue;
                }

                // RecentPlay Weight  [1,0]: higher the more in the past we saw that word
                const float dayLinerWeightDecrease = 1f / ConfigAI.daysForMaximumRecentPlayMalus;
                float weightMalus = daysSinceLastScore * dayLinerWeightDecrease;
                float recentPlayWeight = 1f - UnityEngine.Mathf.Min(1, weightMalus);
                cumulativeWeight += recentPlayWeight * ConfigAI.word_recentPlayWeight;

                //UnityEngine.Debug.Log("Word " + word_Id + " score: " + currentWordScore + " days " + daysSinceLastScore);

                // Save cumulative weight
                if (cumulativeWeight <= 0)
                {
                    continue;
                }
                weights_list.Add(cumulativeWeight);

                // Add the data to the list
                var wordData = dbManager.GetWordDataById(word_Id);
                wordData_list.Add(wordData);
            }

            //UnityEngine.Debug.Log("Number of words: " + wordData_list.Count);

            // Select some words
            if (wordData_list.Count > 0) {
                int nToSelectFromCurrentList = UnityEngine.Mathf.Min(wordData_list.Count, nRemainingToSelect);
                var chosenWords = RandomHelper.RouletteSelectNonRepeating(wordData_list, weights_list, nToSelectFromCurrentList);
                selectedWordData_list.AddRange(chosenWords);
                nRemainingToSelect -= nToSelectFromCurrentList;
                currentAlreadyPickedLetterIds.AddRange(chosenWords.ConvertAll<string>(x => x.Id));
            }
        }


        #region Data Selection logic

        public List<Db.LetterData> SelectLetters(System.Func<List<Db.LetterData>> selectionFunction, SelectionParameters selectionParams)
        {
            // All data to use
            var foundDataList = selectionFunction();

            // Selection filtering
            if (!selectionParams.ignoreJourney && !ConfigAI.forceJourneyIgnore)
            {
                foundDataList = foundDataList.FindAll(x => currentPlaySessionLetters.Contains(x));
            }

            if (foundDataList.Count < selectionParams.nRequired && selectionParams.severity == SelectionSeverity.AllRequired)
            {
                throw new System.Exception("The teacher could not find " + selectionParams.nRequired + " data instances as required by the game.");
            }

            return foundDataList;
        }

        #endregion


        // @todo: move these to JourneyHelper instead
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

        private List<LetterData> GetLettersInPlaySession(string psId, bool pastSessionsToo = false)
        {
            var psData = dbManager.GetPlaySessionDataById(psId);

            HashSet<string> ids_set = new HashSet<string>();
            ids_set.UnionWith(psData.Letters);
            if (pastSessionsToo) ids_set.UnionWith(this.GetAllLetterIdsFromPreviousPlaySessions(psData));

            List<string> ids_list = new List<string>(ids_set);
            return ids_list.ConvertAll(x => dbManager.GetLetterDataById(x));
        }

        private string[] GetAllLetterIdsFromPreviousPlaySessions(PlaySessionData current_ps)
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

        private List<WordData> GetWordsInLearningBlock(string lbId, bool previousToo = true, bool pastBlocksToo = false)
        {
            var lbData = dbManager.GetLearningBlockDataById(lbId);
            var psData_list = dbManager.GetPlaySessionsOfLearningBlock(lbData);

            HashSet<WordData> wordData_set = new HashSet<WordData>();
            foreach (var psData in psData_list)
            {
                var ps_wordData = GetWordsInPlaySession(psData.Id, previousToo, pastBlocksToo);
                wordData_set.UnionWith(ps_wordData);
            }
            return new List<WordData>(wordData_set);
        }

        private List<WordData> GetWordsInPlaySession(string psId, bool previousToo = false, bool pastSessionsToo = false)
        {
            var psData = dbManager.GetPlaySessionDataById(psId);

            HashSet<string> ids_set = new HashSet<string>();
            ids_set.UnionWith(psData.Words);
            if (previousToo) ids_set.UnionWith(psData.Words_previous);
            if (pastSessionsToo) ids_set.UnionWith(this.GetAllWordIdsFromPreviousPlaySessions(psData));

            List<string> ids_list = new List<string>(ids_set);
            return ids_list.ConvertAll(x => dbManager.GetWordDataById(x));
        }

        private string[] GetAllWordIdsFromPreviousPlaySessions(PlaySessionData current_ps)
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

        private List<PhraseData> GePhrasesInLearningBlock(string lbId, bool previousToo = true, bool pastBlocksToo = false)
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

        private List<PhraseData> GetPhrasesInPlaySession(string lbId, bool previousToo = false, bool pastSessionsToo = false)
        {
            var psData = dbManager.GetPlaySessionDataById(lbId);

            HashSet<string> ids_set = new HashSet<string>();
            ids_set.UnionWith(psData.Phrases);
            if (previousToo) ids_set.UnionWith(psData.Phrases_previous);
            if (pastSessionsToo) ids_set.UnionWith(this.GetAllPhraseIdsFromPreviousPlaySessions(psData));

            List<string> ids_list = new List<string>(ids_set);
            return ids_list.ConvertAll(x => dbManager.GetPhraseDataById(x));
        }

        private string[] GetAllPhraseIdsFromPreviousPlaySessions(PlaySessionData current_ps)
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