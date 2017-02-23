using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using EA4S.Audio;
using EA4S.Database;
using EA4S.UI;

namespace EA4S.Book
{
    public class MainMiniGame
    {
        public string id;
        public List<MiniGameInfo> variations;

        public string GetIconResourcePath()
        {
            return variations[0].data.GetIconResourcePath();
        }
    }

    /// <summary>
    /// Displays information on minigames that the player has unlocked.
    /// </summary>
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
        BookArea currentArea = BookArea.None;
        MiniGameData currentMiniGame;

        void Start()
        {
        }

        void OnEnable()
        {
            OpenArea(BookArea.MiniGames);
        }

        void OpenArea(BookArea newArea)
        {
            currentArea = newArea;
            activatePanel(currentArea, true);
        }

        void activatePanel(BookArea panel, bool status)
        {
            switch (panel) {

                case BookArea.MiniGames:
                    //AudioManager.I.PlayDialog("Book_Games");
                    MinigamesPanel();
                    break;
            }
        }

        void MinigamesPanel()
        {
            emptyListContainers();

            var MainGameList = GetMainMiniGameList();
            foreach (var game in MainGameList) {
                btnGO = Instantiate(MinigameItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponent<ItemMainMiniGame>().Init(this, game);

            }
            DetailMiniGame(null);
        }


        public void DetailMiniGame(MiniGameInfo info)
        {
            if (info == null) {
                currentMiniGame = null;
                ScoreText.text = "";
                MiniGameLogoImage.enabled = false;
                MiniGameBadgeImage.enabled = false;
                LaunchGameButton.gameObject.SetActive(false);
                return;
            }

            currentMiniGame = info.data;
            AudioManager.I.PlayDialogue(info.data.GetTitleSoundFilename());

            var Output = "";
            Output += "Score: " + info.score;
            Output += "\nPlayed: ";
            ScoreText.text = Output;

            // Launch button
            if (info.unlocked || AppManager.I.Player.IsDemoUser) {
                LaunchGameButton.gameObject.SetActive(true);
                LaunchGameButton.interactable = true;
            } else {
                LaunchGameButton.gameObject.SetActive(false);
                LaunchGameButton.interactable = false;
            }

            // Set icon
            var icoPath = currentMiniGame.GetIconResourcePath();
            var badgePath = currentMiniGame.GetBadgeIconResourcePath();
            MiniGameLogoImage.sprite = Resources.Load<Sprite>(icoPath);
            MiniGameLogoImage.enabled = true;
            if (badgePath != "") {
                MiniGameBadgeImage.enabled = true;
                MiniGameBadgeImage.sprite = Resources.Load<Sprite>(badgePath);
            } else {
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

            AppManager.I.GameLauncher.LaunchGame(currentMiniGame.Code, forceNewPlaySession: true);
        }

        void emptyListContainers()
        {
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }

        }

        List<MainMiniGame> GetMainMiniGameList()
        {
            Dictionary<string, MainMiniGame> dictionary = new Dictionary<string, MainMiniGame>();
            List<MiniGameInfo> minigameInfoList = AppManager.I.Teacher.scoreHelper.GetAllMiniGameInfo();
            foreach (var minigameInfo in minigameInfoList) {
                if (!dictionary.ContainsKey(minigameInfo.data.Main)) {
                    dictionary[minigameInfo.data.Main] = new MainMiniGame {
                        id = minigameInfo.data.Main,
                        variations = new List<MiniGameInfo>()
                    };
                }
                dictionary[minigameInfo.data.Main].variations.Add(minigameInfo);
            }

            List<MainMiniGame> outputList = new List<MainMiniGame>();
            foreach (var k in dictionary.Keys) {
                if (dictionary[k].id != "Assessment") {
                    outputList.Add(dictionary[k]);
                }
            }

            return outputList;
        }

    }
}