using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S
{

    public class GamesPanel : MonoBehaviour
    {

        [Header("Prefabs")]
        public GameObject MinigameItemPrefab;

        [Header("References")]
        public GameObject ElementsContainer;
        public TextRender ArabicText;
        public TextRender ScoreText;
        public Image MiniGameLogoImage;
        public Image MiniGameBadgeImage;
        public Button LaunchGameButton;

        GameObject btnGO;
        PlayerBookPanel currentArea = PlayerBookPanel.None;
        MiniGameData currentMiniGame;

        void Start()
        {
        }

        void OnEnable()
        {
            OpenArea(PlayerBookPanel.MiniGames);
        }

        void OpenArea(PlayerBookPanel newArea)
        {
            currentArea = newArea;
            activatePanel(currentArea, true);
        }

        void activatePanel(PlayerBookPanel panel, bool status)
        {
            switch (panel) {

                case PlayerBookPanel.MiniGames:
                    //AudioManager.I.PlayDialog("Book_Games");
                    MinigamesPanel();
                    break;
            }
        }

        void MinigamesPanel()
        {
            emptyListContainers();

            var minigame_list = AppManager.I.DB.GetActiveMinigames();

            List<MiniGameInfo> info_list = AppManager.I.Teacher.scoreHelper.GetAllMiniGameInfo();
            foreach (var item_info in info_list) {
                if (minigame_list.Contains(item_info.data)) {
                    btnGO = Instantiate(MinigameItemPrefab);
                    btnGO.transform.SetParent(ElementsContainer.transform, false);
                    btnGO.GetComponent<ItemMiniGame>().Init(this, item_info);
                }
            }

            DetailMiniGame(null);
        }


        public void DetailMiniGame(MiniGameInfo info)
        {
            if (info == null)
            {
                currentMiniGame = null;
                ScoreText.text = "";
                MiniGameLogoImage.enabled = false;
                MiniGameBadgeImage.enabled = false;
                LaunchGameButton.gameObject.SetActive(false);
                return;
            }

            currentMiniGame = info.data;
            AudioManager.I.PlayDialog(info.data.GetTitleSoundFilename());

            var Output = "";
            Output += "Score: " + info.score;
            Output += "\nPlayed: ";
            ScoreText.text = Output;

            // Launch button
            if (info.unlocked || AppManager.I.GameSettings.CheatSuperDogMode)
            {
                LaunchGameButton.gameObject.SetActive(true);
                LaunchGameButton.interactable = true;
            }
            else
            {
                LaunchGameButton.gameObject.SetActive(false);
                LaunchGameButton.interactable = false;
            }

            // Set icon
            var icoPath = currentMiniGame.GetIconResourcePath();
            var badgePath = currentMiniGame.GetBadgeIconResourcePath();
            MiniGameLogoImage.sprite = Resources.Load<Sprite>(icoPath);
            MiniGameLogoImage.enabled = true;
            if (badgePath != "")
            {
                MiniGameBadgeImage.enabled = true;
                MiniGameBadgeImage.sprite = Resources.Load<Sprite>(badgePath);
            } else
            {
                MiniGameBadgeImage.enabled = false;
            }

        }

        public void OnLaunchMinigame()
        {
            // Set to max stage
            AppManager.I.Player.CurrentJourneyPosition.Stage = AppManager.I.Player.MaxJourneyPosition.Stage;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = AppManager.I.Player.MaxJourneyPosition.PlaySession;

            Debug.Log("Playing minigame " + currentMiniGame.Code + " at PS " + AppManager.I.Player.CurrentJourneyPosition);

            AppManager.I.Teacher.InitialiseCurrentPlaySession(chooseMiniGames:false); // We must force this or the teacher won't use the correct data
            AppManager.I.GameLauncher.LaunchGame(currentMiniGame.Code);
        }

        void emptyListContainers()
        {
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }

        }

    }
}