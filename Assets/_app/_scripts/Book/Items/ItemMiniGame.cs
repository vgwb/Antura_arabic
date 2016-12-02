using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Db;

namespace EA4S
{
    public class ItemMiniGame : MonoBehaviour, IPointerClickHandler
    {
        MiniGameInfo info;

        public TextRender Title;
        public Image Icon;
        public Image BadgeIcon;
        public Image LockIcon;

        GamesPanel manager;

        public void Init(GamesPanel _manager, MiniGameInfo _info)
        {
            info = _info;
            manager = _manager;

            if (info.unlocked || AppManager.I.GameSettings.CheatSuperDogMode) {
                LockIcon.enabled = false;
            } else {
                LockIcon.enabled = true;
            }

            //Title.text = data.Title_Ar;

            var icoPath = info.data.GetIconResourcePath();
            var badgePath = info.data.GetBadgeIconResourcePath();

            // @note: we get the minigame saved score, which should be the maximum score achieved
            // @note: I'm leaving the average-based method commented if we want to return to that logic
            var score = info.score;
            //var score = GenericUtilities.GetAverage(TeacherAI.I.scoreHelper.GetLatestScoresForMiniGame(info.data.Code, -1));

            if (score < 0.1f) {
                // disabled
                //GetComponent<Button>().interactable = false;
                //GetComponent<Image>().color = Color.grey;
            }

            Icon.sprite = Resources.Load<Sprite>(icoPath);
            if (badgePath != "") {
                BadgeIcon.sprite = Resources.Load<Sprite>(badgePath);
            }

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.DetailMiniGame(info);
        }
    }
}