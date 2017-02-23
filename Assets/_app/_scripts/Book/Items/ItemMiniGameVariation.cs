using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Database;
using EA4S.UI;

namespace EA4S.Book
{

    /// <summary>
    /// Displays a MiniGame item in the MiniGames panel of the Player Book.
    /// </summary>
    public class ItemMiniGameVariation : MonoBehaviour, IPointerClickHandler
    {
        MiniGameInfo miniGameInfo;

        //public Image Icon;
        public Image BadgeIcon;
        public Image LockIcon;

        GamesPanel panelManager;

        public void Init(GamesPanel _manager, MiniGameInfo _MiniGameInfo)
        {
            miniGameInfo = _MiniGameInfo;
            panelManager = _manager;

            if (miniGameInfo.unlocked || AppManager.I.Player.IsDemoUser) {
                LockIcon.enabled = false;
            } else {
                LockIcon.enabled = true;
            }

            ////Title.text = data.Title_Ar;

            //var icoPath = miniGameInfo.data.GetIconResourcePath();
            //Debug.Log("resource icon for " + miniGameInfo.data.GetId() + ":" + icoPath);
            var badgePath = miniGameInfo.data.GetBadgeIconResourcePath();

            //// @note: we get the minigame saved score, which should be the maximum score achieved
            //// @note: I'm leaving the average-based method commented if we want to return to that logic
            var score = miniGameInfo.score;
            //var score = GenericHelper.GetAverage(TeacherAI.I.scoreHelper.GetLatestScoresForMiniGame(miniGameInfo.data.Code, -1));

            if (score < 0.1f) {
                // disabled
                GetComponent<Button>().interactable = false;
                //GetComponent<Image>().color = Color.grey;
            }

            //Icon.sprite = Resources.Load<Sprite>(icoPath);
            if (badgePath != "") {
                BadgeIcon.sprite = Resources.Load<Sprite>(badgePath);
            }

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            panelManager.DetailMiniGame(miniGameInfo);
        }

    }
}