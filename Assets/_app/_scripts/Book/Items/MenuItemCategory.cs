using EA4S.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.DeExtensions;
using DG.DeInspektor.Attributes;

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

        public UIButton UIButton { get { if (fooUIButton == null) fooUIButton = this.GetComponent<UIButton>(); return fooUIButton; } }
        UIButton fooUIButton;

        public void Init(IBookPanel _manager, GenericCategoryData _data)
        {
            data = _data;
            manager = _manager;

            Title.text = data.Title;
            SubTitle.text = data.Id;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.SelectSubCategory(data);

            Select(data.Id);
        }

        [DeMethodButton("DEBUG: Select", mode = DeButtonMode.PlayModeOnly)]
        public void Select(string code)
        {
            UIButton.Toggle(code == data.Id);
        }


    }
}