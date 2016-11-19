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


        // Use this for initialization
        void Start()
        {

        }

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

            foreach (LearningBlockData word in AppManager.Instance.DB.GetAllLearningBlockData()) {
                btnGO = Instantiate(LearningBlockItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponentInChildren<Text>().text = word.Title_En;
            }
        }
    }
}