using System.Collections.Generic;
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

        public Toggle TutorialEnabledToggle;
        public Toggle VerboseTeacherToggle;
        public Toggle SafeLaunchToggle;
        public Toggle AutoCorrectJourneyPosToggle;
        public Toggle CheatEnabledToggle;

        public bool TutorialEnabled { get { return DebugManager.I.TutorialEnabled; } set { DebugManager.I.TutorialEnabled = value; } }
        public bool VerboseTeacher { get { return DebugManager.I.VerboseTeacher; } set { DebugManager.I.VerboseTeacher = value; } }
        public bool SafeLaunch { get { return DebugManager.I.SafeLaunch; } set { DebugManager.I.SafeLaunch = value; } }
        public bool AutoCorrectJourneyPos { get { return DebugManager.I.AutoCorrectJourneyPos; } set { DebugManager.I.AutoCorrectJourneyPos = value; } }
        public bool CheatEnabled { get { return DebugManager.I.CheatEnabled; } set { DebugManager.I.CheatEnabled = value; } }

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

        #region Open / Close

        public void OnClickOpen()
        {
            clickCounter++;
            if (clickCounter >= 3) {
                Open();
            }
        }

        public void OnClickClose()
        {
            Close();
        }

        #endregion

        #region Launching

        public void ResetPlayTest()
        {
            playedMinigames.Clear();
            BuildUI();
        }

        #endregion

        #region Navigation

        public void GoToHome()
        {
            DebugManager.I.GoToHome();
            Close();
        }

        public void GoToMap()
        {
            DebugManager.I.GoToMap();
            Close();
        }

        public void GoToNext()
        {
            DebugManager.I.GoToNext();
            Close();
        }

        public void GoToEnd()
        {
            DebugManager.I.GoToEnd();
            Close();
        }

        public void GoToReservedArea()
        {
            //WidgetPopupWindow.I.Close();
            DebugManager.I.GoToReservedArea();
            Close();
        }

        #endregion

        #region Profiles

        public void ResetAll()
        {
            Close();
            DebugManager.I.ResetAll();
        }

        public void OnCreateTestProfile()
        {
            DebugManager.I.CreateTestProfile();
            Close();
        }

        public bool FirstContactPassed { get { return DebugManager.I.FirstContactPassed; } set { DebugManager.I.FirstContactPassed = value; } }

        #endregion

        public void OnReportBug()
        {
            AppManager.I.OpenSupportForm();
        }

        #endregion

        #region Internal

        private void Open()
        {
            BuildUI();
            Panel.SetActive(true);
            DebugManager.I.DebugPanelOpened = true;
        }

        private void Close()
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

            TutorialEnabledToggle.isOn = TutorialEnabled;
            CheatEnabledToggle.isOn = CheatEnabled;
            AutoCorrectJourneyPosToggle.isOn = AutoCorrectJourneyPos;
            VerboseTeacherToggle.isOn = VerboseTeacher;
            SafeLaunchToggle.isOn = SafeLaunch;

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

            var debugPs = new JourneyPosition(int.Parse(InputStage.text), int.Parse(InputLearningBlock.text), int.Parse(InputPlaySession.text));

            if (!DebugManager.I.SafeLaunch || AppManager.I.Teacher.CanMiniGameBePlayedAfterMinPlaySession(debugPs, minigameCode))
            {
                LaunchMiniGameAtJourneyPosition(minigameCode, difficulty, debugPs);

            } else {
                if (DebugManager.I.SafeLaunch)
                {
                    JourneyPosition minJ = AppManager.I.JourneyHelper.GetMinimumJourneyPositionForMiniGame(minigameCode);
                    if (minJ == null) {
                        Debug.LogWarningFormat(
                            "Minigame {0} could not be selected for any PlaySession. Please check the PlaySession data table.",
                            minigameCode);
                    } else {
                        Debug.LogErrorFormat("Minigame {0} cannot be selected this PlaySession. Min: {1}", minigameCode,
                            minJ.ToString());

                        if (AutoCorrectJourneyPos)
                        {
                            LaunchMiniGameAtJourneyPosition(minigameCode, difficulty, minJ);
                        }
                    }
                }
            }
        }

        private void LaunchMiniGameAtJourneyPosition(MiniGameCode minigameCode, float difficulty, JourneyPosition journeyPosition)
        {
            WidgetPopupWindow.I.Close();
            DebugManager.I.SetDebugJourneyPos(journeyPosition);
            DebugManager.I.LaunchMiniGame(minigameCode, difficulty);
            Close();
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