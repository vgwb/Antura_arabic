using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Helpers;
using Google2u;

namespace EA4S
{
    public class TeacherAI
    {

        public TeacherAI() {
            Debug.Log("AI exists");
        }

        public List<MinigameData> GimmeGoodMinigames() {
            return AppManager.Instance.DB.gameData;
        }

        public wordsRow GimmeAGoodWord() {
            return words.Instance.Rows.GetRandomElement();

        }

    }
}