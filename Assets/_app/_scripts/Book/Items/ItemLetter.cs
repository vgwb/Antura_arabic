using Antura.Database;
using Antura.UI;
using Antura.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Antura.Book
{
    /// <summary>
    /// Displays an Letter item in the Dictionary page of the Player Book.
    /// </summary>
    public class ItemLetter : MonoBehaviour, IPointerClickHandler
    {
        LetterInfo info;
        public TextRender Title;
        public TextRender SubTitle;
        public Image OkIcon;

        VocabularyPanel manager;

        UIButton uIButton;


        public void Init(VocabularyPanel _manager, LetterInfo _info, bool _selected)
        {
            uIButton = GetComponent<UIButton>();


            info = _info;
            manager = _manager;

            if (info.unlocked || AppManager.I.Player.IsDemoUser) {
                OkIcon.enabled = true;
            } else {
                OkIcon.enabled = false;
            }

            Title.text = info.data.GetStringForDisplay();
            SubTitle.text = info.data.Id;

            hightlight(_selected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.DetailLetter(info);
        }

        public void Select(string code)
        {
            hightlight(code == info.data.Id);
        }

        void hightlight(bool _status)
        {
            uIButton.Toggle(_status);
        }
    }
}