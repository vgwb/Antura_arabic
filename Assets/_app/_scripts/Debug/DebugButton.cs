using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Core;
using EA4S.Database;
using EA4S.UI;

namespace EA4S.Debugging
{
    public class DebugButton : MonoBehaviour, IPointerClickHandler
    {
        public TextRender Title;
        private DebugPanel manager;
        private DebugButtonAction action;
        private MiniGameInfo minigameInfo;
        private bool played;

        public void Init(DebugPanel _manager, DebugButtonAction _action, string _title)
        {
            manager = _manager;
            action = _action;
            Title.text = _title;
        }

        public void Init(DebugPanel _manager, DebugButtonAction _action, MiniGameInfo _MiniGameInfo, bool _played)
        {
            manager = _manager;
            action = _action;
            minigameInfo = _MiniGameInfo;
            Title.text = _MiniGameInfo.data.Title_En;
            played = _played;
            ColorButton();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (action) {
                case DebugButtonAction.MiniGame:
                    ColorButton();
                    manager.LaunchMinigame(minigameInfo.data.Code);
                    break;
                case DebugButtonAction.Reset:
                    manager.Reset();
                    break;
            }
        }

        void ColorButton()
        {
            // Debug.Log(Title.text + " " + played);
            var colors = GetComponent<Button>().colors;
            if (played) {
                colors.normalColor = Color.gray;
            } else {
                colors.normalColor = Color.white;
            }
            GetComponent<Button>().colors = colors;
        }
    }
}