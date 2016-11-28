using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Db;

namespace EA4S
{
    public class ItemWord : MonoBehaviour, IPointerClickHandler
    {
        WordInfo info;
        public TextRender Title;
        public TextRender SubTitle;
        public TextRender Drawing;

        BookPanel manager;

        public void Init(BookPanel _manager, WordInfo _info)
        {
            info = _info;
            manager = _manager;

            // @todo: show score
            //info.score

            if (!info.unlocked)
            {
                GetComponent<Button>().interactable = false;
            }
            else
            {
                GetComponent<Button>().interactable = true;
            }

            Title.text = info.data.Arabic;
            SubTitle.text = info.data.Id;

            if (info.data.Drawing != "") {
                Drawing.text = AppManager.I.Teacher.wordHelper.GetWordDrawing(info.data);
                if (info.data.Category == Db.WordDataCategory.Color) {
                    Drawing.SetColor(GenericUtilities.GetColorFromString(info.data.Value));
                }
                //GetComponent<Image>().color = Color.green;
            } else {
                Drawing.text = "";
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (info.unlocked)
            {
                manager.DetailWord(info);
            }
        }
    }
}