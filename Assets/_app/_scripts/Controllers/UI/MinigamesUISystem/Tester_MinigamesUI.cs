// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/10/28

using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class Tester_MinigamesUI : MonoBehaviour
    {
        public Tester_MinigamesUITimerPanel TimerPanel;

        #region Unity

        void Awake()
        {
            TimerPanel.gameObject.SetActive(false);
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

            TimerPanel.gameObject.SetActive(_id == 1 || _id == 3);
            TimerPanel.Refresh();
        }

        #endregion

        #region Timer

        public void Timer_Setup(float _timerDuration)
        {
            MinigamesUI.Timer.Setup(_timerDuration);
        }

        public void Timer_Play()
        {
            MinigamesUI.Timer.Play();
        }

        public void Timer_Pause()
        {
            MinigamesUI.Timer.Pause();
        }

        public void Timer_ReStart()
        {
            MinigamesUI.Timer.Restart();
        }

        public void Timer_Rewind()
        {
            MinigamesUI.Timer.Rewind();
        }

        public void Timer_Complete()
        {
            MinigamesUI.Timer.Complete();
        }

        public void Timer_Goto(float _time)
        {
            MinigamesUI.Timer.Goto(_time, true);
        }

        public void Timer_GotoPercentage(float _percentage)
        {
            MinigamesUI.Timer.GotoPercentage(_percentage, true);
        }

        #endregion
    }
}