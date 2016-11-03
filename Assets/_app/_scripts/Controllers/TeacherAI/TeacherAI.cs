using UnityEngine;
using System.Collections;
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


        // TO REMOVE
        string[] bodyPartsWords;
        public List<MiniGameData> MiniGamesInPlaySession;
        List<LL_WordData> availableVocabulary = new List<LL_WordData>();

        public TeacherAI(DatabaseManager _dbManager, PlayerProfile _playerProfile)
        {
            I = this;
            this.dbManager = _dbManager;
            this.playerProfile = _playerProfile;

            this.wordHelper =  new WordHelper(_dbManager, this);
            this.journeyHelper = new JourneyHelper(_dbManager, this);

            this.minigameSelectionAI = new MiniGameSelectionAI(dbManager, playerProfile);
            this.wordSelectionAI = new WordSelectionAI(dbManager, playerProfile, this);

            // Debug.Log("TeacherAI exists");

            // TO REMOVE
            MiniGamesInPlaySession = GetMiniGamesForCurrentPlaySession();
            bodyPartsWords = new[]
            {
                "mouth", "tooth", "eye", "nose", "hand", "foot", "belly", "hair", "face", "tongue", "chest", "back"
            };
        }

        public void InitialiseNewPlaySession()
        {
            // @todo: make sure this is called when a new play session starts
            this.minigameSelectionAI.InitialiseNewPlaySession();
            this.wordSelectionAI.InitialiseNewPlaySession();
        }

        #region Stefano's queries
        // TO REMOVE!!!!!!
        // TO REMOVE!!!!!!
        // TO REMOVE!!!!!!

        public List<Db.MiniGameData> GimmeGoodMinigames()
        {
            return dbManager.GetActiveMinigames();
        }

        public Db.WordData GimmeAGoodWord()
        {
            int index = Random.Range(0, bodyPartsWords.Length - 1);
            return dbManager.GetWordDataById(bodyPartsWords[index]);
        }

        /// <summary>
        /// Return WordData from a list of available data.
        /// </summary>
        /// <returns></returns>
        public LL_WordData GimmeAGoodWordData()
        {
            // init vocabulary
            if (availableVocabulary.Count == 0)
                availableVocabulary = getVocabularySubset(bodyPartsWords);

            List<LL_WordData> returnList = new List<LL_WordData>();
            if (AppManager.Instance.ActualGameplayWordAlreadyUsed.Count >= availableVocabulary.Count)
                // if already used all available words... restart.
                AppManager.Instance.ActualGameplayWordAlreadyUsed = new List<LL_WordData>();
            foreach (LL_WordData w in availableVocabulary) {
                if (!AppManager.Instance.ActualGameplayWordAlreadyUsed.Contains(w)) {
                    returnList.Add(w); // Only added if not already used
                }
            }

            LL_WordData returnWord = returnList.GetRandom();
            // Debug.Log("Word: " + returnWord.Key);
            AppManager.Instance.ActualGameplayWordAlreadyUsed.Add(returnWord);
            return returnWord;
        }

        public LL_LetterData GimmeARandomLetter()
        {
            var RandomLetterData = SelectOne(dbManager.GetAllLetterData()); // dbManager.GetLetterDataByRandom();
            return new LL_LetterData(RandomLetterData.GetId());
        }

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
            returnList.Add(SelectOne(dbManager.GetAllLetterData()));
            return returnList;
        }

        List<LL_WordData> getVocabularySubset(string[] _goodWords)
        {
            List<LL_WordData> returnList = new List<LL_WordData>();
            foreach (string wordKey in _goodWords) {
                returnList.Add(LL_WordData.GetWordDataByKeyRow(wordKey));
            }
            return returnList;
        }

        public List<MiniGameData> GetMiniGamesForCurrentPlaySession()
        {
            var miniGamesInPlaySession = new List<MiniGameData>();
            switch (this.playerProfile.CurrentJourneyPosition.PlaySession) {
                case 1:
                    //_MiniGamesInPlaySession = this.dbManager.GetPlaySessionDataById("1.1.1")
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.FastCrowd_alphabet));
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.DancingDots));
                    break;
                case 2:
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Egg));
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Balloons_letter));
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Maze));
                    break;
                case 3:
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.FastCrowd_spelling));
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.ThrowBalls_letters));
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Tobogan_letters));
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.MakeFriends));
                    break;
                case 4:
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.DancingDots));
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.FastCrowd_words));
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Tobogan_words));
                    miniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Balloons_spelling));
                    break;
            }
            return miniGamesInPlaySession;
        }

        public MiniGameData GetCurrentMiniGameData()
        {
            MiniGamesInPlaySession = GetMiniGamesForCurrentPlaySession();
            return MiniGamesInPlaySession.ElementAt(playerProfile.CurrentMiniGameInPlaySession);
        }

        #endregion

        #region MiniGame Selection queries

        public List<Db.MiniGameData> SelectMiniGamesForPlaySession(string playSessionId, int numberToSelect)
        {
            return this.minigameSelectionAI.PerformSelection(playSessionId, numberToSelect);
        }

        #endregion

        #region Letter/Word Selection queries

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
            return Select(2, wordHelper.GetLettersInWord(wordId));
        }

        public List<WordData> SelectWordsWithLetters(int nToSelect, params string[] letters)
        {
            return Select(2, wordHelper.GetWordsWithLetters(letters));
        }


        // @TODO: these could be set as list extensions
        public List<T> Select<T>(int numberToSelect, List<T> all_list)
        {
            return RandomHelper.RouletteSelectNonRepeating<T>(all_list, numberToSelect);
        }

        public T SelectOne<T>(List<T> all_list)
        {
            return RandomHelper.RouletteSelectNonRepeating<T>(all_list, 1)[0];
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
