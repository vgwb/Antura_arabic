using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using EA4S.Core;
using EA4S.UI;

namespace EA4S.Debugging
{
    public enum DebugButtonAction
    {
        MiniGame,
        Reset
    }

    public class DebugPanel : MonoBehaviour
    {
        public static DebugPanel I;

        [Header("References")] public GameObject Panel;
        public GameObject Container;
        public GameObject PrefabRow;
        public GameObject PrefabButton;

        private int clickCounter;
        private GameObject btnGO;
        private Dictionary<MiniGameCode, bool> playedMinigames = new Dictionary<MiniGameCode, bool>();

        void Awake()
        {
            if (I != null)
            {
                Destroy(gameObject);
            }
            else
            {
                I = this;
                DontDestroyOnLoad(gameObject);
            }

            if (Panel.activeSelf)
            {
                Panel.SetActive(false);
            }
        }

        public void OnClickOpen()
        {
            clickCounter++;
            if (clickCounter >= 3)
            {
                open();
            }
        }

        public void OnClickClose()
        {
            close();
        }

        private void open()
        {
            buildUI();
            Panel.SetActive(true);
        }

        private void close()
        {
            clickCounter = 0;
            Panel.SetActive(false);
        }

        public void Reset()
        {
            playedMinigames.Clear();
            buildUI();
        }

        private void buildUI()
        {
            var mainMiniGamesList = MiniGamesUtilities.GetMainMiniGameList();

            emptyContainer(Container);

            var newRow = Instantiate(PrefabRow);
            newRow.transform.SetParent(Container.transform, false);
            btnGO = Instantiate(PrefabButton);
            btnGO.transform.SetParent(newRow.transform, false);
            btnGO.GetComponent<DebugButton>().Init(this, DebugButtonAction.Reset, "Reset Playtest");

            foreach (var mainMiniGame in mainMiniGamesList)
            {
                newRow = Instantiate(PrefabRow);
                newRow.transform.SetParent(Container.transform, false);

                foreach (var gameVariation in mainMiniGame.variations)
                {
                    btnGO = Instantiate(PrefabButton);
                    btnGO.transform.SetParent(newRow.transform, false);
                    bool gamePlayed;
                    playedMinigames.TryGetValue(gameVariation.data.Code, out gamePlayed);
                    btnGO.GetComponent<DebugButton>().Init(this, DebugButtonAction.MiniGame, gameVariation, gamePlayed);
                }
            }
        }

        public void LaunchMinigame(MiniGameCode minigameCode)
        {
            playedMinigames[minigameCode] = true;
            DebugManager.I.Stage = AppManager.I.Player.CurrentJourneyPosition.Stage;
            DebugManager.I.LearningBlock = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
            DebugManager.I.PlaySession = AppManager.I.Player.CurrentJourneyPosition.PlaySession;

            if (!AppConstants.DebugStopPlayAtWrongPlaySessions
                || AppManager.I.Teacher.CanMiniGameBePlayedAfterMinPlaySession(
                    new JourneyPosition(DebugManager.I.Stage, DebugManager.I.LearningBlock, DebugManager.I.PlaySession),
                    minigameCode))
            {
                WidgetPopupWindow.I.Close();
                DebugManager.I.LaunchMiniGame(minigameCode);
                close();
            }
            else
            {
                if (AppConstants.DebugStopPlayAtWrongPlaySessions)
                {
                    JourneyPosition minJ =
                        AppManager.I.JourneyHelper.GetMinimumJourneyPositionForMiniGame(minigameCode);
                    if (minJ == null)
                    {
                        Debug.LogWarningFormat(
                            "Minigame {0} could not be selected for any PlaySession. Please check the PlaySession data table.",
                            minigameCode);
                    }
                    else
                    {
                        Debug.LogErrorFormat("Minigame {0} cannot be selected this PlaySession. Min: {1}", minigameCode,
                            minJ.ToString());
                    }
                }
            }
        }

        public void GoHome()
        {
            // refactor: move to DebugManager
            WidgetPopupWindow.I.Close();
            AppManager.I.NavigationManager.GoToHome(debugMode: true);
            close();
        }

        public void AlphabetSong()
        {
            LaunchMinigame(MiniGameCode.AlphabetSong_alphabet);
        }

        void emptyContainer(GameObject container)
        {
            foreach (Transform t in container.transform)
            {
                Destroy(t.gameObject);
            }
        }
    }
}