using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Db;

namespace EA4S
{
    public class ItemLearningBlock : MonoBehaviour, IPointerClickHandler
    {
        LearningBlockData data;
        public TextRender Title;
        public TextRender SubTitle;

        ParentsPanel manager;

        public void Init(ParentsPanel _manager, LearningBlockData _data)
        {
            data = _data;
            manager = _manager;

            Title.text = data.Title_Ar;
            SubTitle.text = data.Title_En;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            manager.DetailLearningBlock(data);
        }
    }
}