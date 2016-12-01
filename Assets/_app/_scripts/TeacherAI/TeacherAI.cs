using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EA4S.Db;
using EA4S.Teacher;

namespace EA4S
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
        public WordHelper wordHelper;
        public JourneyHelper journeyHelper;
        public ScoreHelper scoreHelper;

        // State
        private List<MiniGameData> currentPlaySessionMiniGames = new List<MiniGameData>();

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

            currentPlaySessionMiniGames.Clear();

            minigameSelectionAI.InitialiseNewPlaySession();
            wordAI.InitialiseNewPlaySession(currentPlaySessionId);
        }

        #endregion

        #region MiniGames

        public List<MiniGameData> InitialiseCurrentPlaySession(bool chooseMiniGames = true)
        {
            return InitialiseCurrentPlaySession(ConfigAI.numberOfMinigamesPerPlaySession, chooseMiniGames);
        }

        private List<MiniGameData> InitialiseCurrentPlaySession(int nMinigamesToSelect, bool chooseMiniGames = true)
        {
            ResetPlaySession();

            if (chooseMiniGames)
            {
                currentPlaySessionMiniGames = SelectMiniGamesForCurrentPlaySession(nMinigamesToSelect);

                if (ConfigAI.verboseTeacher)
                {
                    var debugString = "";
                    debugString += "--------- TEACHER: MiniGames selected ---------";
                    foreach (var minigame in currentPlaySessionMiniGames)
                    {
                        debugString += "\n" + minigame.Code;
                    }
                    Debug.Log(debugString);
                }
            }

            return currentPlaySessionMiniGames;
        }

        public List<MiniGameData> CurrentPlaySessionMiniGames {
            get {
                return currentPlaySessionMiniGames;
            }
        }

        public MiniGameData CurrentMiniGame {
            get {
                return
                    playerProfile.CurrentMiniGameInPlaySession < currentPlaySessionMiniGames.Count
                        ? currentPlaySessionMiniGames.ElementAt(playerProfile.CurrentMiniGameInPlaySession)
                        : null
                    ;
            }
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

        public float GetLearningBlockScore(LearningBlockData lb)
        {
            var allScores = scoreHelper.GetCurrentScoreForPlaySessionsOfLearningBlock(lb.Stage, lb.LearningBlock);
            return scoreHelper.GetAverageScore(allScores);
        }

        #endregion

        #region Assessment

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

        public List<LogMoodData> GetLastMoodData(int number)
        {
            string query = string.Format("SELECT * FROM LogMoodData ORDER BY Timestamp LIMIT {0}", number);
            List<LogMoodData> list = dbManager.FindLogMoodDataByQuery(query);
            return list;
        }

        #endregion


        #region Fake data for question providers

        private static bool giveWarningOnFake = false;

        public List<LL_LetterData> GetAllTestLetterDataLL(LetterFilters filters = null)
        {
            if (filters == null) filters = new LetterFilters();

            List<LL_LetterData> list = new List<LL_LetterData>();
            foreach (var letterData in this.wordHelper.GetAllLetters(filters))
                list.Add(BuildLetterData_LL(letterData));

            /*if (ConfigAI.verboseTeacher)
            {
                Debug.Log("All test letter data requested to teacher.");
            }*/

            return list;
        }

        public LL_LetterData GetRandomTestLetterLL(LetterFilters filters = null)
        {
            if (filters == null) filters = new LetterFilters();

            if (giveWarningOnFake) {
                Debug.LogWarning("You are using fake data for testing. Make sure to test with real data too.");
                giveWarningOnFake = false;
            }

            var data = this.wordHelper.GetAllLetters(filters).RandomSelectOne();

            /*if (ConfigAI.verboseTeacher)
            {
                Debug.Log("Random test Letter requested to teacher: " + data.ToString());
            }*/

            return BuildLetterData_LL(data);
        }

        public LL_WordData GetRandomTestWordDataLL(WordFilters filters = null)
        {
            if (filters == null) filters = new WordFilters();

            if (giveWarningOnFake) {
                Debug.LogWarning("You are using fake data for testing. Make sure to test with real data too.");
                giveWarningOnFake = false;
            }

            var data = this.wordHelper.GetWordsByCategory(WordDataCategory.Animal, filters).RandomSelectOne();

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
