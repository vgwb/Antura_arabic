using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    public class Database
    {
        public List<MinigameData> gameData = new List<MinigameData>();

        public Database() {
            // Debug.Log("Init Database()");
            
            gameData.Add(new MinigameData("fastcrowd", "الحشد سريع", "Fast Crowd", "game_FastCrowd", true));
            gameData.Add(new MinigameData("dontwakeup", "لا يستيقظون", "Don't Wake Up", "game_DontWakeUp", true));
            gameData.Add(new MinigameData("balloons", "بالونات", "Balloons", "game_Balloons", true));
            gameData.Add(new MinigameData("fastcrowd_words", "words الحشد سريع", "Fast Crowd Words", "game_FastCrowd", true));
            //gameData.Add(new MinigameData("pianowoof", "بيانو", "Piano Woof", "", true));
        }

    }
}