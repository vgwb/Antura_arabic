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
        private static int N_REWARDS_TO_SHOW = 5;

        public GameObject dailyRewardUIPrefab;
        public Transform dailyRewardUIPivot;
        public BonesCounter bonesCounter;
        public GameObject todayPivot;
        public GameObject yesterdayTextGo;
        public Button claimButton;

        private DailyRewardManager dailyRewardManager;
        private List<DailyRewardUI> dailyRewardUIs;
        private int newRewardUIIndex;
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
            Debug.Assert(nCurrentConsecutiveDaysOfPlaying >= 1, "Should not access this scene with 0 consecutive days");
            nCurrentConsecutiveDaysOfPlaying = Mathf.Max(nCurrentConsecutiveDaysOfPlaying, 1);

            newRewardIndex = nCurrentConsecutiveDaysOfPlaying - 1;

            // 0 days -> nothing!
            // 1 days -> first reward
            // 2+ days -> second reward
            newRewardUIIndex = Mathf.Min(newRewardIndex, 1);

            int newRewardOffset = Mathf.Max(0, nCurrentConsecutiveDaysOfPlaying - 2);

            // Initialise rewards
            dailyRewardUIs = new List<DailyRewardUI>();
            int dayCounter = 0;
            dayCounter += newRewardOffset;

            foreach (var reward in dailyRewardManager.GetRewards(newRewardOffset, newRewardOffset + N_REWARDS_TO_SHOW))
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
            for (int combo_i = 0; combo_i < newRewardUIIndex; combo_i++)
            {
                dailyRewardUIs[combo_i].SetUnlocked();
            }

            // Initialise UI as hidden
            bonesCounter.Hide();
            todayPivot.transform.position = Vector3.right * 1000;
            yesterdayTextGo.SetActive(newRewardUIIndex > 0);
            claimButton.gameObject.SetActive(false);

            claimButton.onClick.AddListener(UnlockNewReward);

            StartCoroutine(ShowRewardsCO());
        }

        IEnumerator ShowRewardsCO()
        {
            yield return new WaitForSeconds(1.0f);

            // Show the TODAY on the new one
            todayPivot.transform.position = dailyRewardUIs[newRewardUIIndex].transform.position;

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
            dailyRewardUIs[newRewardUIIndex].SetUnlocked();

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