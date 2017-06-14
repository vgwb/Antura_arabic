using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using EA4S.Audio;
using EA4S.Core;
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

        public MiniGameCode GetFirstVariationMiniGameCode()
        {
            return variations[0].data.Code;
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
            emptyListContainers();

            var mainMiniGamesList = GetMainMiniGameList();
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
            if (!AppManager.Instance.NavigationManager.PrevSceneIsReservedArea() && (selectedGameInfo.unlocked || AppManager.Instance.Player.IsDemoUser)) {
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
            AppManager.Instance.Player.CurrentJourneyPosition.Stage = AppManager.Instance.Player.MaxJourneyPosition.Stage;
            AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = AppManager.Instance.Player.MaxJourneyPosition.LearningBlock;
            AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = AppManager.Instance.Player.MaxJourneyPosition.PlaySession;

            Debug.Log("Playing minigame " + currentMiniGame.Code + " at PS " + AppManager.Instance.Player.CurrentJourneyPosition);

            AppManager.Instance.GameLauncher.LaunchGame(currentMiniGame.Code, forceNewPlaySession: true);
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
            List<MiniGameInfo> minigameInfoList = AppManager.Instance.ScoreHelper.GetAllMiniGameInfo();
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

            // Sort minigames and variations based on their minimum journey position
            Dictionary<MiniGameCode, JourneyPosition> minimumJourneyPositions = new Dictionary<MiniGameCode, JourneyPosition>();
            foreach (var mainMiniGame in outputList) {
                foreach (var miniGameInfo in mainMiniGame.variations) {
                    var miniGameCode = miniGameInfo.data.Code;
                    minimumJourneyPositions[miniGameCode] = AppManager.Instance.JourneyHelper.GetMinimumJourneyPositionForMiniGame(miniGameCode);
                }
            }

            // First sort variations (so the first variation is in front)
            foreach (var mainMiniGame in outputList) {
                mainMiniGame.variations.Sort((g1, g2) => minimumJourneyPositions[g1.data.Code].IsMinor(
                        minimumJourneyPositions[g2.data.Code])
                        ? -1
                        : 1);
            }

            // Then sort minigames by their first variation
            outputList.Sort((g1, g2) => SortMiniGames(minimumJourneyPositions, g1, g2));

            return outputList;
        }

        private int SortMiniGames(Dictionary<MiniGameCode, JourneyPosition> minimumJourneyPositions, MainMiniGame g1, MainMiniGame g2)
        {
            // MiniGames are sorted based on minimum play session
            var minPos1 = minimumJourneyPositions[g1.GetFirstVariationMiniGameCode()];
            var minPos2 = minimumJourneyPositions[g2.GetFirstVariationMiniGameCode()];

            if (minPos1.IsMinor(minPos2)) return -1;
            if (minPos2.IsMinor(minPos1)) return 1;

            // Check play session order
            var sharedPlaySessionData = AppManager.Instance.DB.GetPlaySessionDataById(minPos1.ToStringId());
            int ret = 0;
            switch (sharedPlaySessionData.Order) {
                case PlaySessionDataOrder.Random:
                    // No specific sorting
                    ret = 0;
                    break; ;
                case PlaySessionDataOrder.Sequence:
                    // In case of a Sequence PS, two minigames with the same minimum play session are sorted based on the sequence order
                    var miniGameInPlaySession1 = sharedPlaySessionData.Minigames.ToList().Find(x => x.MiniGameCode == g1.GetFirstVariationMiniGameCode());
                    var miniGameInPlaySession2 = sharedPlaySessionData.Minigames.ToList().Find(x => x.MiniGameCode == g2.GetFirstVariationMiniGameCode());
                    ret = miniGameInPlaySession1.Weight - miniGameInPlaySession2.Weight;
                    break;
            }
            return ret;
        }

    }
}