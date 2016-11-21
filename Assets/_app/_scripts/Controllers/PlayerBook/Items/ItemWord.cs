using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Db;

namespace EA4S
{
    public class ItemWord : MonoBehaviour, IPointerClickHandler
    {
        WordData data;
        public TextRender Title;
        public TextRender SubTitle;
        public TextRender Drawing;

        BookPanel manager;

        public void Init(BookPanel _manager, WordData _data)
        {
            data = _data;
            manager = _manager;

            Title.text = data.Arabic;
            SubTitle.text = data.Id;

            if (data.Drawing != "") {
                Drawing.text = AppManager.Instance.Teacher.wordHelper.GetWordDrawing(data);
                if (data.Category == Db.WordDataCategory.Color) {
                    Drawing.SetColor(GenericUtilities.GetColorFromString(data.Value));
                }
                //GetComponent<Image>().color = Color.green;
            } else {
                Drawing.text = "";
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.DetailWord(data);
        }
    }
}