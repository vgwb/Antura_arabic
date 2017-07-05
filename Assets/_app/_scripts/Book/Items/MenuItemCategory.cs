using Antura.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Antura.Book
{
    /// <summary>
    /// Displays a category button in the PlayerBook. Used to select a page of the book.
    /// </summary>
    public class MenuItemCategory : MonoBehaviour, IPointerClickHandler
    {
        GenericCategoryData data;
        public TextRender Title;
        public TextRender SubTitle;
        IBookPanel manager;

        UIButton uIButton;

        public void Init(IBookPanel _manager, GenericCategoryData _data, bool _selected)
        {
            uIButton = GetComponent<UIButton>();

            data = _data;
            manager = _manager;

            Title.text = data.Title;
            SubTitle.text = data.TitleEn;

            hightlight(_selected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.SelectSubCategory(data);
        }

        public void Select(string code)
        {
            hightlight(code == data.Id);
        }

        void hightlight(bool _status)
        {
            uIButton.Toggle(_status);
        }
    }
}