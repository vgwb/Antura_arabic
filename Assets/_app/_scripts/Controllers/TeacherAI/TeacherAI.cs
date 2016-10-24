using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S
{
    public class TeacherAI
    {
        private DatabaseManager dbManager;
        private PlayerProfile playerProfile;
        string[] bodyPartsWords;
        List<LL_WordData> availableVocabulary = new List<LL_WordData>();

        public TeacherAI(DatabaseManager _dbManager, PlayerProfile _playerProfile)
        {
            this.dbManager = _dbManager;
            this.playerProfile = _playerProfile;

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
            if (AppManager.Instance.ActualGameplayWordAlreadyUsed.Count >= availableVocabulary.Count) // if already used all available words... restart.
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


        /// <summary>
        /// Give right game. Alpha version.
        /// </summary>
        public Db.MiniGameData GetMiniGameForActualPlaySession()
        {
            Db.MiniGameData miniGame = null;
            switch (AppManager.Instance.PlaySession)
            {
                case 1:
                    if (AppManager.Instance.PlaySessionGameDone == 0)
                        miniGame = dbManager.GetMiniGameDataByCode(MiniGameCode.FastCrowd_letter);
                    else
                        miniGame = dbManager.GetMiniGameDataByCode(MiniGameCode.Balloons_spelling);
                    break;
                case 2:
                    if (AppManager.Instance.PlaySessionGameDone == 0)
                        miniGame = dbManager.GetMiniGameDataByCode(MiniGameCode.FastCrowd_words);
                    else
                        miniGame = dbManager.GetMiniGameDataByCode(MiniGameCode.Tobogan_letters);
                    break;
                case 3:
                    miniGame = dbManager.GetMiniGameDataByCode(MiniGameCode.Assessment_Alphabet);
                    break;
            }
            AppManager.Instance.ActualMinigame = miniGame;
            return miniGame;
        }

        #endregion


        #region MiniGames queries

        public List<float> GetLatestScoresForMiniGame(MiniGameCode code, int nLastDays)
        {
            string minigameId = code.ToString();
            int toTimestamp = GenericUtilites.GetTimestampForNow();
            int fromTimestamp = GenericUtilites.GetRelativeTimestampFromNow(-nLastDays);

            string query = string.Format("SELECT * FROM LogPlayData WHERE Timestamp > {0} AND Table = MiniGame AND ElementId = {2}", fromTimestamp, minigameId);
            List<ScoreData> list = dbManager.FindScoreDataByQuery(query);
            List<float> scores = list.ConvertAll(x => x.Score);

            return scores;
        }

        public List<LogMoodData> GetLastMoodData(int number)
        {
            string query = string.Format("SELECT * FROM LogMoodData ORDER BY Timestamp LIMIT {0}", number); 
            List<LogMoodData> list = dbManager.FindLogMoodDataByQuery(query);
            return list;
        }

        public List<ScoreData> GetCurrentScoreForAllPlaySessions()
        {
            string query = string.Format("SELECT * FROM ScoreData WHERE Table = PlaySessionData ORDER BY ElementId");
            List<ScoreData> list = dbManager.FindScoreDataByQuery(query);
            return list;
        }

        // @note: shows how to work on the dynamic and static db together
        public List<LogLearnData> GetFailedAssessmentLetters(MiniGameCode assessmentCode)
        {
            string query = string.Format("SELECT * FROM LogLearnData WHERE MiniGame = {0} AND Table = LetterData AND Score < 0", assessmentCode);
            List<LogLearnData> list = dbManager.FindLogLearnDataByQuery(query);
            List<string> ids_list = list.ConvertAll(x => x.Id);
            dbManager.FindLetterData(x => ids_list.Contains(x.Id));
            return list;
        }

        // @note: shows how to work on the dynamic and static db together
        public List<LogLearnData> GetFailedAssessmentWords(MiniGameCode assessmentCode)
        {
            string query = string.Format("SELECT * FROM LogLearnData WHERE MiniGame = {0} AND Table = WordData AND Score < 0", assessmentCode);
            List<LogLearnData> list = dbManager.FindLogLearnDataByQuery(query);
            List<string> ids_list = list.ConvertAll(x => x.Id);
            dbManager.FindWordData(x => ids_list.Contains(x.Id));
            return list;
        }

        // @note: shows how to work with playerprofile as well as the database
        public List<LogPlayData> GetAllScoresForCurrentProgress()
        {
            JourneyPosition currentJourneyPosition = playerProfile.ActualJourneyPosition;
            string playsession_id = currentJourneyPosition.Stage + "." + currentJourneyPosition.LearningBlock + "." + currentJourneyPosition.PlaySession;
            string query = string.Format("SELECT * FROM LogPlayData WHERE Table = PlaySessionData AND PlayEvent = {0} AND ElementId = {1}", PlayEvent.GameFinished, playsession_id);
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
