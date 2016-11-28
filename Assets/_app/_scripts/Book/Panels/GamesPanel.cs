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

        GameObject btnGO;
        PlayerBookPanel currentArea = PlayerBookPanel.None;

        void Start()
        {
        }

        void OnEnable()
        {
            OpenArea(PlayerBookPanel.MiniGames);
        }

        void OpenArea(PlayerBookPanel newArea)
        {
            if (newArea != currentArea) {
                currentArea = newArea;
                activatePanel(currentArea, true);
            }
        }

        void activatePanel(PlayerBookPanel panel, bool status)
        {
            switch (panel) {

                case PlayerBookPanel.MiniGames:
                    AudioManager.I.PlayDialog("Book_Games");
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
                if (minigame_list.Contains(item_info.data))
                {
                    btnGO = Instantiate(MinigameItemPrefab);
                    btnGO.transform.SetParent(ElementsContainer.transform, false);
                    btnGO.GetComponent<ItemMiniGame>().Init(this, item_info);
                }
            }
        }


        public void DetailMiniGame(MiniGameInfo info)
        {
            AudioManager.I.PlayDialog(info.data.GetTitleSoundFilename());
        }

        void emptyListContainers()
        {
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }

        }

    }
}