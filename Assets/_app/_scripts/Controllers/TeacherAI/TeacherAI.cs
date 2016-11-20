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
        public LogIntelligence logIntelligence;

        // Helpers
        public WordHelper wordHelper;
        public JourneyHelper journeyHelper;
        public ScoreHelper scoreHelper;

        // Selection engines
        MiniGameSelectionAI minigameSelectionAI;
        public WordSelectionAI wordAI;
        DifficultySelectionAI difficultySelectionAI;

        // State
        private List<MiniGameData> currentPlaySessionMiniGames = new List<MiniGameData>();

        #region Setup

        public TeacherAI(DatabaseManager _dbManager, PlayerProfile _playerProfile)
        {
            I = this;
            dbManager = _dbManager;
            playerProfile = _playerProfile;

            logIntelligence = new LogIntelligence(_dbManager);

            wordHelper = new WordHelper(_dbManager, this);
            journeyHelper = new JourneyHelper(_dbManager, this);
            scoreHelper = new ScoreHelper(_dbManager);

            minigameSelectionAI = new MiniGameSelectionAI(dbManager, playerProfile);
            wordAI = new WordSelectionAI(dbManager, playerProfile, this, wordHelper);
            difficultySelectionAI = new DifficultySelectionAI(dbManager, playerProfile, this);
        }

        private void ResetPlaySession()
        {
            var currentPlaySessionId = JourneyPositionToPlaySessionId(this.playerProfile.CurrentJourneyPosition);

            this.currentPlaySessionMiniGames.Clear();

            this.minigameSelectionAI.InitialiseNewPlaySession();
            this.wordAI.InitialiseNewPlaySession(currentPlaySessionId);
        }

        #endregion

        #region MiniGames

        public List<MiniGameData> InitialiseCurrentPlaySession()
        {
            return InitialiseCurrentPlaySession(ConfigAI.numberOfMinigamesPerPlaySession);
        }

        private List<MiniGameData> InitialiseCurrentPlaySession(int nMinigamesToSelect)
        {
            ResetPlaySession();
            this.currentPlaySessionMiniGames = SelectMiniGamesForCurrentPlaySession(nMinigamesToSelect);
            //this.currentUsableWords = SelectWordsForPlaySession();
            return currentPlaySessionMiniGames;
        }

        public List<MiniGameData> CurrentPlaySessionMiniGames {
            get {
                return currentPlaySessionMiniGames;
            }
        }

        public MiniGameData CurrentMiniGame {
            get {
                return currentPlaySessionMiniGames.ElementAt(playerProfile.CurrentMiniGameInPlaySession);
            }
        }

        private List<MiniGameData> SelectMiniGamesForCurrentPlaySession(int nMinigamesToSelect)
        {
            var currentPlaySessionId = JourneyPositionToPlaySessionId(this.playerProfile.CurrentJourneyPosition);
            return SelectMiniGamesForPlaySession(currentPlaySessionId, nMinigamesToSelect);
        }

        public List<MiniGameData> SelectMiniGamesForPlaySession(string playSessionId, int numberToSelect)
        {
            return minigameSelectionAI.PerformSelection(playSessionId, numberToSelect);
        }

        #endregion

        #region Difficulty

        public float GetCurrentDifficulty(MiniGameCode miniGameCode)
        {
            return difficultySelectionAI.SelectDifficulty(miniGameCode);
        }

        #endregion

        // HELPER (move to JourneyHelper?)
        public string JourneyPositionToPlaySessionId(JourneyPosition journeyPosition)
        {
            return journeyPosition.Stage + "." + journeyPosition.LearningBlock + "." + journeyPosition.PlaySession;
        }

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

        public List<LL_LetterData> GetAllTestLetterDataLL()
        {
            List<LL_LetterData> list = new List<LL_LetterData>();
            foreach (var letterData in this.wordHelper.GetAllLetters())
                list.Add(BuildLetterData_LL(letterData));
            return list;
        }

        public LL_LetterData GetRandomTestLetterLL()
        {
            if (giveWarningOnFake) {
                Debug.LogWarning("You are using fake data for testing. Make sure to test with real data too.");
                giveWarningOnFake = false;
            }

            var data = this.wordHelper.GetAllLetters().RandomSelectOne();
            return BuildLetterData_LL(data);
        }

        public LL_WordData GetRandomTestWordDataLL()
        {
            if (giveWarningOnFake) {
                Debug.LogWarning("You are using fake data for testing. Make sure to test with real data too.");
                giveWarningOnFake = false;
            }

            var data = this.wordHelper.GetWordsByCategory(WordDataCategory.BodyPart).RandomSelectOne();
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
