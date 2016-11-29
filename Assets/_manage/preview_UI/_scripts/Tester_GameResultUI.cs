// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/18

using System.Collections.Generic;
using EA4S.Db;
using UnityEngine;

namespace EA4S.Test
{
    public class Tester_GameResultUI : MonoBehaviour
    {
        #region EndgameResult

        public void EndgameResult_Show(int _numStars)
        {
            GameResultUI.HideEndsessionResult();

            GameResultUI.ShowEndgameResult(_numStars);
        }

        #endregion

        #region EndsessionResult

        public void EndsessionResult_Show(int _totMinigamesStars)
        {
            GameResultUI.HideEndgameResult();

            MiniGameData d0 = new MiniGameData() { Main = MiniGameCode.Maze.ToString(), Variation = "letters" };
            MiniGameData d1 = new MiniGameData() { Main = MiniGameCode.DancingDots.ToString(), Variation = "alphabet" };
            MiniGameData d2 = new MiniGameData() { Main = MiniGameCode.MakeFriends.ToString(), Variation = "counting" };
            List<EndsessionResultData> res = new List<EndsessionResultData>() {
                new EndsessionResultData(2, d0.GetIconResourcePath(), d0.GetBadgeIconResourcePath()),
                new EndsessionResultData(0, d1.GetIconResourcePath(), d1.GetBadgeIconResourcePath()),
                new EndsessionResultData(3, d2.GetIconResourcePath(), d2.GetBadgeIconResourcePath()),
            };
            for (int i = 0; i < 3; ++i) {
                int num = Mathf.Min(3, _totMinigamesStars);
                _totMinigamesStars -= num;
                res[i].Stars = num;
            }
            int rndUnlocked = UnityEngine.Random.Range(0, 3);
            Debug.Log("ShowEndsessionResult > " + rndUnlocked);
            GameResultUI.ShowEndsessionResult(res, rndUnlocked);
        }

        #endregion
    }
}