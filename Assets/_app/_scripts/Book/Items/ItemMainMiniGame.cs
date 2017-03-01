using UnityEngine;
using UnityEngine.UI;
using EA4S.Database;
using EA4S.UI;

namespace EA4S.Book
{

    /// <summary>
    /// Displays a MiniGame item in the MiniGames panel of the Player Book.
    /// </summary>
    public class ItemMainMiniGame : MonoBehaviour
    {
        MainMiniGame mainGameInfo;

        public GameObject VariationsContainer;
        public GameObject ItemMiniGameVariationPrefab;

        public Image BackgroundImage;
        public TextRender Title;
        public Image Icon;
        public Image BadgeIcon;
        public Image LockIcon;

        bool isSelected;
        GamesPanel panelManager;
        GameObject btnGO;

        public void Init(GamesPanel _manager, MainMiniGame _MainMiniGame)
        {
            mainGameInfo = _MainMiniGame;
            panelManager = _manager;

            //if (info.unlocked || AppManager.I.Player.IsDemoUser) {
            //    LockIcon.enabled = false;
            //} else {
            //    LockIcon.enabled = true;
            //}

            ////Title.text = data.Title_Ar;

            var icoPath = mainGameInfo.GetIconResourcePath();
            //var badgePath = info.data.GetBadgeIconResourcePath();

            //// @note: we get the minigame saved score, which should be the maximum score achieved
            //// @note: I'm leaving the average-based method commented if we want to return to that logic
            //var score = info.score;
            //var score = GenericHelper.GetAverage(TeacherAI.I.ScoreHelper.GetLatestScoresForMiniGame(info.data.Code, -1));

            //if (score < 0.1f) {
            //    // disabled
            //    //GetComponent<Button>().interactable = false;
            //    //GetComponent<Image>().color = Color.grey;
            //}

            Icon.sprite = Resources.Load<Sprite>(icoPath);
            //if (badgePath != "") {
            //    BadgeIcon.sprite = Resources.Load<Sprite>(badgePath);
            //}

            emptyContainers();
            var isLocked = false;
            foreach (var gameVariation in mainGameInfo.variations) {
                btnGO = Instantiate(ItemMiniGameVariationPrefab);
                btnGO.transform.SetParent(VariationsContainer.transform, false);
                btnGO.GetComponent<ItemMiniGameVariation>().Init(this, gameVariation);
                if (!gameVariation.unlocked) {
                    isLocked = true;
                }
            }
            LockIcon.gameObject.SetActive(isLocked);
        }

        public void OnClicked()
        {
            DetailMiniGame(mainGameInfo.variations[0]);
        }

        public void DetailMiniGame(MiniGameInfo miniGameInfo)
        {
            panelManager.DetailMiniGame(miniGameInfo);
        }

        public void Select(MiniGameInfo gameInfo = null)
        {
            if (gameInfo != null) {
                isSelected = (gameInfo.data.Main == mainGameInfo.id);
            } else {
                isSelected = false;
            }
            hightlight(isSelected);
        }

        void hightlight(bool _status)
        {
            if (_status) {
                BackgroundImage.color = Color.yellow;
            } else {
                BackgroundImage.color = Color.white;
            }
        }

        void emptyContainers()
        {
            foreach (Transform t in VariationsContainer.transform) {
                Destroy(t.gameObject);
            }

        }
    }
}