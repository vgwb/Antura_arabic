using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Database;
using EA4S.UI;

namespace EA4S.PlayerBook
{
    /// <summary>
    /// Displays a Phrase item in the Dictionary page of the Player Book.
    /// </summary>
    public class ItemPhrase : MonoBehaviour, IPointerClickHandler
    {
        PhraseInfo info;
        public TextRender Title;
        public TextRender SubTitle;
        public Image LockIcon;

        BookPanel manager;

        public void Init(BookPanel _manager, PhraseInfo _info)
        {
            info = _info;
            manager = _manager;

            if (info.unlocked || AppManager.I.Player.IsDemoUser) {
                LockIcon.enabled = false;
            } else {
                LockIcon.enabled = true;
            }

            Title.text = info.data.Arabic;
            SubTitle.text = info.data.Id;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.DetailPhrase(info);
        }
    }
}