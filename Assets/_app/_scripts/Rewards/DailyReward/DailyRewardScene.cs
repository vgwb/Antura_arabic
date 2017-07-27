using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Antura.Dog;
using Antura.Core;
using Antura.Database;
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
        private DailyRewardManager dailyRewardManager;

        protected override void Start()
        {
            base.Start();
            GlobalUI.ShowPauseMenu(false);

            dailyRewardManager = new DailyRewardManager();

            StartCoroutine(GiveRewardCO());
        }

        public GameObject dailyRewardUIPrefab;
        public Transform dailyRewardUIPivot;
        public BonesCounter bonesCounter;

        IEnumerator GiveRewardCO()
        {
            // Initialise rewards
            List<DailyRewardUI> dailyRewardUIs = new List<DailyRewardUI>();
            foreach (var reward in dailyRewardManager.GetRewards())
            {
                var dailyRewardUIGo = Instantiate(dailyRewardUIPrefab);
                dailyRewardUIGo.transform.SetParent(dailyRewardUIPivot);
                dailyRewardUIGo.transform.localScale = Vector3.one;
                dailyRewardUIGo.transform.localPosition = Vector3.zero;
                var dailyRewardUI = dailyRewardUIGo.GetComponent<DailyRewardUI>();
                dailyRewardUI.SetReward(reward);
                dailyRewardUI.SetLocked();
                dailyRewardUIs.Add(dailyRewardUI);
            }

            int nComboDays = AppManager.I.Player.ComboPlayDays;

            if (nComboDays > dailyRewardManager.MaxComboDays)
            {
                nComboDays = dailyRewardManager.MaxComboDays;
            }
            int newRewardDay = nComboDays - 1;

            for (int combo_i = 0; combo_i < newRewardDay; combo_i++)
            {
                dailyRewardUIs[combo_i].SetUnlocked();
            }

            bonesCounter.SetValueAuto();

            // Unlock the new one
            yield return new WaitForSeconds(1.5f);
            dailyRewardUIs[newRewardDay].SetUnlocked();

            // Add the new reward (for now, just bones)
            int nNewBones = dailyRewardManager.GetReward(newRewardDay).amount;
            for (int bone_i = 0; bone_i < nNewBones; bone_i++)
            {
                bonesCounter.IncreaseByOne();
                yield return new WaitForSeconds(0.1f);
            }
            AppManager.I.Player.AddBones(nNewBones);

            yield return new WaitForSeconds(4.4f);

            ContinueScreen.Show(Continue, ContinueScreenMode.Button, true);
        }

        private void Continue()
        {
            AppManager.I.Player.Save();
            AppManager.I.NavigationManager.GoToNextScene();
        }
    }
}