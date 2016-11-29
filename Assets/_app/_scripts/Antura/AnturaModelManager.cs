using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace EA4S {

    public class AnturaModelManager : MonoBehaviour {

        public static AnturaModelManager Instance;

        [Header("Bones Attach")]
        public Transform Dog_head;
        public Transform Dog_spine01;
        public Transform Dog_jaw;
        public Transform Dog_Tail3;
        public Transform Dog_R_ear04;
        public Transform Dog_L_ear04;

        /// <summary>
        /// Pointer to transform used as parent for add reward model (and remove if already mounted yet).
        /// </summary>
        [HideInInspector]
        public Transform transformParent;

        #region Life cycle

        void Awake() {
            Instance = this;
            chargeCategoryList();
        }

        void Start() {
            if (AppManager.I.Player != null)
                AnturaModelManager.Instance.LoadAnturaCustomization(AppManager.I.Player.CurrentAnturaCustomizations);
        }

        #endregion

        class LoadedModel {
            public RewardPack Reward;
            public GameObject GO;
        }

        List<LoadedModel> LoadedModels = new List<LoadedModel>();

        #region API

        public void LoadAnturaCustomization(AnturaCustomization _anturaCustomization) {
            clearLoadedRewards();
            foreach (RewardPack forniture in _anturaCustomization.Fornitures) {
                GameObject GOAdded = LoadRewardPackOnAntura(forniture);

                ModelsManager.SwitchMaterial(LoadRewardPackOnAntura(forniture), forniture.GetMaterialPair());
            }
            /// - texture
            /// - decal
        }

        public AnturaCustomization SaveAnturaCustomization() {
            AnturaCustomization returnCustomization = new AnturaCustomization();
            foreach (LoadedModel loadedModel in LoadedModels) {
                returnCustomization.Fornitures.Add(new RewardPack() { ItemID = loadedModel.Reward.ItemID, ColorId = loadedModel.Reward.ColorId, Type = RewardTypes.reward });
            }
            AppManager.I.Player.SaveCustomization(returnCustomization);
            return returnCustomization;
        }


        public GameObject LoadRewardPackOnAntura(RewardPack _rewardPack) {
            switch (_rewardPack.Type) {
                case RewardTypes.reward:
                    return LoadRewardOnAntura(_rewardPack);
                case RewardTypes.texture:
                    break;
                case RewardTypes.decal:
                    break;
                default:
                    Debug.LogWarningFormat("Reward Type {0} not found!", _rewardPack.Type);
                    break;
            }
            return null;
        }

        void clearLoadedRewards() {
            foreach (var item in LoadedModels) {
                Destroy(item.GO);
            }
            LoadedModels.Clear();
        }

        /// <summary>
        /// Sets the reward material colors.
        /// </summary>
        /// <param name="_gameObject">The game object.</param>
        /// <param name="_rewardPack">The reward pack.</param>
        /// <returns></returns>
        public GameObject SetRewardMaterialColors(GameObject _gameObject, RewardPack _rewardPack) {
            ModelsManager.SwitchMaterial(_gameObject, _rewardPack.GetMaterialPair());
            //actualRewardsForCategoryColor.Add()
            return _gameObject;
        }

        /// <summary>
        /// Loads the reward on model.
        /// </summary>
        /// <param name="_id">The identifier.</param>
        /// <returns></returns>
        public GameObject LoadRewardOnAntura(RewardPack _rewardPack) {
            Reward reward = RewardSystemManager.GetConfig().Rewards.Find(r => r.ID == _rewardPack.ItemID);
            if (reward == null) {
                Debug.LogFormat("Reward {0} not found!", _rewardPack.ItemID);
                return null;
            }
            // Check if already charged reward of this category
            LoadedModel loadedModel = LoadedModels.Find(lm => lm.Reward.GetRewardCategory() == reward.Category);
            if(loadedModel != null) { 
                Destroy(loadedModel.GO);
                LoadedModels.Remove(loadedModel);
            }

            // Load Model
            string boneParent = reward.BoneAttach;
            Transform transformParent = transform;
            GameObject rewardModel = null;
            switch (boneParent) {
                case "dog_head":
                    transformParent = Dog_head;
                    //if (Dog_head_pointer)
                    //    Destroy(Dog_head_pointer.gameObject);
                    //Dog_head_pointer = ModelsManager.MountModel(reward.ID, transformParent).transform;
                    rewardModel = ModelsManager.MountModel(reward.ID, transformParent);
                    break;
                case "dog_spine01":
                    transformParent = Dog_spine01;
                    //if (Dog_spine01_pointer)
                    //    Destroy(Dog_spine01_pointer.gameObject);
                    //Dog_spine01_pointer = ModelsManager.MountModel(reward.ID, transformParent).transform;
                    rewardModel = ModelsManager.MountModel(reward.ID, transformParent);
                    break;
                case "dog_jaw":
                    transformParent = Dog_jaw;
                    //if (Dog_jaw_pointer)
                    //    Destroy(Dog_jaw_pointer.gameObject);
                    //Dog_jaw_pointer = ModelsManager.MountModel(reward.ID, transformParent).transform;
                    rewardModel = ModelsManager.MountModel(reward.ID, transformParent);
                    break;
                case "dog_Tail4":
                    transformParent = Dog_Tail3;
                    //if (Dog_Tail3_pointer)
                    //    Destroy(Dog_Tail3_pointer.gameObject);
                    //Dog_Tail3_pointer = ModelsManager.MountModel(reward.ID, transformParent).transform;
                    rewardModel = ModelsManager.MountModel(reward.ID, transformParent);
                    break;
                case "dog_R_ear04":
                    transformParent = Dog_R_ear04;
                    //if (dog_R_ear04_pointer)
                    //    Destroy(dog_R_ear04_pointer.gameObject);
                    //dog_R_ear04_pointer = ModelsManager.MountModel(reward.ID, transformParent).transform;
                    rewardModel = ModelsManager.MountModel(reward.ID, transformParent);
                    break;
                case "dog_L_ear04":
                    transformParent = Dog_L_ear04;
                    //if (dog_L_ear04_pointer)
                    //    Destroy(dog_L_ear04_pointer.gameObject);
                    //dog_L_ear04_pointer = ModelsManager.MountModel(reward.ID, transformParent).transform;
                    rewardModel = ModelsManager.MountModel(reward.ID, transformParent);
                    break;
                default:
                    break;
            }

            // Set materials
            ModelsManager.SwitchMaterial(rewardModel, _rewardPack.GetMaterialPair());

            // Save on LoadedModel List
            LoadedModels.Add(new LoadedModel() { Reward = _rewardPack, GO = rewardModel });
            return rewardModel;
        }
        #endregion

        #region Rewards standard (Antura fornitures)

        /// <summary>
        /// The category list
        /// </summary>
        List<string> categoryList = new List<string>();
        /// <summary>
        /// Charges the category list.
        /// </summary>
        void chargeCategoryList() {
            foreach (var reward in RewardSystemManager.GetConfig().Rewards) {
                if (!categoryList.Contains(reward.Category))
                    categoryList.Add(reward.Category);
            }
        }

        #endregion

        #region events

        #region subscriptions
        void OnEnable() {
            RewardSystemManager.OnRewardChanged += RewardSystemManager_OnRewardItemChanged;
            PlayerProfileManager.OnProfileChanged += PlayerProfileManager_OnProfileChanged;
        }

        private void PlayerProfileManager_OnProfileChanged() {
            LoadAnturaCustomization(AppManager.I.Player.CurrentAnturaCustomizations);
        }

        private void RewardSystemManager_OnRewardItemChanged(RewardPack _rewardPack) {
            LoadRewardOnAntura(_rewardPack);
        }

        void OnDisable() {
            RewardSystemManager.OnRewardChanged -= RewardSystemManager_OnRewardItemChanged;
            PlayerProfileManager.OnProfileChanged -= PlayerProfileManager_OnProfileChanged;
        }
        #endregion

        #endregion
    }

    #region Data Structures    
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AnturaCustomization {
        public List<RewardPack> Fornitures = new List<RewardPack>();
        public RewardPack MainTexture = new RewardPack();
        public RewardPack DecalTexture = new RewardPack();
    }

    #endregion
}