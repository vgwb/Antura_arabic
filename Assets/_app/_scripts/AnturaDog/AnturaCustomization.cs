using System;
using System.Collections.Generic;
using Antura.Database;
using Antura.Rewards;
using UnityEngine;

namespace Antura.Dog
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AnturaCustomization
    {
        [NonSerialized]
        public List<RewardPackUnlockData> Fornitures = new List<RewardPackUnlockData>();

        public List<string> FornituresIds = new List<string>();

        [NonSerialized]
        public RewardPackUnlockData TileTexture = new RewardPackUnlockData();

        public string TileTextureId = null;

        [NonSerialized]
        public RewardPackUnlockData DecalTexture = new RewardPackUnlockData();

        public string DecalTextureId = null;

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
            List<RewardPackUnlockData> unlocked = AppManager.I.Player.RewardsUnlocked;
            AnturaCustomization tmp = JsonUtility.FromJson<AnturaCustomization>(_listOfIdsAsJsonString);
            if (tmp != null) {
                FornituresIds = tmp.FornituresIds;
                TileTextureId = tmp.TileTextureId;
                DecalTextureId = tmp.DecalTextureId;
            }
            if (string.IsNullOrEmpty(TileTextureId)) {
                RewardPackUnlockData defaultTileTexturePack = unlocked.Find(r => r.Type == RewardTypes.texture);
                TileTextureId = defaultTileTexturePack.GetIdAccordingToDBRules();
            }
            if (string.IsNullOrEmpty(DecalTextureId)) {
                RewardPackUnlockData defaultDecalTexturePack = unlocked.Find(r => r.Type == RewardTypes.decal);
                DecalTextureId = defaultDecalTexturePack.GetIdAccordingToDBRules();
            }
            Fornitures = new List<RewardPackUnlockData>();
            foreach (string itemId in FornituresIds) {
                // Load Fornitures for any id from db
                RewardPackUnlockData pack = unlocked.Find(r => r.Id == itemId);
                Fornitures.Add(pack);
            }

            // Load TileTexture from TileTextureId
            if (TileTextureId != null) {
                TileTexture = unlocked.Find(r => r.Id == TileTextureId);
            }

            // Load DecalTexture from DecalTextureId
            if (DecalTextureId != null) {
                DecalTexture = unlocked.Find(r => r.Id == DecalTextureId);
            }
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