using Antura.Core;
using Antura.Database;
using Antura.UI;
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
        public TextRender Title;
        public TextRender SubTitle;
        public Image OkIcon;

        private LettersPage myManager;
        private LetterInfo myLetterInfo;
        private UIButton uIButton;

        public void Init(LettersPage _manager, LetterInfo _info, bool _selected)
        {
            myLetterInfo = _info;
            myManager = _manager;
            uIButton = GetComponent<UIButton>();

            if (myLetterInfo.unlocked || AppManager.I.Player.IsDemoUser) {
                OkIcon.enabled = true;
            } else {
                OkIcon.enabled = false;
            }

            Title.text = myLetterInfo.data.GetStringForDisplay();
            SubTitle.text = myLetterInfo.data.Id;

            hightlight(_selected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            myManager.DetailLetter(myLetterInfo);
        }

        public void Select(string code)
        {

            hightlight(code == myLetterInfo.data.Id);
        }

        private void hightlight(bool _status)
        {
            uIButton.Toggle(_status);
        }
    }
}