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
        IBookPanel manager;

        UIButton uIButton;

        public void Init(IBookPanel _manager, GenericCategoryData _data)
        {
            uIButton = GetComponent<UIButton>();

            data = _data;
            manager = _manager;

            Title.text = data.Title;
            SubTitle.text = data.Id;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.SelectSubCategory(data);
        }

        public void Select(string code)
        {
            uIButton.Toggle(code == data.Id);
        }
    }
}