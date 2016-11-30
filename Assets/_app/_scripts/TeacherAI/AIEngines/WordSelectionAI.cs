using System.Collections;
using System.Collections.Generic;
using EA4S.Db;
using System.Linq;

namespace EA4S.Teacher
{
    /// <summary>
    /// Handles the selection of what words a minigame should use, given a playsession
    /// </summary>
    public class WordSelectionAI 
    {
        // References
        private DatabaseManager dbManager;
        private TeacherAI teacher;
        private WordHelper wordHelper;

        // Inner state
        private HashSet<LetterData> journeyLetters = new HashSet<LetterData>();
        private HashSet<WordData> journeyWords = new HashSet<WordData>();
        private HashSet<PhraseData> journeyPhrases = new HashSet<PhraseData>();

        private HashSet<LetterData> currentPlaySessionLetters = new HashSet<LetterData>();
        private HashSet<WordData> currentPlaySessionWords = new HashSet<WordData>();
        private HashSet<PhraseData> currentPlaySessionPhrases = new HashSet<PhraseData>();

        public WordSelectionAI(DatabaseManager _dbManager, PlayerProfile _playerProfile, TeacherAI _teacher, WordHelper _wordHelper)
        {
            this.dbManager = _dbManager;
            this.teacher = _teacher;
            this.wordHelper = _wordHelper;
        }

        public void InitialiseNewPlaySession(string currentPlaySessionId)
        {
            currentPlaySessionLetters = new HashSet<LetterData>(GetLettersInPlaySession(currentPlaySessionId));
            currentPlaySessionWords = new HashSet<WordData>(GetWordsInPlaySession(currentPlaySessionId));
            currentPlaySessionPhrases = new HashSet<PhraseData>(GetPhrasesInPlaySession(currentPlaySessionId, true));

            journeyLetters = new HashSet<LetterData>(GetLettersInPlaySession(currentPlaySessionId, true));
            journeyWords = new HashSet<WordData>(GetWordsInPlaySession(currentPlaySessionId, true, true));
            journeyPhrases = new HashSet<PhraseData>(GetPhrasesInPlaySession(currentPlaySessionId, true, true));

            if (ConfigAI.verboseDataSelection)
            {
                string debugString = "";
                debugString += "--------- TEACHER: play session initialisation (journey " + currentPlaySessionId + ") --------- ";
                debugString += "\n" + journeyLetters.Count + " letters available";
                debugString += "\n" + journeyWords.Count + " words available";
                debugString += "\n" + journeyPhrases.Count + " phrases available";
                UnityEngine.Debug.Log(debugString);
            }
        }

        #region Data Selection logic

