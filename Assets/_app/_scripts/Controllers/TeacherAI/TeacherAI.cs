using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EA4S.Db;
using EA4S.Teacher;

namespace EA4S
{
    public class TeacherAI
    {
        public static TeacherAI I;

        // References
        private DatabaseManager dbManager;
        private PlayerProfile playerProfile;

        // Helpers
        public WordHelper wordHelper;
        public JourneyHelper journeyHelper;

        // Selection engines
        MiniGameSelectionAI minigameSelectionAI;
        WordSelectionAI wordSelectionAI;

        // State
        private List<MiniGameData> currentPlaySessionMiniGames = new List<MiniGameData>();

        #region Setup

        public TeacherAI(DatabaseManager _dbManager, PlayerProfile _playerProfile)
        {
            I = this;
            this.dbManager = _dbManager;
            this.playerProfile = _playerProfile;

            this.wordHelper = new WordHelper(_dbManager, this);
            this.journeyHelper = new JourneyHelper(_dbManager, this);

            this.minigameSelectionAI = new MiniGameSelectionAI(dbManager, playerProfile);
            this.wordSelectionAI = new WordSelectionAI(dbManager, playerProfile, this);
        }

        private void ResetPlaySession()
        {
            this.currentPlaySessionMiniGames.Clear();

            this.minigameSelectionAI.InitialiseNewPlaySession();
            this.wordSelectionAI.InitialiseNewPlaySession();
        }

        #endregion

        #region Interface - MapManager

        // TODO: call this from the MapManager
        public List<Db.MiniGameData> InitialiseCurrentPlaySession(int nMinigamesToSelect)
        {
            ResetPlaySession();
            this.currentPlaySessionMiniGames = this.SelectMiniGamesForCurrentPlaySession(nMinigamesToSelect);
            return currentPlaySessionMiniGames;
        }

        #endregion

        #region Interface - MiniGame Getters

        // TODO: call this to get all minigame data for the current play session
        public List<MiniGameData> CurrentPlaySessionMiniGames
        {
            get { 
                return currentPlaySessionMiniGames;
            }
        }

        // TODO: call this to get the current single minigame data
        public MiniGameData CurrentMiniGame
        {
            get
            {
                return currentPlaySessionMiniGames.ElementAt(playerProfile.CurrentMiniGameInPlaySession);
            }
        }

        // DEPRECATED (should now use CurrentPlaySessionMiniGames)
        public List<MiniGameData> MiniGamesInPlaySession
        {
            get
            {
                return currentPlaySessionMiniGames;
            }
        }

        // DEPRECATED (use CurrentMiniGameData instead)
        public MiniGameData GetCurrentMiniGameData()
        {
            return CurrentMiniGame;
        }

        // DEPRECATED (should now be performed through MiniGame Selection)
        public List<MiniGameData> GimmeGoodMinigames()
        {
            return dbManager.GetActiveMinigames();
        }


        bool FAKE_SELECTION = true; // @todo: this must be removed when minigames are correct!

