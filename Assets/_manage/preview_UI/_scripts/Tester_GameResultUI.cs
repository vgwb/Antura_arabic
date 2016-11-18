// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/18

using UnityEngine;

namespace EA4S.Test
{
    public class Tester_GameResultUI : MonoBehaviour
    {
        #region EndgameResult

        public void EndgameResult_Show(int _numStars)
        {
            GameResultUI.ShowEndgameResult(_numStars);
        }

        #endregion
    }
}