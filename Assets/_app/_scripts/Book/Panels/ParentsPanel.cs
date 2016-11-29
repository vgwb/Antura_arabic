using UnityEngine;
using UnityEngine.UI;
using EA4S.Db;
using System.Collections.Generic;

namespace EA4S
{
    public class ParentsPanel : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject LearningBlockItemPrefab;

        [Header("References")]
        public GameObject ElementsContainer;
        public TextRender ScoreText;

        void OnEnable()
        {
            InitUI();
        }

        void InitUI()
        {
            GameObject btnGO;

            emptyListContainers();

            List<LearningBlockInfo> info_list = AppManager.I.Teacher.scoreHelper.GetAllLearningBlockInfo();
            foreach (var item_info in info_list) {
                btnGO = Instantiate(LearningBlockItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponent<ItemLearningBlock>().Init(this, item_info);
            }
        }

        public void DetailLearningBlock(LearningBlockInfo info)
        {
            AudioManager.I.PlayDialog(info.data.GetTitleSoundFilename());
            ScoreText.text = "Score: " + info.score;
        }

        void emptyListContainers()
        {
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }
        }
    }
}