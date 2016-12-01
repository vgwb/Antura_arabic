using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Db;

namespace EA4S
{
    public class ItemLetter : MonoBehaviour, IPointerClickHandler
    {
        LetterInfo info;
        public TextRender Title;
        public TextRender SubTitle;
        public Image LockIcon;

        BookPanel manager;

        public void Init(BookPanel _manager, LetterInfo _info)
        {
            info = _info;
            manager = _manager;

            if (info.unlocked || AppManager.I.GameSettings.CheatSuperDogMode)
            {
                LockIcon.enabled = false;
            }
            else {
                LockIcon.enabled = true;
            }

            Title.text = info.data.GetChar();
            SubTitle.text = info.data.Id;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.DetailLetter(info);
        }
    }
}