using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    public class Database
    {
        public List<MinigameData> gameData = new List<MinigameData>();

        public Database() {
            Debug.Log("Init Database()");
            
            gameData.Add(new MinigameData("fastcrowd", "الحشد سريع", "Fast Crowd", true));
            gameData.Add(new MinigameData("dontwakeup", "لا يستيقظون", "Don't Wake Up", true));
            gameData.Add(new MinigameData("balloons", "بالونات", "Balloons", true));
            gameData.Add(new MinigameData("pianowoof", "بيانو", "Piano Woof", true));
        }

    }
}