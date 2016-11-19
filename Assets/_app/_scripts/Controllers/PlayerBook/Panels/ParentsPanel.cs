using UnityEngine;
using UnityEngine.UI;
using EA4S.Db;

namespace EA4S
{
    public class ParentsPanel : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject LearningBlockItemPrefab;

        [Header("References")]
        public GameObject ElementsContainer;


        void OnEnable()
        {
            InitUI();
        }

        void InitUI()
        {
            GameObject btnGO;

            //// Words
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }

            foreach (LearningBlockData item in AppManager.Instance.DB.GetAllLearningBlockData()) {
                btnGO = Instantiate(LearningBlockItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponent<ItemLearningBlock>().Init(this, item);
            }
        }


        public void DetailLearningBlock(LearningBlockData data)
        {

        }
    }
}