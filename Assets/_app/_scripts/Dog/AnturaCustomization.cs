using Antura.Core;
using Antura.Database;
using Antura.Rewards;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Antura.Dog
{
    /// <summary>
    /// Saved data that defines how Antura is currently customized
    /// </summary>
    [Serializable]
    public class AnturaCustomization
    {
        [NonSerialized]
        public List<RewardPack> PropPacks = new List<RewardPack>();
        public List<string> PropPacksIds = new List<string>();

        [NonSerialized]
        public RewardPack TexturePack = null;
        public string TexturePackId = null;

        [NonSerialized]
        public RewardPack DecalPack =  null;
        public string DecalPackId = null;

        /// <summary>
        /// Loads all rewards in "this" object instance from list of reward ids.
        /// </summary>
        /// <param name="_listOfIdsAsJsonString">The list of ids as json string.</param>
        public void LoadFromListOfIds(string _listOfIdsAsJsonString)
        {
            if (AppManager.I.Player == null) {
                Debug.Log("No default reward already created. Unable to load customization now");
                return;
            }

            AnturaCustomization tmp = JsonUtility.FromJson<AnturaCustomization>(_listOfIdsAsJsonString);
            if (tmp != null) {
                PropPacksIds = tmp.PropPacksIds;
                TexturePackId = tmp.TexturePackId;
                DecalPackId = tmp.DecalPackId;
            }

            var rewardSystem = AppManager.I.RewardSystemManager;

            if (string.IsNullOrEmpty(TexturePackId))
            {
                RewardPack defaultTileTexturePack = rewardSystem.GetRewardPacksOfType(RewardBaseType.Texture)[0];
                Debug.Log("Default texture: " + defaultTileTexturePack);
                TexturePackId = defaultTileTexturePack.UniqueId;
            }
            if (string.IsNullOrEmpty(DecalPackId))
            {
                RewardPack defaultDecalTexturePack = rewardSystem.GetRewardPacksOfType(RewardBaseType.Decal)[0];
                Debug.Log("Default decal: " + defaultDecalTexturePack);
                DecalPackId = defaultDecalTexturePack.UniqueId;
            }

            // Load correct packs from IDs
            PropPacks = new List<RewardPack>();
            foreach (string propPackId in PropPacksIds)
            {
                var pack = rewardSystem.GetRewardPackByUniqueId(propPackId);
                if (pack != null)
                    PropPacks.Add(pack);
            }

            TexturePack = rewardSystem.GetRewardPackByUniqueId(TexturePackId);
            DecalPack = rewardSystem.GetRewardPackByUniqueId(DecalPackId);
        }

        /// <summary>
        /// Return all rewards objects to json list of ids (to be stored on db).
        /// </summary>
        public string GetJsonListOfIds()
        {
            return JsonUtility.ToJson(this);
        }
    }
}