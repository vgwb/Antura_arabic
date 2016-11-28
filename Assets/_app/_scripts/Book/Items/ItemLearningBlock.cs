using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Db;

namespace EA4S
{
    public class ItemLearningBlock : MonoBehaviour, IPointerClickHandler
    {
        LearningBlockInfo info;
        public TextRender Title;
        public TextRender Info;
        public TextRender SubTitle;

        ParentsPanel manager;

        public void Init(ParentsPanel _manager, LearningBlockInfo _info)
        {
            info = _info;
            manager = _manager;

            Title.text = info.data.Title_Ar;
            SubTitle.text = info.data.Title_En + " " + info.data.Id;

            if (!info.unlocked)
            {
                GetComponent<Button>().interactable = false;
            }
            else
            {
                GetComponent<Button>().interactable = true;
            }

            var score = info.score;
            // @note: we should already save the score when a block is finished, and not compute it when showing it
            //var score = TeacherAI.I.GetLearningBlockScore(info.data);

            Info.text = "Score: " + score; 
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (info.unlocked)
            {
                manager.DetailLearningBlock(info);
            }
        }
    }
}