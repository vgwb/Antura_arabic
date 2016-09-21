using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    public class Database
    {
        public List<MinigameData> gameData = new List<MinigameData>();

        public Database()
        {
            // Debug.Log("Init Database()");
            gameData.Add(new MinigameData("fastcrowd", "جماهير الحروف", "Fast Crowd", "game_FastCrowd", true));
            gameData.Add(new MinigameData("dontwakeup", "لا توقظ عنتورة", "Don't Wake Antura", "game_DontWakeUp", true));
            gameData.Add(new MinigameData("balloons", "البالونات", "Balloons", "game_Balloons", true));
            gameData.Add(new MinigameData("fastcrowd_words", "جماهير الكلمات", "Fast Crowd Words", "game_FastCrowd", true));
            //gameData.Add(new MinigameData("pianowoof", "بيانو", "Piano Woof", "", true));
        }

    }
}