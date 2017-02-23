using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Database;
using EA4S.UI;

namespace EA4S.PlayerBook
{

    /// <summary>
    /// Displays a MiniGame item in the MiniGames panel of the Player Book.
    /// </summary>
    public class ItemMiniGame : MonoBehaviour, IPointerClickHandler
    {
        MainMiniGame info;

        public GameObject VariationsContainer;
        public GameObject ItemMiniGameVariationPrefab;

        public TextRender Title;
        public Image Icon;
        public Image BadgeIcon;
        public Image LockIcon;

        GamesPanel manager;

        public void Init(GamesPanel _manager, MainMiniGame _MainMiniGame)
        {
            info = _MainMiniGame;
            manager = _manager;

            //if (info.unlocked || AppManager.I.Player.IsDemoUser) {
            //    LockIcon.enabled = false;
            //} else {
            //    LockIcon.enabled = true;
            //}

            ////Title.text = data.Title_Ar;

            //var icoPath = info.data.GetIconResourcePath();
            //var badgePath = info.data.GetBadgeIconResourcePath();

            //// @note: we get the minigame saved score, which should be the maximum score achieved
            //// @note: I'm leaving the average-based method commented if we want to return to that logic
            //var score = info.score;
            //var score = GenericHelper.GetAverage(TeacherAI.I.scoreHelper.GetLatestScoresForMiniGame(info.data.Code, -1));

            //if (score < 0.1f) {
            //    // disabled
            //    //GetComponent<Button>().interactable = false;
            //    //GetComponent<Image>().color = Color.grey;
            //}

            //Icon.sprite = Resources.Load<Sprite>(icoPath);
            //if (badgePath != "") {
            //    BadgeIcon.sprite = Resources.Load<Sprite>(badgePath);
            //}

            emptyContainers();

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //manager.DetailMiniGame(info.variations[0].Code);
        }

        void emptyContainers()
        {
            foreach (Transform t in VariationsContainer.transform) {
                Destroy(t.gameObject);
            }

        }
    }
}