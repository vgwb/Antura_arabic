using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    public class TeacherAI
    {
        string[] bodyPartsWords;
        List<WordData> availableVocabulary = new List<WordData>();

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
            return AppManager.Instance.DB.FindAllActiveMinigames();
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
        public WordData GimmeAGoodWordData()
        {
            // init vocabulary
            if (availableVocabulary.Count == 0)
                availableVocabulary = getVocabularySubset(bodyPartsWords);

            List<WordData> returnList = new List<WordData>();
            if (AppManager.Instance.ActualGameplayWordAlreadyUsed.Count >= availableVocabulary.Count) // if already used all available words... restart.
                AppManager.Instance.ActualGameplayWordAlreadyUsed = new List<WordData>();
            foreach (WordData w in availableVocabulary) {
                if (!AppManager.Instance.ActualGameplayWordAlreadyUsed.Contains(w)) {
                    returnList.Add(w); // Only added if not already used
                }
            }

            WordData returnWord = returnList.GetRandom();
            // Debug.Log("Word: " + returnWord.Key);
            AppManager.Instance.ActualGameplayWordAlreadyUsed.Add(returnWord);
            return returnWord;
        }

        public LetterData GimmeARandomLetter()
        {
            var RandomLetterData = AppManager.Instance.DB.GetLetterDataByRandom();
            return new LetterData(RandomLetterData.GetId());
        }

        List<WordData> getVocabularySubset(string[] _goodWords)
        {
            List<WordData> returnList = new List<WordData>();
            foreach (string wordKey in _goodWords) {
                returnList.Add(WordData.GetWordDataByKeyRow(wordKey));
            }
            return returnList;
        }

        #endregion


        #region MiniGames queries

        public List<float> GetLatestScoresForMiniGame(MiniGameCode code, int nLastDays)
        {
            string minigameId = code.ToString();
            int toTimestamp = nLastDays;

            string query = string.Format("select * from LogScoreData where Timestamp > {0} and MiniGameId = {1}", toTimestamp, minigameId);
            List<Db.LogScoreData> list = AppManager.Instance.DB.FindLogScoreDataByQuery(query);
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
