using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Antura.Core;
using Antura.UI;
using UnityEngine.UI;

namespace Antura.Rewards
{
    /// <summary>
    /// Manages the Rewards scene.
    /// Accessed after a learning block is completed.
    /// </summary>
    public class DailyRewardScene : SceneBase
    {
        public GameObject dailyRewardUIPrefab;
        public Transform dailyRewardUIPivot;
        public BonesCounter bonesCounter;
        public GameObject todayPivot;
        public Button claimButton;

        private DailyRewardManager dailyRewardManager;
        private List<DailyRewardUI> dailyRewardUIs;
        private int newRewardIndex;

        protected override void Start()
        {
            base.Start();
            GlobalUI.ShowPauseMenu(false);

            // Cleanup UI
            foreach (Transform childTr in dailyRewardUIPivot.transform)
            {
                Destroy(childTr.gameObject);
            }

            // Setup daily reward manager
            dailyRewardManager = new DailyRewardManager();
            int nCurrentConsecutiveDaysOfPlaying = AppManager.I.Player.ComboPlayDays;
            if (nCurrentConsecutiveDaysOfPlaying > dailyRewardManager.MaxComboDays)
            {
                // Reached max
                nCurrentConsecutiveDaysOfPlaying = dailyRewardManager.MaxComboDays;
            }
            // 0 days -> nothing!
            // 1 days -> first reward
            // N days -> last reward
            newRewardIndex = nCurrentConsecutiveDaysOfPlaying-1;

            // Initialise rewards
            dailyRewardUIs = new List<DailyRewardUI>();
            int dayCounter = 0;
            foreach (var reward in dailyRewardManager.GetRewards())
            {
                dayCounter++;
                var dailyRewardUIGo = Instantiate(dailyRewardUIPrefab);
                dailyRewardUIGo.transform.SetParent(dailyRewardUIPivot);
                dailyRewardUIGo.transform.localScale = Vector3.one;
                dailyRewardUIGo.transform.localPosition = Vector3.zero;
                var dailyRewardUI = dailyRewardUIGo.GetComponent<DailyRewardUI>();
                dailyRewardUI.SetReward(reward);
                dailyRewardUI.SetDay(dayCounter);
                dailyRewardUI.SetLocked();
                dailyRewardUIs.Add(dailyRewardUI);
            }

            // Unlock the previous rewards
            for (int combo_i = 0; combo_i < newRewardIndex; combo_i++)
            {
                dailyRewardUIs[combo_i].SetUnlocked();
            }

            // Initialise UI as hidden
            bonesCounter.Hide();
            todayPivot.transform.position = Vector3.right * 1000;
            claimButton.gameObject.SetActive(false);

            claimButton.onClick.AddListener(UnlockNewReward);

            StartCoroutine(ShowRewardsCO());
        }

        IEnumerator ShowRewardsCO()
        {
            yield return new WaitForSeconds(1.0f);

            // Show the TODAY on the new one
            todayPivot.transform.position = dailyRewardUIs[newRewardIndex].transform.position;

            yield return new WaitForSeconds(1.0f);

            // Show the bones counter
            bonesCounter.Show();

            // Show the CLAIM button
            claimButton.gameObject.SetActive(true);
        }

        public void UnlockNewReward()
        {
            claimButton.gameObject.SetActive(false);
            StartCoroutine(UnlockNewRewardCO());
        }

        IEnumerator UnlockNewRewardCO()
        {

            // Unlock the new one
            dailyRewardUIs[newRewardIndex].SetUnlocked();

            // Add the new reward (for now, just bones)
            int nNewBones = dailyRewardManager.GetReward(newRewardIndex).amount;
            for (int bone_i = 0; bone_i < nNewBones; bone_i++)
            {
                bonesCounter.IncreaseByOne();
                yield return new WaitForSeconds(0.1f);
            }
            AppManager.I.Player.AddBones(nNewBones);

            yield return new WaitForSeconds(2.0f);

            // Log
            LogManager.I.LogInfo(InfoEvent.DailyRewardReceived);

            ContinueScreen.Show(Continue, ContinueScreenMode.Button, true);
        }

        private void Continue()
        {
            AppManager.I.Player.Save();
            AppManager.I.NavigationManager.GoToNextScene();
        }
    }
}