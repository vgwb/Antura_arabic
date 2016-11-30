using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Db;

namespace EA4S
{
    public class ItemPhrase : MonoBehaviour, IPointerClickHandler
    {
        PhraseInfo info;
        public TextRender Title;
        public TextRender SubTitle;

        BookPanel manager;

        public void Init(BookPanel _manager, PhraseInfo _info)
        {
            info = _info;
            manager = _manager;

            if (info.unlocked || AppManager.I.GameSettings.CheatSuperDogMode) {
                GetComponent<Button>().interactable = true;
            } else {
                GetComponent<Button>().interactable = false;
            }

            Title.text = info.data.Arabic;
            SubTitle.text = info.data.Id;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (info.unlocked || AppManager.I.GameSettings.CheatSuperDogMode) {
                manager.DetailPhrase(info);
            }
        }
    }
}