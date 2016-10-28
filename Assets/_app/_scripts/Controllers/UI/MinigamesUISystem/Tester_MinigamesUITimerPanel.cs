// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/10/28

using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class Tester_MinigamesUITimerPanel : MonoBehaviour
    {
        public Button[] DisabledUntilSetup;

        Button[] bts;

        #region Unity

        void Start()
        {
            bts = this.GetComponentsInChildren<Button>(true);
            foreach (Button button in bts) {
                Button bt = button;
                bt.onClick.AddListener(Refresh);
            }
        }

        void OnDestroy()
        {
            foreach (Button bt in bts) bt.onClick.RemoveAllListeners();
        }

        #endregion

        #region Public Methods

        public void Refresh()
        {
            foreach (Button bt in DisabledUntilSetup) {
                bt.interactable = MinigamesUI.Timer.IsSetup;
            }
        }

        #endregion
    }
}