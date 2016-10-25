using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EA4S.Db;

namespace EA4S
{
    public class TeacherAI
    {
        public static TeacherAI I;

        private DatabaseManager dbManager;
        private PlayerProfile playerProfile;
        string[] bodyPartsWords;

        public List<MiniGameData> MiniGamesInPlaySession;

        List<LL_WordData> availableVocabulary = new List<LL_WordData>();

        MiniGameSelectionAI minigameSelectionAI;

        public TeacherAI(DatabaseManager _dbManager, PlayerProfile _playerProfile)
        {
            I = this;
            this.dbManager = _dbManager;
            this.playerProfile = _playerProfile;
        
            this.minigameSelectionAI = new MiniGameSelectionAI(dbManager, playerProfile);

            MiniGamesInPlaySession = GetMiniGamesForCurrentPlaySession();
            // Debug.Log("AI exists");

            bodyPartsWords = new[]
            {
                "mouth", "tooth", "eye", "nose", "hand", "foot", "belly", "hair", "face", "tongue", "chest", "back"
            };
        }

        #region Stefano's queries

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
            var RandomLetterData = dbManager.GetLetterDataByRandom();
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
            returnList.Add(dbManager.GetLetterDataByRandom());
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
            MiniGamesInPlaySession = new List<MiniGameData>();
            switch (this.playerProfile.CurrentJourneyPosition.PlaySession) {
                case 1:
                    //miniGames = this.dbManager.GetPlaySessionDataById("1.1.1")
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.FastCrowd_alphabet));
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.DancingDots));
                    break;
                case 2:
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Egg));
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Balloons_letter));
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Maze));
                    break;
                case 3:
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.FastCrowd_spelling));
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.ThrowBalls_letters));
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Tobogan_letters));
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.MakeFriends));
                    break;
                case 4:
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.DancingDots));
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.FastCrowd_words));
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Tobogan_words));
                    MiniGamesInPlaySession.Add(this.dbManager.GetMiniGameDataByCode(MiniGameCode.Balloons_spelling));
                    break;
            }
            return MiniGamesInPlaySession;
        }

        public MiniGameData GetCurrentMiniGameData()
        {
            MiniGamesInPlaySession = GetMiniGamesForCurrentPlaySession();
            return MiniGamesInPlaySession.ElementAt(playerProfile.CurrentMiniGameInPlaySession);
        }

        public List<Db.MiniGameData> GetMiniGameForActualPlaySession_AI(string playSessionId, int numberToSelect)
        {
            return this.minigameSelectionAI.PerformSelection(playSessionId, numberToSelect);
        }

        #endregion

        /// <summary>
        /// Give right game. Alpha version.
        /// </summary>
        //public Db.MiniGameData GetMiniGameForActualPlaySession()
        //{
        //    Db.MiniGameData miniGame = null;
        //    switch (AppManager.Instance.Player.CurrentJourneyPosition.PlaySession) {
        //        case 1:
        //            if (AppManager.Instance.PlaySessionGameDone == 0)
        //                miniGame = dbManager.GetMiniGameDataByCode(MiniGameCode.FastCrowd_letter);
        //            else
        //                miniGame = dbManager.GetMiniGameDataByCode(MiniGameCode.Balloons_spelling);
        //            break;
        //        case 2:
        //            if (AppManager.Instance.PlaySessionGameDone == 0)
        //                miniGame = dbManager.GetMiniGameDataByCode(MiniGameCode.FastCrowd_words);
        //            else
        //                miniGame = dbManager.GetMiniGameDataByCode(MiniGameCode.Tobogan_letters);
        //            break;
        //        case 3:
        //            miniGame = dbManager.GetMiniGameDataByCode(MiniGameCode.Assessment_Alphabet);
        //            break;
        //    }
        //    AppManager.Instance.CurrentMinigame = miniGame;
        //    return miniGame;
        //}

        #region MiniGames queries

        public List<float> GetLatestScoresForMiniGame(MiniGameCode minigameCode, int nLastDays)
        {
            int fromTimestamp = GenericUtilites.GetRelativeTimestampFromNow(-nLastDays);
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
            // First, get all play sessions given a stage
            List<PlaySessionData> eligiblePlaySessionData_list =
                this.dbManager.FindPlaySessionData(x => x.Stage == stage);
            List<string> eligiblePlaySessionData_id_list = eligiblePlaySessionData_list.ConvertAll(x => x.Id);

            // Then, get all scores of all play sessions
            string query = string.Format("SELECT * FROM ScoreData WHERE TableName = 'PlaySessions'");
            List<ScoreData> all_playsession_list = dbManager.FindScoreDataByQuery(query);

            // At last, filter by the given stage
            List<ScoreData> stage_playsession_list =
                all_playsession_list.FindAll(x => eligiblePlaySessionData_id_list.Contains(x.ElementId));

            return stage_playsession_list;
        }

        public List<LogMoodData> GetLastMoodData(int number)
        {
            string query = string.Format("SELECT * FROM LogMoodData ORDER BY Timestamp LIMIT {0}", number);
            List<LogMoodData> list = dbManager.FindLogMoodDataByQuery(query);
            return list;
        }

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

        #region Assessment queries

        #endregion

        #region Journeymap queries

        #endregion

        #region Mood queries

        #endregion

        #region Frequency of use queries

        #endregion
    }
}
