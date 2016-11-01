using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace EA4S {

    public class RewardsList : MonoBehaviour {

        public GridLayoutGroup ElementContainer;
        public GameObject ElementPrefab;

        public string RewardTypeFilter = "";

        #region life cycle
        void Awake() {
            ElementContainer = GetComponentInChildren<GridLayoutGroup>();
        }

        // Use this for initialization
        void Start() {
            ClearList();
        }

        // Update is called once per frame
        void Update() {
            if(AnturaModelManager.Instance.transformParent != null)
                Camera.main.transform.LookAt(AnturaModelManager.Instance.transformParent.position);
            else
                Camera.main.transform.LookAt(AnturaModelManager.Instance.transform.position);
        }
        #endregion

        #region Button List
        void ClearList() {
            foreach (Button b in ElementContainer.GetComponentsInChildren<Button>()) {
                b.onClick.RemoveAllListeners();
                GameObject.Destroy(b.gameObject);    
            }
            
        }

        void LoadRewarsList(string _position = "") {
            ClearList();
            List<AnturaModelManager.Reward> rewards;
            if (_position != "")
                rewards = AnturaModelManager.Instance.config.Antura_rewards.FindAll(r => r.BoneAttach == _position);
            else
                rewards = AnturaModelManager.Instance.config.Antura_rewards;

            foreach (AnturaModelManager.Reward reward in rewards) {
                Button b = Instantiate<Button>(ElementPrefab.GetComponent<Button>());
                b.transform.SetParent(ElementContainer.transform);
                b.GetComponentInChildren<Text>().text = reward.RewardName;
                b.onClick.AddListener(delegate { OnClickButton(b.GetComponentInChildren<Text>().text); });
            }
        }

        /// <summary>
        /// Delegate function for button click.
        /// </summary>
        /// <param name="_name">The name.</param>
        void OnClickButton(string _name) {
            AnturaModelManager.Reward reward = AnturaModelManager.Instance.config.Antura_rewards.Find(r => r.RewardName == _name);
            AnturaModelManager.Instance.LoadReward(reward.ID);
        }
        #endregion

        public void SetRewardTypeFilter(string _filterString) {
            RewardTypeFilter = _filterString;
            LoadRewarsList(_filterString);

            switch (_filterString) {
                case "dog_head":
                    doMoveCamera(new Vector3(0.0f, 11.0f, -17.0f));
                    break;
                case "dog_spine01":
                    doMoveCamera(new Vector3(-10.0f, 11.0f, 8.0f));
                    break;
                case "dog_jaw":
                    doMoveCamera(new Vector3(-5.0f, 6.0f, -12.0f));
                    break;
                case "dog_Tail4":
                    doMoveCamera(new Vector3(-7.0f, 9.0f, 8.0f));
                    break;
                case "dog_R_ear04":
                    doMoveCamera(new Vector3(-8.0f, 9.0f, -8.0f));
                    break;
                case "dog_L_ear04":
                    doMoveCamera(new Vector3(8.0f, 9.0f, -8.0f));
                    break;
            }
        }

       
        

        void doMoveCamera(Vector3 _position) {
            float duration = 2;
            Camera.main.transform.DOMove(_position, duration);
            
        }
    }
}