        public List<T> SelectData<T>(System.Func<List<T>> builderSelectionFunction, SelectionParameters selectionParams) where T : IData
        {
            // skip if we require 0 values
            if (selectionParams.nRequired == 0 && !selectionParams.getMaxData) return new List<T>();

            string debugString = "";
            //debugString += "--------- TEACHER: data selection --------- ";

            // @note: not the best of solutions, but I do not seem to be able to get more generic than this without rewriting most stuff.
            System.Type typeParameterType = typeof(T);
            HashSet<T> journeyData = null;
            HashSet<T> currentPSData = null;
            DbTables table = DbTables.Letters;
            if (typeParameterType == typeof(LetterData))
            {
                table = DbTables.Letters;
                journeyData = new HashSet<T>(journeyLetters.Cast<T>());
                currentPSData = new HashSet<T>(currentPlaySessionLetters.Cast<T>());
            }
            else if (typeParameterType == typeof(WordData))
            {
                table = DbTables.Words;
                journeyData = new HashSet<T>(journeyWords.Cast<T>());
                currentPSData = new HashSet<T>(currentPlaySessionWords.Cast<T>());
            }
            else if (typeParameterType == typeof(PhraseData))
            {
                table = DbTables.Phrases;
                journeyData = new HashSet<T>(journeyPhrases.Cast<T>());
                currentPSData = new HashSet<T>(currentPlaySessionPhrases.Cast<T>());
            }

            // Get unfiltered data based on the builder's logic
            var dataList = builderSelectionFunction();
            int nAfterBuilder = dataList.Count;
            debugString += ("Builder: " + dataList.Count);

            // Filtering based on journey
            if (selectionParams.useJourney && !ConfigAI.forceJourneyIgnore)
            {
                dataList = dataList.FindAll(x => journeyData.Contains(x));
            }
            if (selectionParams.severity == SelectionSeverity.AllRequired) 
            {
                if (!CheckRequiredNumberReached(dataList, selectionParams, nAfterBuilder))
                {
                    UnityEngine.Debug.Log(debugString);
                    throw new System.Exception("The teacher could not find " + selectionParams.nRequired + " data instances after applying the journey logic.");
                }
            }
            debugString += (" \tJourney: " + dataList.Count);

            // Filtering based on pack-list history 
            PackListHistory sev = selectionParams.packListHistory;
            switch (sev)
            {
                case PackListHistory.NoFilter:
                    // we do not care which are picked, in this case
                    break;

                case PackListHistory.ForceAllDifferent:
                    // filter only by those that have not been found already in this pack, if possible
                    dataList = dataList.FindAll(x => !selectionParams.filteringIds.Contains(x.GetId()));
                    if (!CheckRequiredNumberReached(dataList, selectionParams, nAfterBuilder))
                    {
                        UnityEngine.Debug.Log(debugString);
                        throw new System.Exception("The teacher could not find " + selectionParams.nRequired + " data instances after applying the pack-history logic.");
                    }
                    break;

                case PackListHistory.RepeatWhenFull:
                    // reset the previous pack list if needed
                    var tmpDataList = dataList.FindAll(x => !selectionParams.filteringIds.Contains(x.GetId()));
                    if (tmpDataList.Count < selectionParams.nRequired)
                    {
                        // reset and re-pick
                        selectionParams.filteringIds.Clear();
                        dataList = dataList.FindAll(x => !selectionParams.filteringIds.Contains(x.GetId()));
                    }
                    else
                    {
                        dataList = tmpDataList;
                    }
                    break;
            }
            debugString += (" \tHistory: " + dataList.Count);

            // Weighted selection on the remaining number
            List<T> selectedList = null;
            if (selectionParams.getMaxData) selectedList = dataList;
            else selectedList = WeightedDataSelect(dataList, currentPSData, selectionParams.nRequired, table, selectionParams.severity);
            debugString += (" \tSelection: " + selectedList.Count);

            if (ConfigAI.verboseDataSelection)
            {
                UnityEngine.Debug.Log(debugString);
            }

            if (selectedList.Count == 0)
            {
                throw new System.Exception("The teacher could not find any data with the current filters. The game does not seem to be playable at the selected play session.");
            }

            // Update the filtering ids
            if (selectionParams.packListHistory != PackListHistory.NoFilter)
            {
                selectionParams.filteringIds.AddRange(selectedList.ConvertAll<string>(x => x.GetId()).ToArray());
            }

            return selectedList;
        }

        private bool CheckRequiredNumberReached<T>(List<T> dataList, SelectionParameters selectionParams, int nAfterBuilder)
        {
            return (!selectionParams.getMaxData && dataList.Count >= selectionParams.nRequired)
                || (selectionParams.getMaxData && dataList.Count >= nAfterBuilder);
        }