        // DEPRECATED (should now be performed through MiniGame Selection)
        public List<MiniGameData> GetMiniGamesForCurrentPlaySession()
        {
            int number = 2; // TODO: should be injected somehow!
            InitialiseCurrentPlaySession(number);

            if (FAKE_SELECTION)
            {
                this.currentPlaySessionMiniGames.Clear();
                this.currentPlaySessionMiniGames.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.FastCrowd_alphabet));
                this.currentPlaySessionMiniGames.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.DancingDots));
            } 

            return CurrentPlaySessionMiniGames;
        }

        #endregion


        #region Interface - Letters / Words

        // DEPRECATED (could safely remove this! it is not used anymore!)
        public WordData GimmeAGoodWord(WordDataCategory category)
        {
            return dbManager.FindWordData(x => x.Category == WordDataCategory.BodyPart).RandomSelectOne();
        }

        // DEPRECATED (should be in the new "MiniGameLauncher SYSTEM")
        public LL_LetterData GimmeARandomLetter()
        {
            var data = this.SelectRandomLetter();
            return BuildLetterData_LL(data);
        }

        // DEPRECATED (should be in the new "MiniGameLauncher SYSTEM")
        /// <summary>
        /// Return WordData from a list of available data.
        /// </summary>
        /// <returns></returns>
        public LL_WordData GimmeAGoodWordData()
        {
            // init vocabulary
            var availableVocabulary = BuildWordData_LL_Set(dbManager.FindWordData(x => x.Category == WordDataCategory.BodyPart));

            List<LL_WordData> returnList = new List<LL_WordData>();
            if (AppManager.Instance.ActualGameplayWordAlreadyUsed.Count >= availableVocabulary.Count)
            { 
                // if already used all available words... restart.
                AppManager.Instance.ActualGameplayWordAlreadyUsed = new List<LL_WordData>();
            }

            foreach (LL_WordData w in availableVocabulary)
            {
                if (!AppManager.Instance.ActualGameplayWordAlreadyUsed.Contains(w))
                {
                    returnList.Add(w); // Only added if not already used
                }
            }

            LL_WordData returnWord = returnList.GetRandom();
            // Debug.Log("Word: " + returnWord.Key);
            AppManager.Instance.ActualGameplayWordAlreadyUsed.Add(returnWord);
            return returnWord;
        }


        // DEPRECATED (should be in the new "MiniGameLauncher SYSTEM")
        /// <summary>
        /// TODO
        /// Gimmes n similar letters to the current.
        /// </summary>
        /// <returns>The similar letters.</returns>
        /// <param name="letter">Letter.</param>
        /// <param name="howMany">How many.</param>
        public List<Db.LetterData> GimmeSimilarLetters(Db.LetterData letter, int howMany)
        {
            List<Db.LetterData> returnList = new List<Db.LetterData>();
            returnList.Add(dbManager.GetAllLetterData().RandomSelectOne());
            return returnList;
        }

        #endregion

        #region WordData -> LL_WordData helpers

        // HELPER (could be in the new "MiniGameLauncher SYSTEM")
        private LL_LetterData BuildLetterData_LL(LetterData data)
        {
            return new LL_LetterData(data.GetId());
        }

        // HELPER (could be in the new "MiniGameLauncher SYSTEM")
        private LL_WordData BuildWordData_LL(WordData data)
        {
            return new LL_WordData(data.GetId(), data);
        }

        // HELPER (should be in the new "MiniGameLauncher SYSTEM")
        private List<LL_WordData> BuildWordData_LL_Set(List<WordData> data_list)
        {
            List<LL_WordData> returnList = new List<LL_WordData>();
            foreach (var data in data_list)
            {
                returnList.Add(BuildWordData_LL(data));
            }
            return returnList;
        }

        #endregion


        // HELPER (move to JourneyHelper?)
        private string JourneyPositionToPlaySessionId(JourneyPosition journeyPosition)
        {
            return journeyPosition.Stage + "." + journeyPosition.LearningBlock + "." + journeyPosition.PlaySession;
        }
      
        #region MiniGame Selection queries

        private List<Db.MiniGameData> SelectMiniGamesForCurrentPlaySession(int nMinigamesToSelect)
        {
            var currentPlaySessionId = JourneyPositionToPlaySessionId(this.playerProfile.CurrentJourneyPosition);
            return this.SelectMiniGamesForPlaySession(currentPlaySessionId, nMinigamesToSelect);
        }

        public List<Db.MiniGameData> SelectMiniGamesForPlaySession(string playSessionId, int numberToSelect)
        {
            return this.minigameSelectionAI.PerformSelection(playSessionId, numberToSelect);
        }

        #endregion

        #region Letter/Word Selection queries

        public LetterData SelectRandomLetter()
        {
            return dbManager.GetAllLetterData().RandomSelectOne();
        }

        public List<Db.WordData> SelectWordsForPlaySession(string playSessionId, int numberToSelect)
        {
            return this.wordSelectionAI.PerformSelection(playSessionId, numberToSelect);
        }

        public List<LetterData> GetLettersInWord(string wordId)
        {
            return wordHelper.GetLettersInWord(wordId);
        }

        public List<LetterData> SelectLettersInWord(int nToSelect, string wordId)
        {
            return wordHelper.GetLettersInWord(wordId).RandomSelect(2);
        }

        public List<WordData> SelectWordsWithLetters(int nToSelect, params string[] letters)
        {
            return wordHelper.GetWordsWithLetters(letters).RandomSelect(2);
        }

        #endregion

        #region Score Log queries

        public List<float> GetLatestScoresForMiniGame(MiniGameCode minigameCode, int nLastDays)
        {
            int fromTimestamp = GenericUtilities.GetRelativeTimestampFromNow(-nLastDays);
            string query = string.Format("SELECT * FROM LogPlayData WHERE MiniGame = '{0}' AND Timestamp < {1}",
                (int)minigameCode, fromTimestamp);
            List<LogPlayData> list = dbManager.FindLogPlayDataByQuery(query);
            List<float> scores = list.ConvertAll(x => x.Score);
            return scores;
        }

        public List<ScoreData> GetCurrentScoreForAllPlaySessions()
        {
            string query = string.Format("SELECT * FROM ScoreData WHERE TableName = 'PlaySessions' ORDER BY ElementId ");
            List<ScoreData> list = dbManager.FindScoreDataByQuery(query);
            return list;
        }

        public List<ScoreData> GetCurrentScoreForPlaySessionsOfStage(int stage)
        {
            // First, get all data given a stage
            List<PlaySessionData> eligiblePlaySessionData_list = this.dbManager.FindPlaySessionData(x => x.Stage == stage);
            List<string> eligiblePlaySessionData_id_list = eligiblePlaySessionData_list.ConvertAll(x => x.Id);

            // Then, get all scores
            string query = string.Format("SELECT * FROM ScoreData WHERE TableName = 'PlaySessions'");
            List<ScoreData> all_score_list = dbManager.FindScoreDataByQuery(query);

            // At last, filter by the given stage
            List<ScoreData> filtered_score_list = all_score_list.FindAll(x => eligiblePlaySessionData_id_list.Contains(x.ElementId));
            return filtered_score_list;
        }

        public List<ScoreData> GetCurrentScoreForPlaySessionsOfStageAndLearningBlock(int stage, int learningBlock)
        {
            // First, get all data given a stage
            List<PlaySessionData> eligiblePlaySessionData_list = this.dbManager.FindPlaySessionData(x => x.Stage == stage && x.LearningBlock == learningBlock); // TODO: make this readily available!
            List<string> eligiblePlaySessionData_id_list = eligiblePlaySessionData_list.ConvertAll(x => x.Id);

            // Then, get all scores
            string query = string.Format("SELECT * FROM ScoreData WHERE TableName = 'PlaySessions'");
            List<ScoreData> all_score_list = dbManager.FindScoreDataByQuery(query);

            // At last, filter
            List<ScoreData> filtered_score_list = all_score_list.FindAll(x => eligiblePlaySessionData_id_list.Contains(x.ElementId));
            return filtered_score_list;
        }
        

        public List<ScoreData> GetCurrentScoreForLearningBlocksOfStage(int stage)
        {
            // First, get all data given a stage
            List<LearningBlockData> eligibleLearningBlockData_list = this.dbManager.FindLearningBlockData(x => x.Stage == stage);
            List<string> eligibleLearningBlockData_id_list = eligibleLearningBlockData_list.ConvertAll(x => x.Id);

            // Then, get all scores
            string query = string.Format("SELECT * FROM ScoreData WHERE TableName = 'LearningBlock'");
            List<ScoreData> all_score_list = dbManager.FindScoreDataByQuery(query);

            // At last, filter by the given stage
            List<ScoreData> filtered_score_list = all_score_list.FindAll(x => eligibleLearningBlockData_id_list.Contains(x.ElementId));
            return filtered_score_list;
        }

        #endregion

        #region Assessment Log queries

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

        #region Journeymap Log queries

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

        #region Mood Log queries

        public List<LogMoodData> GetLastMoodData(int number)
        {
            string query = string.Format("SELECT * FROM LogMoodData ORDER BY Timestamp LIMIT {0}", number);
            List<LogMoodData> list = dbManager.FindLogMoodDataByQuery(query);
            return list;
        }

        #endregion

    }
}
