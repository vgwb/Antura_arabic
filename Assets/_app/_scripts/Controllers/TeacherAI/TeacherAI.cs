using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Helpers;
using Google2u;

namespace EA4S
{
    public class TeacherAI
    {
        string[] bodyPartsWords;
        List<WordData> availableVocabulary = new List<WordData>();

        public TeacherAI() {
            // Debug.Log("AI exists");

            bodyPartsWords = new []
            {
                "mouth", "tooth", "eye", "nose", "hand", "foot", "belly", "hair", "face", "tongue", "chest", "back"
            };

        }

        public List<MinigameData> GimmeGoodMinigames() {
            return AppManager.Instance.DB.gameData;
        }

        public wordsRow GimmeAGoodWord() {
            int index = Random.Range(0, bodyPartsWords.Length - 1);
            return words.Instance.GetRow(bodyPartsWords[index]);
        }

        /// <summary>
        /// Return WordData from a list of available data.
        /// </summary>
        /// <returns></returns>
        public WordData GimmeAGoodWordData() {
            // init vocabulary
            if (availableVocabulary.Count == 0)
                availableVocabulary = getVocabularySubset(bodyPartsWords);

            List<WordData> returnList = new List<WordData>();
            foreach (WordData w in availableVocabulary) {
                if (!AppManager.Instance.ActualGameplayWordAlreadyUsed.Contains(w)) {
                    returnList.Add(w); // Only added if not already used
                }
            }
            // If all words already used (not new word in return list...)
            if (returnList.Count == 0) // ... reset already used word list.
                AppManager.Instance.ActualGameplayWordAlreadyUsed = new List<WordData>();

            WordData returnWord = returnList.GetRandomElement();
            Debug.Log("Word: " + returnWord.Key);
            AppManager.Instance.ActualGameplayWordAlreadyUsed.Add(returnWord);
            return returnWord;
        }

        List<WordData> getVocabularySubset(string[] _goodWords) {
            List<WordData> returnList = new List<WordData>();
            foreach (string wordKey in _goodWords) {
                returnList.Add(WordData.GetWordDataByKeyRow(wordKey));
            }
            return returnList;
        }
    }
}