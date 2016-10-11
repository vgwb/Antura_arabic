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
            gameData.Add(new MinigameData("Balloons_spelling", "البالونات", "Balloons", "game_Balloons", true));
            gameData.Add(new MinigameData("DancingDots", "", "Dancing Dots", "game_DancingDots", true));
            gameData.Add(new MinigameData("FastCrowd_letter", "جماهير الحروف", "Fast Crowd", "game_FastCrowd", true));
            gameData.Add(new MinigameData("FastCrowd_words", "جماهير الكلمات", "Fast Crowd Words", "game_FastCrowd", true));
            gameData.Add(new MinigameData("MakeFriends", "", "Make Friends Words", "game_MakeFriends", true));
            gameData.Add(new MinigameData("Maze", "", "Maze", "game_Maze", true));
            gameData.Add(new MinigameData("ThrowBalls", "", "Throw Balls", "game_ThrowBalls", true));
            gameData.Add(new MinigameData("Tobogan", "", "Tobogan", "game_Tobogan", true));

            //gameData.Add(new MinigameData("dontwakeup", "لا توقظ عنتورة", "Don't Wake Antura", "game_DontWakeUp", true));
            //gameData.Add(new MinigameData("pianowoof", "بيانو", "Piano Woof", "", true));
        }

    }
}