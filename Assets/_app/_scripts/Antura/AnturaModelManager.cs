using System;
using UnityEngine;
using System.Collections.Generic;
using EA4S.Database;
using EA4S.Profile;
using EA4S.Rewards;

namespace EA4S.Antura
{
    /// <summary>
    /// Handles loading and assignment of visual reward props appearing on Antura.
    /// </summary>
    // convention: rename variables
    // refactor: the class needs a complete refactoring
    public class AnturaModelManager : MonoBehaviour
    {

        // refactor: remove static instance
        public static AnturaModelManager Instance;

        [Header("Bones Attach")]
        public Transform Dog_head;
        public Transform Dog_spine01;
        public Transform Dog_jaw;
        public Transform Dog_Tail3;
        public Transform Dog_R_ear04;
        public Transform Dog_L_ear04;

        [Header("Materials Owner")]
        public SkinnedMeshRenderer SkinnedMesh;
        public SkinnedMeshRenderer[] SkinnedMeshsTextureOnly;

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
            public RewardPackUnlockData Reward;
            public GameObject GO;
        }

        List<LoadedModel> LoadedModels = new List<LoadedModel>();
        RewardPackUnlockData LoadedTileTexture = new RewardPackUnlockData();
        RewardPackUnlockData LoadedDecal = new RewardPackUnlockData();

        #region API

        /// <summary>
        /// Loads the antura customization elements on Antura model.
        /// </summary>
        /// <param name="_anturaCustomization">The antura customization.</param>
        public void LoadAnturaCustomization(AnturaCustomization _anturaCustomization) {
            ClearLoadedRewards();
            foreach (RewardPackUnlockData forniture in _anturaCustomization.Fornitures) {
                LoadRewardPackOnAntura(forniture);
                ModelsManager.SwitchMaterial(LoadRewardPackOnAntura(forniture), forniture.GetMaterialPair());
            }
            LoadRewardPackOnAntura(_anturaCustomization.TileTexture);
            LoadRewardPackOnAntura(_anturaCustomization.DecalTexture);
            /// - decal
        }

        /// <summary>
        /// Saves the antura customization using the current model customization.
        /// </summary>
        /// <returns></returns>
        public AnturaCustomization SaveAnturaCustomization() {
            AnturaCustomization returnCustomization = new AnturaCustomization();
            foreach (LoadedModel loadedModel in LoadedModels) {
                RewardPackUnlockData pack = new RewardPackUnlockData() { ItemId = loadedModel.Reward.ItemId, ColorId = loadedModel.Reward.ColorId, Type = RewardTypes.reward };
                returnCustomization.Fornitures.Add(pack);
                returnCustomization.FornituresIds.Add(pack.GetIdAccordingToDBRules());
            }
            returnCustomization.TileTexture = LoadedTileTexture;
            returnCustomization.TileTextureId = LoadedTileTexture.GetIdAccordingToDBRules();
            returnCustomization.DecalTexture = LoadedDecal;
            returnCustomization.DecalTextureId = LoadedDecal.GetIdAccordingToDBRules();
            AppManager.I.Player.SaveCustomization(returnCustomization);
            return returnCustomization;
        }


        public GameObject LoadRewardPackOnAntura(RewardPackUnlockData rewardPackUnlockData) {
            if (rewardPackUnlockData == null)
                return null;
            switch (rewardPackUnlockData.Type) {
                case RewardTypes.reward:
                    return LoadRewardOnAntura(rewardPackUnlockData);
                case RewardTypes.texture:
                    Material newMaterial = MaterialManager.LoadTextureMaterial(rewardPackUnlockData.ItemId, rewardPackUnlockData.ColorId);
                    // Main mesh
                    Material[] mats = SkinnedMesh.sharedMaterials;
                    mats[0] = newMaterial;
                    SkinnedMesh.sharedMaterials = mats;
                    LoadedTileTexture = rewardPackUnlockData;
                    // Sup mesh for texture
                    foreach (SkinnedMeshRenderer renderer in SkinnedMeshsTextureOnly) {
                        Material[] materials = renderer.sharedMaterials;
                        materials[0] = newMaterial;
                        renderer.sharedMaterials = materials;
                    }
                    break;
                case RewardTypes.decal:
                    Material newDecalMaterial = MaterialManager.LoadTextureMaterial(rewardPackUnlockData.ItemId, rewardPackUnlockData.ColorId);
                    // Main mesh
                    Material[] decalMats = SkinnedMesh.sharedMaterials;
                    decalMats[1] = newDecalMaterial;
                    SkinnedMesh.sharedMaterials = decalMats;
                    // Sup mesh for texture
                    foreach (SkinnedMeshRenderer renderer in SkinnedMeshsTextureOnly) {
                        Material[] materials = renderer.sharedMaterials;
                        materials[1] = newDecalMaterial;
                        renderer.sharedMaterials = materials;
                    }
                    LoadedDecal = rewardPackUnlockData;
                    break;
                default:
                    Debug.LogWarningFormat("Reward Type {0} not found!", rewardPackUnlockData.Type);
                    break;
            }
            return null;
        }

        public void ClearLoadedRewards() {
            foreach (var item in LoadedModels) {
                Destroy(item.GO);
            }
            LoadedModels.Clear();
        }

