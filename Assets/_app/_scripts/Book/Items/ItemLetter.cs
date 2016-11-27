using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Db;

namespace EA4S
{
    public class ItemLetter : MonoBehaviour, IPointerClickHandler
    {
        LetterData data;
        public TextRender Title;
        public TextRender SubTitle;

        BookPanel manager;

        public void Init(BookPanel _manager, LetterData _data)
        {
            data = _data;
            manager = _manager;

            Title.text = data.GetChar();
            SubTitle.text = data.Id;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.DetailLetter(data);
        }
    }
}