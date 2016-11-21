// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/18

using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class GameResultUI : MonoBehaviour
    {
        public EndgameResultPanel EndgameResultPanel;
        public EndsessionResultPanel EndsessionResultPanel;

        static GameResultUI I;
        const string ResourcesPath = "Prefabs/UI/GameResultUI";

        #region Unity + Init

        static void Init()
        {
            if (I != null) return;

            I = Instantiate(Resources.Load<GameResultUI>(ResourcesPath));
        }

        void Awake()
        {
            I = this;
        }

        void OnDestroy()
        {
            if (I == this) I = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Never use this directly! Use the <code>Minigames Interface</code> instead.
        /// </summary>
        public static void ShowEndgameResult(int _numStars)
        {
            Init();
            I.EndgameResultPanel.Show(_numStars);
        }

        /// <summary>
        /// Never use this directly! Use the <code>Minigames Interface</code> instead.
        /// </summary>
        public static void ShowEndsessionResult(List<EndsessionResultData> _sessionData, bool _immediate = false)
        {
            Init();
            I.EndsessionResultPanel.Show(_sessionData, _immediate);
        }

        /// <summary>
        /// Never use this directly! Use the <code>Minigames Interface</code> instead.
        /// </summary>
        public static void HideEndgameResult(bool _immediate = false)
        {
            if (I == null) return;

            I.EndgameResultPanel.Hide(_immediate);
        }

        /// <summary>
        /// Never use this directly! Use the <code>Minigames Interface</code> instead.
        /// </summary>
        public static void HideEndsessionResult(bool _immediate = false)
        {
            if (I == null) return;

            I.EndsessionResultPanel.Hide(_immediate);
        }

        #endregion
    }
}