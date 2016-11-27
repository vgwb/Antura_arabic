using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Db;

namespace EA4S
{
    public class ItemPhrase : MonoBehaviour, IPointerClickHandler
    {
        PhraseData data;
        public TextRender Title;
        public TextRender SubTitle;

        BookPanel manager;

        public void Init(BookPanel _manager, PhraseData _data)
        {
            data = _data;
            manager = _manager;

            Title.text = data.Arabic;
            SubTitle.text = data.Id;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.DetailPhrase(data);
        }
    }
}