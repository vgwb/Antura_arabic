using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using EA4S.Core;
using EA4S.UI;
using UnityEngine.UI;

namespace EA4S.Debugging
{
    public class DebugPanel : MonoBehaviour
    {
        public static DebugPanel I;

        [Header("References")] public GameObject Panel;
        public GameObject Container;
        public GameObject PrefabRow;
        public GameObject PrefabMiniGameButton;

        public InputField InputStage;
        public InputField InputLearningBlock;
        public InputField InputPlaySession;
        public Toggle ToggleTutorialEnabled;

        private int clickCounter;
        private Dictionary<MiniGameCode, bool> playedMinigames = new Dictionary<MiniGameCode, bool>();

        void Awake()
        {
            if (I != null) {
                Destroy(gameObject);
            } else {
                I = this;
                DontDestroyOnLoad(gameObject);
            }

            if (Panel.activeSelf) {
                Panel.SetActive(false);
            }
        }

        #region Buttons

        public void OnClickOpen()
        {
            clickCounter++;
            if (clickCounter >= 3) {
                open();
            }
        }

        public void OnClickClose()
        {
            close();
        }

        public void ResetPlayTest()
        {
            playedMinigames.Clear();
            BuildUI();
        }

        public void ResetAll()
        {
            close();
            DebugManager.I.ResetAll();
        }

        public void GoHome()
        {
            // refactor: move to DebugManager
            WidgetPopupWindow.I.Close();
            AppManager.I.NavigationManager.GoToHome(debugMode: true);
            close();
        }
        
        public void OnCreateTestProfile()
        {
            // refactor: move to DebugManager
            AppManager.I.PlayerProfileManager.CreatePlayerProfile(4, PlayerGender.F, 1, PlayerTint.Blue);
            AppManager.I.NavigationManager.GoToHome(debugMode: true);
            close();
        }

        public void OnReportBug()
        {
            AppManager.I.OpenSupportForm();
        }

        #endregion

        #region Internal

        private void open()
        {
            BuildUI();
            Panel.SetActive(true);
            DebugManager.I.DebugPanelOpened = true;
        }

        private void close()
        {
            clickCounter = 0;
            Panel.SetActive(false);
            DebugManager.I.DebugPanelOpened = false;
        }

        #endregion

        #region UI

        private void BuildUI()
        {
            if (AppManager.I.Player != null) {
                InputStage.text = AppManager.I.Player.CurrentJourneyPosition.Stage.ToString();
                InputLearningBlock.text = AppManager.I.Player.CurrentJourneyPosition.LearningBlock.ToString();
                InputPlaySession.text = AppManager.I.Player.CurrentJourneyPosition.PlaySession.ToString();
            }

            var mainMiniGamesList = MiniGamesUtilities.GetMainMiniGameList();
            var difficultiesForTesting = MiniGamesUtilities.GetMiniGameDifficultiesForTesting();

            EmptyContainer(Container);

            foreach (var mainMiniGame in mainMiniGamesList)
            {
                var newRow = Instantiate(PrefabRow);
                newRow.transform.SetParent(Container.transform, false);

                newRow.GetComponent<DebugMiniGameRow>().Title.text = mainMiniGame.id;

                foreach (var gameVariation in mainMiniGame.variations)
                {
                    Debug.Assert(difficultiesForTesting.ContainsKey(gameVariation.data.Code), "No difficulty for testing setup for game variation " + gameVariation.data.Code);
                    var difficulties = difficultiesForTesting[gameVariation.data.Code];

                    foreach (var difficulty in difficulties)
                    {
                        var btnGO = Instantiate(PrefabMiniGameButton);
                        btnGO.transform.SetParent(newRow.transform, false);
                        bool gamePlayed;
                        playedMinigames.TryGetValue(gameVariation.data.Code, out gamePlayed);
                        btnGO.GetComponent<DebugMiniGameButton>().Init(this, gameVariation, gamePlayed, difficulty);
                    }
                }

            }
        }



        #endregion

        #region Actions

        public void LaunchMiniGame(MiniGameCode minigameCode, float difficulty)
        {
            playedMinigames[minigameCode] = true;
            DebugManager.I.Stage = int.Parse(InputStage.text);
            DebugManager.I.LearningBlock = int.Parse(InputLearningBlock.text);
            DebugManager.I.PlaySession = int.Parse(InputPlaySession.text);

            bool tutorialEnabled = ToggleTutorialEnabled.isOn;

            if (!AppConstants.DebugStopPlayAtWrongPlaySessions
                || AppManager.I.Teacher.CanMiniGameBePlayedAfterMinPlaySession(
                    new JourneyPosition(DebugManager.I.Stage, DebugManager.I.LearningBlock, DebugManager.I.PlaySession),
                    minigameCode)) {
                WidgetPopupWindow.I.Close();
                DebugManager.I.LaunchMiniGame(minigameCode, difficulty, tutorialEnabled);
                close();
            } else {
                if (AppConstants.DebugStopPlayAtWrongPlaySessions) {
                    JourneyPosition minJ =
                        AppManager.I.JourneyHelper.GetMinimumJourneyPositionForMiniGame(minigameCode);
                    if (minJ == null) {
                        Debug.LogWarningFormat(
                            "Minigame {0} could not be selected for any PlaySession. Please check the PlaySession data table.",
                            minigameCode);
                    } else {
                        Debug.LogErrorFormat("Minigame {0} cannot be selected this PlaySession. Min: {1}", minigameCode,
                            minJ.ToString());
                    }
                }
            }
        }

        #endregion

        #region Utilities

        private void EmptyContainer(GameObject container)
        {
            foreach (Transform t in container.transform) {
                Destroy(t.gameObject);
            }
        }

        #endregion
    }
}