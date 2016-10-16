using UnityEngine;
using System.Collections;

namespace EA4S.Db
{
    public class TestScriptableDB : MonoBehaviour
    {
        public Database DB;

        void Start()
        {
            foreach (var game in DB.MiniGames) {
                Debug.Log("game " + game.Title_En);
            }

            var newgame = new MiniGameData();
            newgame.InitMiniGameData(MiniGameCode.ColorTickle, "colorticke", "asda", "color Ticke", "scene_colorTicke", true);
            DB.MiniGames.Add(newgame);

        }

    }
}