        private List<T> WeightedDataSelect<T>(List<T> source_data_list, HashSet<T> currentPSData, int nToSelect, DbTables table, SelectionSeverity severity) where T : IData
        {
            // Given a (filtered) list of data, select some using weights
            List<ScoreData> score_data_list = dbManager.FindScoreDataByQuery("SELECT * FROM ScoreData WHERE TableName = '" + table.ToString() + "'");

            string debugString = "-- Teacher Selection Weights";

            List<float> weights_list = new List<float>();
            foreach (var sourceData in source_data_list)
            {
                float cumulativeWeight = 0;
                debugString += "\n"+ sourceData.GetId() + " ---";

                // Get score data
                var score_data = score_data_list.Find(x => x.ElementId == sourceData.GetId());
                float currentScore = 0;
                int daysSinceLastScore = 0;
                if (score_data != null)
                {
                    var timespanFromLastScoreToNow = GenericUtilities.GetTimeSpanBetween(score_data.LastAccessTimestamp, GenericUtilities.GetTimestampForNow());
                    daysSinceLastScore = timespanFromLastScoreToNow.Days;
                    currentScore = score_data.Score;
                }
                //UnityEngine.Debug.Log("Data " + id + " score: " + currentScore + " days " + daysSinceLastScore);

                // Score Weight [0,1]: higher the lower the score [-1,1] is
                var scoreWeight = 0.5f * (1 - currentScore);
                cumulativeWeight += scoreWeight * ConfigAI.data_scoreWeight;
                debugString += " \tScore: " + scoreWeight * ConfigAI.data_scoreWeight + "(" + scoreWeight + ")";

                // RecentPlay Weight  [1,0]: higher the more in the past we saw that data
                const float dayLinerWeightDecrease = 1f / ConfigAI.daysForMaximumRecentPlayMalus;
                float weightMalus = daysSinceLastScore * dayLinerWeightDecrease;
                float recentPlayWeight = 1f - UnityEngine.Mathf.Min(1, weightMalus);
                cumulativeWeight += recentPlayWeight * ConfigAI.data_recentPlayWeight;
                debugString += " \tRecent: " + recentPlayWeight * ConfigAI.data_recentPlayWeight + "(" + recentPlayWeight + ")";

                // Current focus weight [1,0]: higher if the data is part of the current play session
                float currentPlaySessionWeight = currentPSData.Contains(sourceData) ? 1 : 0f;
                cumulativeWeight += currentPlaySessionWeight * ConfigAI.data_currentPlaySessionWeight;
                debugString += " \tCurrentPS: " + currentPlaySessionWeight * ConfigAI.data_currentPlaySessionWeight + "(" + currentPlaySessionWeight + ")";

                // If the cumulative weight goes to the negatives, we give it a fixed weight
                if (cumulativeWeight <= 0)
                {
                    cumulativeWeight = ConfigAI.data_minimumTotalWeight;
                    continue;
                }

                // Save cumulative weight
                weights_list.Add(cumulativeWeight);
                debugString += " TOTw: " + cumulativeWeight;
            }

            if (ConfigAI.verboseDataSelection)
            {
                UnityEngine.Debug.Log(debugString);
            }

            // Select data from the list
            List<T> selected_data_list = new List<T>();
            if (source_data_list.Count > 0)
            {
                int nToSelectFromCurrentList = 0;
                List<T> chosenData = null;
                switch (severity)
                {
                    case SelectionSeverity.AsManyAsPossible:
                    case SelectionSeverity.AllRequired:
                        nToSelectFromCurrentList = UnityEngine.Mathf.Min(source_data_list.Count, nToSelect);
                        chosenData = RandomHelper.RouletteSelectNonRepeating(source_data_list, weights_list, nToSelectFromCurrentList);
                        selected_data_list.AddRange(chosenData);
                        break;
                    case SelectionSeverity.MayRepeatIfNotEnough:
                        int nRemainingToSelect = nToSelect;
                        while (nRemainingToSelect > 0)
                        {
                            var listCopy = new List<T>(source_data_list);
                            nToSelectFromCurrentList = UnityEngine.Mathf.Min(source_data_list.Count, nRemainingToSelect);
                            chosenData = RandomHelper.RouletteSelectNonRepeating(listCopy, weights_list, nToSelectFromCurrentList);
                            selected_data_list.AddRange(chosenData);
                            nRemainingToSelect -= nToSelectFromCurrentList;
                        }
                        break;
                }
            }
            return selected_data_list;
        }

        #endregion

        // @todo: move these to JourneyHelper instead?
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