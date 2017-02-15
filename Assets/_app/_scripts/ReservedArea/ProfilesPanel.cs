using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EA4S.Core;
using EA4S.UI;
using EA4S.Profile;

namespace EA4S.ReservedArea
{
    public class ProfilesPanel : MonoBehaviour
    {
        public GameObject PlayerIconContainer;
        public GameObject PlayerIconPrefab;
        public GameObject ProfileCommandsContainer;
        public GameObject pleaseWaitPanel;

        string SelectedPlayerId;

        void Start()
        {
            ResetAll();
        }

        void ResetAll()
        {
            SelectedPlayerId = "";
            RefreshPlayerIcons();
            RefreshUI();
        }

        void RefreshPlayerIcons()
        {
            GameObject newIcon;

            foreach (Transform t in PlayerIconContainer.transform) {
                Destroy(t.gameObject);
            }

            List<PlayerIconData> players = AppManager.I.PlayerProfileManager.GetSavedPlayers();

            // reverse the list for RIGHT 2 LEFT layout
            players.Reverse();
            foreach (var player in players) {
                newIcon = Instantiate(PlayerIconPrefab);
                newIcon.transform.SetParent(PlayerIconContainer.transform, false);
                newIcon.GetComponent<PlayerIcon>().Init(player);
                newIcon.GetComponent<UIButton>().Bt.onClick.AddListener(() => OnSelectPlayerProfile(player.Uuid));
            }
        }

        void RefreshUI()
        {
            // highlight selected profile
            ProfileCommandsContainer.SetActive(SelectedPlayerId != "");
            foreach (Transform t in PlayerIconContainer.transform) {
                t.GetComponent<PlayerIcon>().Select(SelectedPlayerId);
            }
        }

        public void OnSelectPlayerProfile(string uuid)
        {
            //Debug.Log("OnSelectPlayerProfile " + uuid);
            if (SelectedPlayerId != uuid) {
                SelectedPlayerId = uuid;
            } else {
                SelectedPlayerId = "";
            }
            RefreshUI();
        }

        public void OnOpenSelectedPlayerProfile()
        {
            //Debug.Log("OPEN " + SelectedPlayerId);
            AppManager.I.PlayerProfileManager.SetPlayerAsCurrentByUUID(SelectedPlayerId);
            AppManager.I.NavigationManager.GoToPlayerBook();
        }

        public void OnDeleteSelectPlayerProfile()
        {
            GlobalUI.ShowPrompt(true, "ATTENTION\nReally delete this player?", DoDeleteSelectPlayerProfile);
        }

        void DoDeleteSelectPlayerProfile()
        {
            //Debug.Log("DELETE " + SelectedPlayerId);
            AppManager.I.PlayerProfileManager.DeletePlayerProfile(SelectedPlayerId);
            ResetAll();
        }

        public void OnExportSelectPlayerProfile()
        {
            Debug.Log("EXPORT " + SelectedPlayerId);
        }

        public void OnCreateDemoPlayer()
        {
            GlobalUI.ShowPrompt(true, "ATTENTION\nReally create a Full Unlocked Demo player?", DoCreateDemoPlayer);
        }

        void DoCreateDemoPlayer()
        {
            //Debug.Log("creating DEMO USER ");
            var demoUserUiid = AppManager.I.PlayerProfileManager.CreatePlayerProfile(10, PlayerGender.F, 1, PlayerTint.Red, true);
            SelectedPlayerId = demoUserUiid;
            AppManager.I.PlayerProfileManager.SetPlayerAsCurrentByUUID(SelectedPlayerId);
            PopupateDemouser();
            ResetAll();
        }

        public void OnImportProfile()
        {
            Debug.Log("IMPORT");
        }

        #region Demo User Helpers

        void PopupateDemouser()
        {
            activateWaitingScreen(true);
            Debug.Log("Cheat Mode enabled: unlocking all game data");
            var maxJourneyPos = AppManager.I.Teacher.journeyHelper.GetFinalJourneyPosition();
            AppManager.I.Player.SetMaxJourneyPosition(maxJourneyPos, true);
            StartCoroutine(PopulateDatabaseWithUsefulDataCO(maxJourneyPos));
            Rewards.RewardSystemManager.UnlockAllRewards();
            activateWaitingScreen(false);
        }

        void activateWaitingScreen(bool status)
        {
            pleaseWaitPanel.gameObject.SetActive(status);
            GlobalUI.I.BackButton.gameObject.SetActive(!status);
        }

        IEnumerator PopulateDatabaseWithUsefulDataCO(JourneyPosition targetPosition)
        {

            var logAi = AppManager.I.Teacher.logAI;
            var fakeAppSession = LogManager.I.AppSession;

            // Add some mood data
            int nMoodData = 15;
            for (int i = 0; i < nMoodData; i++) {
                logAi.LogMood("XXX", Random.Range(AppConstants.minimumMoodValue, AppConstants.maximumMoodValue + 1));
                Debug.Log("Add mood " + i);
                yield return null;
            }

            // Add scores for all play sessions
            var allPlaySessionInfos = AppManager.I.Teacher.scoreHelper.GetAllPlaySessionInfo();
            for (int i = 0; i < allPlaySessionInfos.Count; i++) {
                if (allPlaySessionInfos[i].data.Stage <= targetPosition.Stage) {
                    logAi.LogPlaySessionScore("XXX", allPlaySessionInfos[i].data.GetJourneyPosition(), Random.Range(1, 4), 12f);
                    Debug.Log("Add play session score for " + allPlaySessionInfos[i].data.Id);
                    yield return null;
                }
            }

            // Add scores for all minigames
            var allMiniGameInfo = AppManager.I.Teacher.scoreHelper.GetAllMiniGameInfo();
            for (int i = 0; i < allMiniGameInfo.Count; i++) {
                logAi.LogMiniGameScore("XXX", JourneyPosition.InitialJourneyPosition, allMiniGameInfo[i].data.Code, Random.Range(1, 4), 12f);
                Debug.Log("Add minigame score " + i);
                yield return null;
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
                    logAi.LogLearn(fakeAppSession, maxPlaySession, MiniGameCode.Assessment_LetterForm, resultsList);
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
                    logAi.LogLearn(fakeAppSession, maxPlaySession, MiniGameCode.Assessment_LetterForm, resultsList);
                }
            }*/

        }

        #endregion
    }
}