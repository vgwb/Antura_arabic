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


        public TeacherAI() {
            Debug.Log("AI exists");


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

    }
}