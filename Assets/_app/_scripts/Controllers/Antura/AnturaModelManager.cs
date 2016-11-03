using System;
using UnityEngine;
using System.Collections.Generic;
namespace EA4S {
    public class AnturaModelManager : MonoBehaviour {

        public static AnturaModelManager Instance;

        public const string ANTURA_REWARDS_CONFIG_PATH = "Configs/" + "AnturaRewardsConfig";
        public const string ANTURA_REWARDS_MODEL_PATH = "Models/Rewards/" + "Prefabs/";

        [Header("Bones Attach")]
        public Transform Dog_head;
        public Transform Dog_spine01;
        public Transform Dog_jaw;
        public Transform Dog_Tail3;
        public Transform Dog_R_ear04;
        public Transform Dog_L_ear04;

        Transform Dog_head_pointer, Dog_spine01_pointer, Dog_jaw_pointer, Dog_Tail3_pointer, dog_R_ear04_pointer, dog_L_ear04_pointer;
        [HideInInspector]
        public Transform transformParent;

        public RewardConfig config;

        void Awake() {
            Instance = this;
            LoadFromConfig();
        }

        void Start() {
            
        }

        void LoadFromConfig() {
            TextAsset configData = Resources.Load(ANTURA_REWARDS_CONFIG_PATH) as TextAsset;
            string configString = configData.text;
            config = JsonUtility.FromJson<RewardConfig>(configString);
        }

        #region API

        /// <summary>
        /// Loads the reward.
        /// </summary>
        /// <param name="_id">The identifier.</param>
        /// <returns></returns>
        public GameObject LoadReward(string _id) {
            Reward reward = config.Antura_rewards.Find(r => r.ID == _id);
            string boneParent = reward.BoneAttach;
            Transform transformParent = transform;
            GameObject rewardModel = null;
            switch (boneParent) {
                case "dog_head":
                    transformParent = Dog_head;
                    if (Dog_head_pointer)
                        Destroy(Dog_head_pointer.gameObject);
                    rewardModel = Instantiate(Resources.Load(ANTURA_REWARDS_MODEL_PATH + reward.ID), transformParent, false) as GameObject;
                    Dog_head_pointer = rewardModel.transform;
                    break;
                case "dog_spine01":
                    transformParent = Dog_spine01;
                    if (Dog_spine01_pointer)
                        Destroy(Dog_spine01_pointer.gameObject);
                    rewardModel = Instantiate(Resources.Load(ANTURA_REWARDS_MODEL_PATH + reward.ID), transformParent, false) as GameObject;
                    Dog_spine01_pointer = rewardModel.transform;
                    break;
                case "dog_jaw":
                    transformParent = Dog_jaw;
                    if (Dog_jaw_pointer)
                        Destroy(Dog_jaw_pointer.gameObject);
                    rewardModel = Instantiate(Resources.Load(ANTURA_REWARDS_MODEL_PATH + reward.ID), transformParent, false) as GameObject;
                    Dog_jaw_pointer = rewardModel.transform;
                    break;
                case "dog_Tail4":
                    transformParent = Dog_Tail3;
                    if (Dog_Tail3_pointer)
                        Destroy(Dog_Tail3_pointer.gameObject);
                    rewardModel = Instantiate(Resources.Load(ANTURA_REWARDS_MODEL_PATH + reward.ID), transformParent, false) as GameObject;
                    Dog_Tail3_pointer = rewardModel.transform;
                    break;
                case "dog_R_ear04":
                    transformParent = Dog_R_ear04;
                    if (dog_R_ear04_pointer)
                        Destroy(dog_R_ear04_pointer.gameObject);
                    rewardModel = Instantiate(Resources.Load(ANTURA_REWARDS_MODEL_PATH + reward.ID), transformParent, false) as GameObject;
                    dog_R_ear04_pointer = rewardModel.transform;
                    break;
                case "dog_L_ear04":
                    transformParent = Dog_L_ear04;
                    if (dog_L_ear04_pointer)
                        Destroy(dog_L_ear04_pointer.gameObject);
                    rewardModel = Instantiate(Resources.Load(ANTURA_REWARDS_MODEL_PATH + reward.ID), transformParent, false) as GameObject;
                    dog_L_ear04_pointer = rewardModel.transform;
                    break;
                default:
                    break;
            }
            return rewardModel;
        }

        #endregion

        #region structures

        [Serializable]
        public class RewardConfig {
            public List<Reward> Antura_rewards;
        }

        [Serializable]
        public class Reward {
            //public string Type;
            public string ID;
            public string RewardName;
            //public string Priority;
            //public string Done;
            public string BoneAttach;
            public string Material1;
            public string Material2;
        }

        #endregion
    }


}