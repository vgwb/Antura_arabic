using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EA4S.Core;
using EA4S.Database;
using EA4S.Helpers;
using EA4S.MinigamesAPI;
using EA4S.Profile;

namespace EA4S.Teacher
{
    /// <summary>
    /// Handles logic that represent the Teacher's expert system:
    /// - selects minigames according to a given progression flow
    /// - selects question packs according to the given profression flow
    /// - selects minigame difficulty according to the player's status
    /// </summary>
    public class TeacherAI
    {
        public static TeacherAI I;

        // References
        private DatabaseManager dbManager;
        private PlayerProfile playerProfile;

        // Inner engines
        public LogAI logAI;
        public WordSelectionAI wordAI;
        MiniGameSelectionAI minigameSelectionAI;
        DifficultySelectionAI difficultySelectionAI;

        // Helpers
        // refactor: these helpers should be separated from the TeacherAI.
        public WordHelper wordHelper;
        public JourneyHelper journeyHelper;
        public ScoreHelper scoreHelper;
        
        #region Setup

        public TeacherAI(DatabaseManager _dbManager, PlayerProfile _playerProfile)
        {
            I = this;
            dbManager = _dbManager;
            playerProfile = _playerProfile;

            wordHelper = new WordHelper(_dbManager);
            journeyHelper = new JourneyHelper(_dbManager, this);
            scoreHelper = new ScoreHelper(_dbManager);

            logAI = new LogAI(_dbManager);
            minigameSelectionAI = new MiniGameSelectionAI(dbManager, playerProfile);
            wordAI = new WordSelectionAI(dbManager, playerProfile, this, wordHelper);
            difficultySelectionAI = new DifficultySelectionAI(dbManager, playerProfile);
        }

        private void ResetPlaySession()
        {
            var currentPlaySessionId = journeyHelper.JourneyPositionToPlaySessionId(playerProfile.CurrentJourneyPosition);
            minigameSelectionAI.InitialiseNewPlaySession();
            wordAI.LoadCurrentPlaySessionData(currentPlaySessionId);
        }

        #endregion

        #region MiniGames

        public void InitialiseNewPlaySession()
        {
            ResetPlaySession();
        }

        public List<MiniGameData> SelectMiniGames()
        {
            return SelectMiniGames(ConfigAI.numberOfMinigamesPerPlaySession);
        }

        private List<MiniGameData> SelectMiniGames(int nMinigamesToSelect)
        {
            List<MiniGameData> newPlaySessionMiniGames = SelectMiniGamesForCurrentPlaySession(nMinigamesToSelect);

            if (ConfigAI.verboseTeacher)
            {
                var debugString = "";
                debugString += "--------- TEACHER: MiniGames selected ---------";
                foreach (var minigame in newPlaySessionMiniGames)
                {
                    debugString += "\n" + minigame.Code;
                }
                Debug.Log(debugString);
            }

            return newPlaySessionMiniGames;
        }

        private List<MiniGameData> SelectMiniGamesForCurrentPlaySession(int nMinigamesToSelect)
        {
            var currentPlaySessionId = journeyHelper.JourneyPositionToPlaySessionId(playerProfile.CurrentJourneyPosition);
            return SelectMiniGamesForPlaySession(currentPlaySessionId, nMinigamesToSelect);
        }

        public List<MiniGameData> SelectMiniGamesForPlaySession(string playSessionId, int numberToSelect)
        {
            return minigameSelectionAI.PerformSelection(playSessionId, numberToSelect);
        }

        // refactor: this is not used for now, could probably be used by the DebugManager
        public bool CanMiniGameBePlayedAtPlaySession(string playSessionId, MiniGameCode code)
        {
            if (dbManager.HasPlaySessionDataById(playSessionId))
            {
                var psData = dbManager.GetPlaySessionDataById(playSessionId);
                foreach (var minigameInPlaySession in psData.Minigames)
                    if (minigameInPlaySession.MiniGameCode == code)
                        return true;
            }
            return false;
        }

        #endregion

        #region Difficulty

        public float GetCurrentDifficulty(MiniGameCode miniGameCode)
        {
            return difficultySelectionAI.SelectDifficulty(miniGameCode);
        }

        #endregion

        #region Learning Blocks

        // refactor: Not used. 
        public float GetLearningBlockScore(LearningBlockData lb)
        {
            var allScores = scoreHelper.GetCurrentScoreForPlaySessionsOfLearningBlock(lb.Stage, lb.LearningBlock);
            return scoreHelper.GetAverageScore(allScores);
        }

        #endregion

        #region Assessment

        // refactor: Only for tests purpose. Remove from the teacher.
        public List<LetterData> GetFailedAssessmentLetters(MiniGameCode assessmentCode) // also play session
        {
            // @note: this code shows how to work on the dynamic and static db together
            string query =
                string.Format(
                    "SELECT * FROM LogLearnData WHERE TableName = 'LetterData' AND Score < 0 and MiniGame = {0}",
                    (int)assessmentCode);
            List<LogLearnData> logLearnData_list = dbManager.FindLogLearnDataByQuery(query);
            List<string> letter_ids_list = logLearnData_list.ConvertAll(x => x.ElementId);
            List<LetterData> letters = dbManager.FindLetterData(x => letter_ids_list.Contains(x.Id));
            return letters;
        }