        /// <summary>
        /// Clears the loaded reward in category.
        /// </summary>
        /// <param name="_categoryId">The category identifier.</param>
        public void ClearLoadedRewardInCategory(string _categoryId) {
            LoadedModel lm = LoadedModels.Find(m => m.Reward.GetRewardCategory() == _categoryId);
            if(lm != null) { 
                Destroy(lm.GO);
                LoadedModels.Remove(lm);
            }
        }

        /// <summary>
        /// Sets the reward material colors.
        /// </summary>
        /// <param name="_gameObject">The game object.</param>
        /// <param name="rewardPackUnlockData">The reward pack.</param>
        /// <returns></returns>
        public GameObject SetRewardMaterialColors(GameObject _gameObject, RewardPackUnlockData rewardPackUnlockData) {
            ModelsManager.SwitchMaterial(_gameObject, rewardPackUnlockData.GetMaterialPair());
            //actualRewardsForCategoryColor.Add()
            return _gameObject;
        }

        /// <summary>
        /// Loads the reward on model.
        /// </summary>
        /// <param name="_id">The identifier.</param>
        /// <returns></returns>
        public GameObject LoadRewardOnAntura(RewardPackUnlockData rewardPackUnlockData) {
            Reward reward = RewardSystemManager.GetConfig().Rewards.Find(r => r.ID == rewardPackUnlockData.ItemId);
            if (reward == null) {
                Debug.LogFormat("Reward {0} not found!", rewardPackUnlockData.ItemId);
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
            ModelsManager.SwitchMaterial(rewardModel, rewardPackUnlockData.GetMaterialPair());

            // Save on LoadedModel List
            LoadedModels.Add(new LoadedModel() { Reward = rewardPackUnlockData, GO = rewardModel });
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

        private void RewardSystemManager_OnRewardItemChanged(RewardPackUnlockData rewardPackUnlockData) {
            LoadRewardPackOnAntura(rewardPackUnlockData);
            SaveAnturaCustomization();
        }

        void OnDisable() {
            RewardSystemManager.OnRewardChanged -= RewardSystemManager_OnRewardItemChanged;
            PlayerProfileManager.OnProfileChanged -= PlayerProfileManager_OnProfileChanged;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AnturaCustomization {
        [NonSerialized]
        public List<RewardPackUnlockData> Fornitures = new List<RewardPackUnlockData>();
        public List<string> FornituresIds = new List<string>();
        [NonSerialized]
        public RewardPackUnlockData TileTexture = new RewardPackUnlockData();
        public string TileTextureId;
        [NonSerialized]
        public RewardPackUnlockData DecalTexture = new RewardPackUnlockData();
        public string DecalTextureId;

        /// <summary>
        /// Loads all rewards in "this" object instance from list of reward ids.
        /// </summary>
        /// <param name="_listOfIdsAsJsonString">The list of ids as json string.</param>
        public void LoadFromListOfIds(string _listOfIdsAsJsonString) {
            AnturaCustomization tmp = JsonUtility.FromJson<AnturaCustomization>(_listOfIdsAsJsonString);
            if(tmp!= null) { 
                FornituresIds = tmp.FornituresIds;
                TileTextureId = tmp.TileTextureId;
                DecalTextureId = tmp.DecalTextureId;
            }
            if (TileTextureId == string.Empty) {
                RewardPackUnlockData defaultTileTexturePack = RewardSystemManager.GetFirstAnturaReward(RewardTypes.texture);
                TileTextureId = defaultTileTexturePack.GetIdAccordingToDBRules();
            }
            if (DecalTextureId == string.Empty) {
                RewardPackUnlockData defaultDecalTexturePack = RewardSystemManager.GetFirstAnturaReward(RewardTypes.decal);
                DecalTextureId = defaultDecalTexturePack.GetIdAccordingToDBRules();
            }
            Fornitures = new List<RewardPackUnlockData>();
            foreach (string itemId in FornituresIds) {
                // Load Fornitures for any id from db
                Debug.Log(AppManager.I.Player);
                RewardPackUnlockData pack = AppManager.I.Player.RewardsUnlocked.Find(r => r.Id == itemId);
                Fornitures.Add(pack);
            }

            // Load TileTexture from TileTextureId
            if (TileTextureId != null)
                TileTexture = AppManager.I.Player.RewardsUnlocked.Find(r => r.Id == TileTextureId);

            // Load DecalTexture from DecalTextureId
            if (DecalTextureId != null)
                DecalTexture = AppManager.I.Player.RewardsUnlocked.Find(r => r.Id == DecalTextureId);

        }

        /// <summary>
        /// Return all rewards objects to json list of ids (to be stored on db).
        /// </summary>
        public string GetJsonListOfIds() {
            ////// Fornitures
            //FornituresIds = new List<string>();
            //foreach (RewardPackUnlockData pack in Fornitures) {
            //    FornituresIds.Add(pack.GetIdAccordingToDBRules());
            //}

            ////// TileTextureId
            //TileTextureId = TileTexture.GetIdAccordingToDBRules();

            ////// DecalTextureId
            //DecalTextureId = DecalTexture.GetIdAccordingToDBRules();
            return JsonUtility.ToJson(this);
        }

    }
}