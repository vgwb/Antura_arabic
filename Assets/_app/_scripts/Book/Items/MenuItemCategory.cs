using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Db;

namespace EA4S
{
    public class MenuItemCategory : MonoBehaviour, IPointerClickHandler
    {
        GenericCategoryData data;
        public TextRender Title;
        public TextRender SubTitle;
        BookPanel manager;

        public void Init(BookPanel _manager, GenericCategoryData _data)
        {
            data = _data;
            manager = _manager;

            Title.text = data.Title;
            SubTitle.text = data.Id;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.SelectSubCategory(data);
        }
    }
}