        // refactor: Only for tests purpose. Remove from the teacher.
        public List<WordData> GetFailedAssessmentWords(MiniGameCode assessmentCode)
        {
            string query =
                string.Format(
                    "SELECT * FROM LogLearnData WHERE TableName = 'WordData' AND Score < 0 and MiniGame = {0}",
                    (int)assessmentCode);
            List<LogLearnData> logLearnData_list = dbManager.FindLogLearnDataByQuery(query);
            List<string> words_ids_list = logLearnData_list.ConvertAll(x => x.ElementId);
            List<WordData> words = dbManager.FindWordData(x => words_ids_list.Contains(x.Id));
            return words;
        }

        #endregion

        #region Journeymap

        // refactor: Only for tests purpose. Remove from the teacher.
        public List<LogPlayData> GetScoreHistoryForCurrentJourneyPosition()
        {
            // @note: shows how to work with playerprofile as well as the database
            JourneyPosition currentJourneyPosition = playerProfile.CurrentJourneyPosition;
            string query = string.Format("SELECT * FROM LogPlayData WHERE PlayEvent = {0} AND PlaySession = '{1}'",
                (int)PlayEvent.GameFinished, currentJourneyPosition.ToString());
            List<LogPlayData> list = dbManager.FindLogPlayDataByQuery(query);
            return list;
        }

        #endregion

        #region Mood

        // refactor: Refactor access to the data through an AnalyticsManager, instead of passing through the TeacherAI.
        public List<LogMoodData> GetLastMoodData(int number)
        {
            string query = string.Format("SELECT * FROM LogMoodData ORDER BY Timestamp LIMIT {0}", number);
            List<LogMoodData> list = dbManager.FindLogMoodDataByQuery(query);
            return list;
        }

        #endregion


        #region Fake data for question providers

        // refactor: Refactor access to test data throught a TestDataManager, instead of passing through the TeacherAI.
        private static bool giveWarningOnFake = false;

        public List<LL_LetterData> GetAllTestLetterDataLL(LetterFilters filters = null, bool useMaxJourneyData = false)
        {
            if (filters == null) filters = new LetterFilters();

            if (useMaxJourneyData)
            {
               wordAI.LoadCurrentPlaySessionData(AppManager.I.Player.MaxJourneyPosition.ToString());
            }

            var availableLetters = wordAI.SelectData(
              () => wordHelper.GetAllLetters(filters),
                new SelectionParameters(SelectionSeverity.AsManyAsPossible, getMaxData: true, useJourney: useMaxJourneyData)
              );

            List<LL_LetterData> list = new List<LL_LetterData>();
            foreach (var letterData in availableLetters)
                list.Add(BuildLetterData_LL(letterData));

            /*if (ConfigAI.verboseTeacher)
            {
                Debug.Log("All test letter data requested to teacher.");
            }*/

            return list;
        }

        public LL_LetterData GetRandomTestLetterLL(LetterFilters filters = null, bool useMaxJourneyData = false)
        {
            if (filters == null) filters = new LetterFilters();

            if (useMaxJourneyData)
            {
               wordAI.LoadCurrentPlaySessionData(AppManager.I.Player.MaxJourneyPosition.ToString());
            }

            var availableLetters = wordAI.SelectData(
              () => wordHelper.GetAllLetters(filters),
                new SelectionParameters(SelectionSeverity.AsManyAsPossible, getMaxData: true, useJourney: useMaxJourneyData)
              );

            if (giveWarningOnFake)
            {
                Debug.LogWarning("You are using fake data for testing. Make sure to test with real data too.");
                giveWarningOnFake = false;
            }

            var data = availableLetters.RandomSelectOne();

            /*if (ConfigAI.verboseTeacher)
            {
                Debug.Log("Random test Letter requested to teacher: " + data.ToString());
            }*/

            return BuildLetterData_LL(data);
        }

        public LL_WordData GetRandomTestWordDataLL(WordFilters filters = null, bool useMaxJourneyData = false)
        {
            if (filters == null) filters = new WordFilters();

            if (useMaxJourneyData)
            {
                wordAI.LoadCurrentPlaySessionData(AppManager.I.Player.MaxJourneyPosition.ToString());
            }

            if (giveWarningOnFake)
            {
                Debug.LogWarning("You are using fake data for testing. Make sure to test with real data too.");
                giveWarningOnFake = false;
            }

            var availableWords = wordAI.SelectData(
              () => wordHelper.GetWordsByCategory(WordDataCategory.Animal, filters),
                new SelectionParameters(SelectionSeverity.AsManyAsPossible, getMaxData: true, useJourney: useMaxJourneyData)
              );

            var data = availableWords.RandomSelectOne();

            /*if (ConfigAI.verboseTeacher)
            {
                Debug.Log("Random test Word requested to teacher: " + data.ToString());
            }*/

            return BuildWordData_LL(data);
        }

        private LL_LetterData BuildLetterData_LL(LetterData data)
        {
            return new LL_LetterData(data.GetId());
        }

        private List<ILivingLetterData> BuildLetterData_LL_Set(List<LetterData> data_list)
        {
            return data_list.ConvertAll<ILivingLetterData>(x => BuildLetterData_LL(x));
        }

        private LL_WordData BuildWordData_LL(WordData data)
        {
            return new LL_WordData(data.GetId(), data);
        }

        private List<ILivingLetterData> BuildWordData_LL_Set(List<WordData> data_list)
        {
            return data_list.ConvertAll<ILivingLetterData>(x => BuildWordData_LL(x));
        }

        #endregion


    }
}
