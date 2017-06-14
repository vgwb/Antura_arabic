using EA4S.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Database;
using EA4S.Helpers;
using EA4S.UI;
using EA4S.Utilities;

namespace EA4S.Book
{

    /// <summary>
    /// Displays a Word item in the Dictionary page of the Player Book.
    /// </summary>
    public class ItemWord : MonoBehaviour, IPointerClickHandler
    {
        WordInfo info;
        public TextRender Title;
        public TextRender SubTitle;
        public TextRender Drawing;
        public Image LockIcon;

        VocabularyPanel manager;

        public void Init(VocabularyPanel _manager, WordInfo _info)
        {
            info = _info;
            manager = _manager;

            if (info.unlocked || (AppManager.Instance as AppManager).Player.IsDemoUser) {
                LockIcon.enabled = false;
            } else {
                LockIcon.enabled = true;
            }

            Title.text = info.data.Arabic;
            SubTitle.text = info.data.Id;

            if (info.data.Drawing != "") {
                Drawing.text = (AppManager.Instance as AppManager).VocabularyHelper.GetWordDrawing(info.data);
                if (info.data.Category == Database.WordDataCategory.Color) {
                    Drawing.SetColor(GenericHelper.GetColorFromString(info.data.Value));
                }
            } else {
                Drawing.text = "";
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.DetailWord(info);
        }
    }
}