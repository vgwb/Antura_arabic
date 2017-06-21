using UnityEngine;
using UnityEngine.UI;
using EA4S.Audio;
using EA4S.Core;
using EA4S.Database;
using EA4S.UI;

namespace EA4S.Book
{
    /// <summary>
    /// Displays information on minigames that the player has unlocked.
    /// </summary>
    public class GamesPanel : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject MinigameItemPrefab;

        [Header("References")]
        public GameObject DetailPanel;
        public GameObject ElementsContainer;
        public TextRender ArabicText;
        public TextRender EnglishText;
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
            emptyContainer(ElementsContainer);

            var mainMiniGamesList = MiniGamesUtilities.GetMainMiniGameList();
            foreach (var game in mainMiniGamesList) {
                btnGO = Instantiate(MinigameItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponent<ItemMainMiniGame>().Init(this, game);
            }
            DetailMiniGame(null);
        }

        public void DetailMiniGame(MiniGameInfo selectedGameInfo)
        {
            //foreach (Transform t in ElementsContainer.transform) {
            //    t.GetComponent<ItemMainMiniGame>().Select(selectedGameInfo);
            //}

            if (selectedGameInfo == null) {
                currentMiniGame = null;
                ArabicText.text = "";
                EnglishText.text = "";
                ScoreText.text = "";
                MiniGameLogoImage.enabled = false;
                MiniGameBadgeImage.enabled = false;
                LaunchGameButton.gameObject.SetActive(false);
                DetailPanel.SetActive(false);
                return;
            }
            DetailPanel.SetActive(true);
            currentMiniGame = selectedGameInfo.data;
            ElementsContainer.BroadcastMessage("Select", selectedGameInfo, SendMessageOptions.DontRequireReceiver);
            AudioManager.I.PlayDialogue(selectedGameInfo.data.GetTitleSoundFilename());

            ArabicText.text = selectedGameInfo.data.Title_Ar;
            EnglishText.text = selectedGameInfo.data.Title_En;

            //var Output = "";
            //Output += "Score: " + selectedGameInfo.score;
            //Output += "\nPlayed: ";
            //ScoreText.text = Output;

            // Launch button
            if (!AppManager.I.NavigationManager.PrevSceneIsReservedArea() && (selectedGameInfo.unlocked || AppManager.I.Player.IsDemoUser)) {
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

        void emptyContainer(GameObject container)
        {
            foreach (Transform t in container.transform) {
                Destroy(t.gameObject);
            }
        }
    }
}