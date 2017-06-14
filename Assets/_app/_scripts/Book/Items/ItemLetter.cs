using EA4S.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Database;
using EA4S.UI;

namespace EA4S.Book
{
    /// <summary>
    /// Displays an Letter item in the Dictionary page of the Player Book.
    /// </summary>
    public class ItemLetter : MonoBehaviour, IPointerClickHandler
    {
        LetterInfo info;
        public TextRender Title;
        public TextRender SubTitle;
        public Image LockIcon;

        VocabularyPanel manager;

        UIButton uIButton;


        public void Init(VocabularyPanel _manager, LetterInfo _info, bool _selected)
        {
            uIButton = GetComponent<UIButton>();


            info = _info;
            manager = _manager;

            if (info.unlocked || AppManager.Instance.Player.IsDemoUser) {
                LockIcon.enabled = false;
            } else {
                LockIcon.enabled = true;
            }

            Title.text = info.data.GetChar();
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