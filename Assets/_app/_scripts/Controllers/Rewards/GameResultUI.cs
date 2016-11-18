// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/18

using System.Collections;
using UnityEngine;

namespace EA4S
{
    public class GameResultUI : MonoBehaviour
    {
        public EndgameResultPanel EndgameResultPanel;

        static GameResultUI I;
        const string ResourcesPath = "Prefabs/UI/GameResultUI";

        #region Unity + Init

        static void Init()
        {
            if (I != null) return;

            I = Instantiate(Resources.Load<EA4S.GameResultUI>(ResourcesPath));
        }

        void OnDestroy()
        {
            if (I == this) I = null;
        }

        #endregion

        #region Public Methods

        public static void HideEndgameResult()
        {
            if (I == null) return;

            I.EndgameResultPanel.Show(false);
        }

        public static void ShowEndgameResult(int _numStars)
        {
            Init();
            I.EndgameResultPanel.Show(true, _numStars);
        }

        #endregion
    }
}