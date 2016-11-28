using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Db;

namespace EA4S
{
    public class ItemMiniGame : MonoBehaviour, IPointerClickHandler
    {
        MiniGameData data;

        public TextRender Title;
        public Image Icon;
        public Image BadgeIcon;

        GamesPanel manager;

        public void Init(GamesPanel _manager, MiniGameData _data)
        {
            data = _data;
            manager = _manager;

            //Title.text = data.Title_Ar;

            var icoPath = data.GetIconResourcePath();
            var badgePath = data.GetBadgeIconResourcePath();

            var score = GenericUtilities.GetAverage(TeacherAI.I.scoreHelper.GetLatestScoresForMiniGame(data.Code, -1));

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
            manager.DetailMiniGame(data);
        }
    }
}