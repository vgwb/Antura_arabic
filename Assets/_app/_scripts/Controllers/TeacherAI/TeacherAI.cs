using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    public class TeacherAI
    {
        string[] bodyPartsWords;
        List<LL_WordData> availableVocabulary = new List<LL_WordData>();

        public TeacherAI()
        {
            // Debug.Log("AI exists");

            bodyPartsWords = new[]
            {
                "mouth", "tooth", "eye", "nose", "hand", "foot", "belly", "hair", "face", "tongue", "chest", "back"
            };

        }

        #region Stefano's queries

        public List<Db.MiniGameData> GimmeGoodMinigames()
        {
            return AppManager.Instance.DB.GetActiveMinigames();
        }

        public Db.WordData GimmeAGoodWord()
        {
            int index = Random.Range(0, bodyPartsWords.Length - 1);
            return AppManager.Instance.DB.GetWordDataById(bodyPartsWords[index]);
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
            var RandomLetterData = AppManager.Instance.DB.GetLetterDataByRandom();
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
            returnList.Add(AppManager.Instance.DB.GetLetterDataByRandom());
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
                        miniGame = AppManager.Instance.DB.GetMiniGameDataByCode(MiniGameCode.FastCrowd_letter);
                    else
                        miniGame = AppManager.Instance.DB.GetMiniGameDataByCode(MiniGameCode.Balloons_spelling);
                    break;
                case 2:
                    if (AppManager.Instance.PlaySessionGameDone == 0)
                        miniGame = AppManager.Instance.DB.GetMiniGameDataByCode(MiniGameCode.FastCrowd_words);
                    else
                        miniGame = AppManager.Instance.DB.GetMiniGameDataByCode(MiniGameCode.Tobogan_letters);
                    break;
                case 3:
                    miniGame = AppManager.Instance.DB.GetMiniGameDataByCode(MiniGameCode.Assessment_Alphabet);
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
            int toTimestamp = nLastDays;

            string query = string.Format("select * from LogScoreData where Timestamp > {0} and MiniGameId = {1}", toTimestamp, minigameId);
            List<Db.ScoreData> list = AppManager.Instance.DB.FindScoreDataByQuery(query);
            List<float> scores = list.ConvertAll(x => x.Score);

            // Test log
            string output = "Scores:\n";
            foreach (var score in scores) output += score.ToString() + "\n";
            Debug.Log(output);

            return scores;
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
