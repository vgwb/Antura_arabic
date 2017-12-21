using Antura.Audio;
using Antura.Core;
using Antura.Helpers;
using Antura.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.Rewards
{
    /// <summary>
    /// every day some rewards
    /// </summary>
    public class DailyRewardScene : SceneBase
    {
        private const int MAX_REWARDS_TO_SHOW = 5;

        public GameObject dailyRewardUIPrefab;
        public Transform dailyRewardUIPivot;
        public BonesCounter bonesCounter;
        public GameObject todayPivot;
        public GameObject yesterdayTextGo;
        public Button claimButton;
        [SerializeField]
        private DailyRewardPopupPool popupPool;
        [SerializeField]
        private RectTransform fromPopupPivot, toPopupPivot;

        private DailyRewardManager dailyRewardManager;
        private List<DailyRewardUI> dailyRewardUIs;
        private int newRewardUIIndex;
        private int newRewardContentIndex;

        [Header("Debug")]
        public bool useForcedConsecutiveDays = false;
        public int forcedConsecutiveDays = 1;

        protected override void Start()
        {
            base.Start();
            GlobalUI.ShowPauseMenu(false);

            // Cleanup UI
            foreach (Transform childTr in dailyRewardUIPivot.transform) {
                Destroy(childTr.gameObject);
            }

            dailyRewardManager = new DailyRewardManager();
            int nCurrentConsecutiveDaysOfPlaying = AppManager.I.Player.ConsecutivePlayDays;
            Debug.Assert(nCurrentConsecutiveDaysOfPlaying >= 1, "Should not access this scene with 0 consecutive days");
            nCurrentConsecutiveDaysOfPlaying = Mathf.Max(nCurrentConsecutiveDaysOfPlaying, 1);

            if (useForcedConsecutiveDays) {
                nCurrentConsecutiveDaysOfPlaying = forcedConsecutiveDays;
                Debug.LogError("FORCING CONSECUTIVE DAYS: " + nCurrentConsecutiveDaysOfPlaying);
            }

            // Index of the new reward (for the content, not the UI)
            newRewardContentIndex = nCurrentConsecutiveDaysOfPlaying - 1;

            int nRewardsToShowToday = Mathf.Min(MAX_REWARDS_TO_SHOW, nCurrentConsecutiveDaysOfPlaying + 2);

            // 0 days -> nothing!
            // 1 days -> first reward
            // 2+ days -> second reward
            newRewardUIIndex = Mathf.Min(newRewardContentIndex, 2);

            // Offset of where the NEW reward is (0 -> all to the right)
            int newRewardOffset = 0;

            /*
            Debug.Log("New Reward Index: " + newRewardContentIndex);
            Debug.Log("New Reward UI Index: " + newRewardUIIndex);
            Debug.Log("New Reward Offset: " + newRewardOffset);
            */

            // Initialise rewards
            dailyRewardUIs = new List<DailyRewardUI>();
            int dayCounter = 0;
            dayCounter += newRewardOffset;

            foreach (var reward in dailyRewardManager.GetRewards(newRewardOffset, newRewardOffset + nRewardsToShowToday)) {
                dayCounter++;
                var dailyRewardUIGo = Instantiate(dailyRewardUIPrefab);
                dailyRewardUIGo.transform.SetParent(dailyRewardUIPivot);
                dailyRewardUIGo.transform.localScale = Vector3.one;
                dailyRewardUIGo.transform.localPosition = Vector3.zero;
                var dailyRewardUI = dailyRewardUIGo.GetComponent<DailyRewardUI>();
                dailyRewardUI.SetReward(reward);
                dailyRewardUI.SetDay(dayCounter);
                dailyRewardUI.HideDay();
                dailyRewardUI.SetLocked();
                dailyRewardUIs.Add(dailyRewardUI);
            }

            // Unlock the previous rewards
            for (int combo_i = 0; combo_i < newRewardUIIndex; combo_i++) {
                dailyRewardUIs[combo_i].SetUnlocked();
            }
            // Prepare the next one
            dailyRewardUIs[newRewardUIIndex].SetReadyToBeUnlocked();

            // Initialise UI as hidden
            bonesCounter.Hide();
            todayPivot.transform.position = Vector3.right * 1000;
            todayPivot.gameObject.SetActive(false);
            yesterdayTextGo.SetActive(newRewardUIIndex > 0);
            claimButton.gameObject.SetActive(false);

            claimButton.onClick.AddListener(UnlockNewReward);

            StartCoroutine(ShowRewardsCO(nCurrentConsecutiveDaysOfPlaying != 1));
        }

        IEnumerator ShowRewardsCO(bool withTranslation)
        {
            // Start to the left
            float targetX = dailyRewardUIPivot.transform.localPosition.x;
            if (withTranslation) {
                dailyRewardUIPivot.transform.localPosition += Vector3.left * 200;
            }

            Sequence s = DOTween.Sequence().AppendInterval(1f);
            if (withTranslation) {
                s.Append(dailyRewardUIPivot.DOLocalMoveX(targetX, 1f).SetEase(Ease.InOutSine));
            }
            s.AppendCallback(() => {
                todayPivot.transform.position = dailyRewardUIs[newRewardUIIndex].transform.position;
                todayPivot.gameObject.SetActive(true);
            });
            s.Insert(s.Duration() - 0.15f, dailyRewardUIs[newRewardUIIndex].transform.DOScale(1.25f, 0.35f).SetEase(Ease.OutBack));
            yield return s.WaitForCompletion();

            AudioManager.I.PlaySound(Sfx.Win);
            dailyRewardUIs[newRewardUIIndex].Bounce(true);

            //            yield return new WaitForSeconds(1.0f);
            //
            //            //  Translate to the middle
            //            if (withTranslation)
            //                dailyRewardUIPivot.DOLocalMoveX(0, 1f).SetEase(Ease.InOutSine);
            //
            //            dailyRewardUIs[newRewardUIIndex].transform.DOScale(1.3f, 1f).SetEase(Ease.OutBack);
            //
            //            yield return new WaitForSeconds(1.0f);

            // Show the TODAY on the new one
            //            todayPivot.transform.position = dailyRewardUIs[newRewardUIIndex].transform.position;

            bonesCounter.Show();

            // Set in CLAIM mode (any click will work)
            waitForClaimCoroutine = StartCoroutine(WaitForClaimCO());
        }

        private Coroutine waitForClaimCoroutine;
        IEnumerator WaitForClaimCO()
        {
            float waitTime = 0.0f;
            while (true) {
                waitTime += Time.deltaTime;

                if (Input.GetMouseButtonDown(0)) {
                    UnlockNewReward();
                }

                yield return null;

                if (waitTime > 1.0f) {
                    waitTime -= 1.0f;
                    Tutorial.TutorialUI.Click(dailyRewardUIs[newRewardUIIndex].transform.position);
                }
            }
        }

        public void UnlockNewReward()
        {
            StopCoroutine(waitForClaimCoroutine);
            StartCoroutine(UnlockNewRewardCO());
        }

        IEnumerator UnlockNewRewardCO()
        {
            // Unlock the new one
            dailyRewardUIs[newRewardUIIndex].SetUnlocked(true);

            // Add the new reward (for now, just bones)
            int nNewBones = dailyRewardManager.GetReward(newRewardContentIndex).amount;
            List<DailyRewardPopup> popups = popupPool.Spawn(nNewBones);
            Vector2 toP = UIHelper.SwitchToRectTransform(toPopupPivot, popupPool.GetComponent<RectTransform>());
            for (int bone_i = 0; bone_i < nNewBones; bone_i++) {
                bonesCounter.IncreaseByOne();
                popups[bone_i].PopFromTo(fromPopupPivot.anchoredPosition, toP);
                yield return new WaitForSeconds(0.1f);
            }
            AppManager.I.Player.AddBones(nNewBones);

            yield return new WaitForSeconds(1.0f);

            // Log
            LogManager.I.LogInfo(InfoEvent.DailyRewardReceived);

            // Continue after a little while
            Invoke("Continue", 1.5f);
        }

        private void Continue()
        {
            AppManager.I.Player.Save();
            AppManager.I.NavigationManager.GoToNextScene();
        }
    }
}