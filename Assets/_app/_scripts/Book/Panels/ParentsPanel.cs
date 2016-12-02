using UnityEngine;
using UnityEngine.UI;
using EA4S.Db;
using System.Collections.Generic;

namespace EA4S
{
    public class ParentsPanel : MonoBehaviour
    {
        [Header("Prefabs")]
        //public GameObject LearningBlockItemPrefab;

        [Header("References")]
        //public GameObject ElementsContainer;
        public TextRender ScoreText;

        void OnEnable()
        {
            InitUI();
            parentLockCounter = 0;
            lockedParentAreaGo.SetActive(false);
        }

        void InitUI()
        {

        }

        public void OnUnlockAllGameData()
        {
            GlobalUI.ShowPrompt(true, "UNLOCK EVERYTHING? (also adds fake scores)", GoUnlockAllGameData);
        }

        void GoUnlockAllGameData()
        {
            Debug.Log("Cheat Mode enabled: unlocking all game data");
            var maxJourneyPos = new JourneyPosition(6, 15, 100);
            SetJourneyPos(maxJourneyPos);
            StartCoroutine(PopulateDatabaseWithUsefulDataCO(maxJourneyPos, true));
            GoUnlockAllRewards();
        }

        public void OnDeleteProfile()
        {
            GlobalUI.ShowPrompt(true, "delete this profile?", GoDeleteProfile);
        }

        void GoDeleteProfile()
        {
            AppManager.I.ResetCurrentPlayer();
        }

        public void OnResetEverything()
        {
            GlobalUI.ShowPrompt(true, "ATTENTION\nReally Reset all data and everything?", GoResetEverything);
        }

        void GoResetEverything()
        {
            AppManager.I.ResetEverything();
        }

        public void OnExportData()
        {
            GlobalUI.ShowPrompt(true, "Export the current Database?\n(not yet functional: will save the db into the device or send it to our data gathering center)", GoExportData);
        }

        void GoExportData()
        {
            Debug.Log("Data exported");
        }

        public void OnUnlockStage(int stage)
        {
            Debug.Log("Unlocking up to stage " + stage);
            var targetJourneyPos = new JourneyPosition(stage, 1, 1);
            SetJourneyPos(targetJourneyPos);
            StartCoroutine(PopulateDatabaseWithUsefulDataCO(targetJourneyPos));
        }

        public void OnUnlockAllRewards()
        {
            GlobalUI.ShowPrompt(true, "Unlock some rewards rewards?", GoUnlockAllRewards);

        }

        public void GoUnlockAllRewards()
        {
            // moved to centralized location
            RewardSystemManager.UnlockAllRewards();
        }

        #region Super Dog Helpers

        public Image pleaseWaitPanel;

        private void SetJourneyPos(JourneyPosition targetPosition)
        {
            // @note: set as SRDebugOptions
            AppManager.I.Player.SetMaxJourneyPosition(targetPosition, true);
        }

        private System.Collections.IEnumerator PopulateDatabaseWithUsefulDataCO(JourneyPosition targetPosition, bool cheatMode = false)
        {
            pleaseWaitPanel.gameObject.SetActive(true);
            GlobalUI.I.BackButton.gameObject.SetActive(false);

            var logAi = AppManager.I.Teacher.logAI;
            var fakeAppSession = LogManager.I.Session;

            if (cheatMode) {
                // Enable cheat mode
                AppManager.I.GameSettings.CheatSuperDogMode = true;

                // Add some mood data
                int nMoodData = 15;
                for (int i = 0; i < nMoodData; i++) {
                    logAi.LogMood(Random.Range(AppConstants.minimumMoodValue, AppConstants.maximumMoodValue + 1));
                    Debug.Log("Add mood " + i);
                    yield return null;
                }

                // Force update of graph
                FindObjectOfType<GraphMood>().OnEnable();
            }

            // Add scores for all play sessions
            var allPlaySessionInfos = AppManager.I.Teacher.scoreHelper.GetAllPlaySessionInfo();
            for (int i = 0; i < allPlaySessionInfos.Count; i++) {
                if (allPlaySessionInfos[i].data.Stage <= targetPosition.Stage) {
                    logAi.LogPlaySessionScore(allPlaySessionInfos[i].data.Id, Random.Range(1, 4));
                    Debug.Log("Add play session score for " + allPlaySessionInfos[i].data.Id);
                    yield return null;
                }
            }

            if (cheatMode) {
                // Add scores for all minigames
                var allMiniGameInfo = AppManager.I.Teacher.scoreHelper.GetAllMiniGameInfo();
                for (int i = 0; i < allMiniGameInfo.Count; i++) {
                    logAi.LogMiniGameScore(allMiniGameInfo[i].data.Code, Random.Range(1, 4));
                    Debug.Log("Add minigame score " + i);
                    yield return null;
                }
            }

            // Add scores for some learning data (words/letters/phrases)
            /*var maxPlaySession = AppManager.I.Player.MaxJourneyPosition.ToString();
            var allWordInfo = AppManager.I.Teacher.scoreHelper.GetAllWordInfo();
            for (int i = 0; i < allWordInfo.Count; i++)
            {
                if (Random.value < 0.3f)
                {
                    var resultsList = new List<Teacher.LogAI.LearnResultParameters>();
                    var newResult = new Teacher.LogAI.LearnResultParameters();
                    newResult.elementId = allWordInfo[i].data.Id;
                    newResult.table = DbTables.Words;
                    newResult.nCorrect = Random.Range(1,5);
                    newResult.nWrong = Random.Range(1, 5);
                    resultsList.Add(newResult);
                    logAi.LogLearn(fakeAppSession, maxPlaySession, MiniGameCode.Assessment_LetterShape, resultsList);
                }
            }
            var allLetterInfo = AppManager.I.Teacher.scoreHelper.GetAllLetterInfo();
            for (int i = 0; i < allLetterInfo.Count; i++)
            {
                if (Random.value < 0.3f)
                {
                    var resultsList = new List<Teacher.LogAI.LearnResultParameters>();
                    var newResult = new Teacher.LogAI.LearnResultParameters();
                    newResult.elementId = allLetterInfo[i].data.Id;
                    newResult.table = DbTables.Letters;
                    newResult.nCorrect = Random.Range(1, 5);
                    newResult.nWrong = Random.Range(1, 5);
                    resultsList.Add(newResult);
                    logAi.LogLearn(fakeAppSession, maxPlaySession, MiniGameCode.Assessment_LetterShape, resultsList);
                }
            }*/

            pleaseWaitPanel.gameObject.SetActive(false);
            GlobalUI.I.BackButton.gameObject.SetActive(true);
        }

        #endregion

        #region Parent Lock

        private int parentLockCounter;

        public void OnGreenParentUnlock()
        {
            parentLockCounter++;
        }

        public void OnRedParentUnlock()
        {
            if (parentLockCounter == 7) {
                UnlockParentControls();
            } else {
                parentLockCounter = 8; // disabling
            }
        }

        public GameObject lockedParentAreaGo;

        private void UnlockParentControls()
        {
            lockedParentAreaGo.SetActive(true);
        }

        #endregion
    }
}