using System;
using UnityEngine;
using System.Collections.Generic;
namespace EA4S {
    public class AnturaModelManager : MonoBehaviour {

        public const string ANTURA_REWARDS_CONFIG_PATH = "Configs/" + "AnturaRewardsConfig";
        public const string ANTURA_REWARDS_MODEL_PATH = "Models/Rewards/" + "Prefabs/";

        [Header("Bones Attach")]
        public Transform Dog_head, Dog_spine01, Dog_jaw, Dog_Tail3, dog_R_ear04, dog_L_ear04;

        RewardConfig config;

        void Start() {
            LoadFromConfig();
            //LoadReward("reward_punk_hair");
        }

        void LoadFromConfig() {
            TextAsset configData = Resources.Load(ANTURA_REWARDS_CONFIG_PATH) as TextAsset;
            string configString = configData.text;
            config = JsonUtility.FromJson<RewardConfig>(configString);
        }

        #region API

        public void LoadReward(string _id) {
            Reward reward = config.Antura_rewards.Find(r => r.ID == _id);
            string boneParent = reward.BoneAttach;
            Transform transformParent = transform;
            switch (boneParent) {
                case "dog_head":
                    transformParent = Dog_head;
                    break;
                case "dog_spine01":
                    transformParent = Dog_spine01;
                    break;
                case "dog_jaw":
                    transformParent = Dog_jaw;
                    break;
                case "dog_Tail3":
                    transformParent = Dog_Tail3;
                    break;
                case "dog_R_ear04":
                    transformParent = dog_R_ear04;
                    break;
                case "dog_L_ear04":
                    transformParent = dog_L_ear04;
                    break;
                default:
                    break;
            }

            GameObject rewardModel = Instantiate(Resources.Load(ANTURA_REWARDS_MODEL_PATH + reward.ID)) as GameObject;
            //rewardModel.transform.parent = transformParent;
            rewardModel.transform.SetParent(transformParent, true);
            //rewardModel.transform.localPosition = Vector3.zero;
            Debug.LogFormat("{0} -> {1}", rewardModel.transform.position, transformParent.position);

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