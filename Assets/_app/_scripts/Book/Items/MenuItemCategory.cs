using EA4S.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EA4S.PlayerBook
{

    /// <summary>
    /// Displays a category button in the PlayerBook. Used to select a page of the book.
    /// </summary>
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