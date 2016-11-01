// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/10/28

using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class Tester_MinigamesUI : MonoBehaviour
    {
        public Tester_MinigamesUIPanel[] Panels;

        #region Unity

        void Awake()
        {
            foreach (Tester_MinigamesUIPanel panel in Panels) panel.gameObject.SetActive(false);
        }

        #endregion

        #region Init

        public void Init(int _id)
        {
            switch (_id) {
            case 0: MinigamesUI.Init(MinigamesUIElement.Starbar);
                break;
            case 1: MinigamesUI.Init(MinigamesUIElement.Starbar | MinigamesUIElement.Timer);
                break;
            case 2: MinigamesUI.Init(MinigamesUIElement.Starbar | MinigamesUIElement.Lives);
                break;
            case 3: MinigamesUI.Init(MinigamesUIElement.Starbar | MinigamesUIElement.Timer | MinigamesUIElement.Lives);
                break;
            }

            foreach (Tester_MinigamesUIPanel panel in Panels) {
                switch (panel.PanelType) {
                case Tester_MinigamesUIPanel.UIPanelType.Timer:
                    panel.gameObject.SetActive(_id == 1 || _id == 3);
                    break;
                case Tester_MinigamesUIPanel.UIPanelType.Lives:
                    panel.gameObject.SetActive(_id == 2 || _id == 3);
                    break;
                case Tester_MinigamesUIPanel.UIPanelType.Starbar:
                    panel.gameObject.SetActive(true);
                    break;
                default:
                    continue;
                }
                if (panel.gameObject.activeSelf) panel.Refresh();
            }
        }

        #endregion

        #region Lives

        public void Lives_Setup(int _totLives)
        { MinigamesUI.Lives.Setup(_totLives); }

        public void Lives_SetCurrLives(int _to)
        { MinigamesUI.Lives.SetCurrLives(_to, true); }

        public void Lives_ResetToMax()
        { MinigamesUI.Lives.ResetToMax(); }
        public void Lives_GainALife(bool _canExceedMax)
        { MinigamesUI.Lives.GainALife(_canExceedMax); }

        public void Lives_LoseALife()
        { MinigamesUI.Lives.LoseALife(); }

        #endregion

        #region Starbar

        public void Starbar_Goto(float _percentage)
        { MinigamesUI.Starbar.Goto(_percentage); }

        public void Starbar_GotoStar(int _starIndex)
        { MinigamesUI.Starbar.GotoStar(_starIndex); }

        #endregion

        #region Timer

        public void Timer_Setup(float _timerDuration)
        { MinigamesUI.Timer.Setup(_timerDuration); }

        public void Timer_Play()
        { MinigamesUI.Timer.Play(); }

        public void Timer_Pause()
        { MinigamesUI.Timer.Pause(); }

        public void Timer_ReStart()
        { MinigamesUI.Timer.Restart(); }

        public void Timer_Rewind()
        { MinigamesUI.Timer.Rewind(); }

        public void Timer_Complete()
        { MinigamesUI.Timer.Complete(); }

        public void Timer_Goto(float _time)
        { MinigamesUI.Timer.Goto(_time, true); }

        public void Timer_GotoPercentage(float _percentage)
        { MinigamesUI.Timer.GotoPercentage(_percentage, true); }

        #endregion
    }
}