using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ModularFramework.Core;

namespace EA4S {

    public class PlayerProfileSelection : MonoBehaviour {

        public GameObject[] GOAvatars;

        public ScrollRect PlayerSelectables;
        public ScrollRect AvailableAvatars;

        

        // Use this for initialization
        void Start() {
            // Instantiate avatars form GOAvatars list
            int dim = 100;
            for (int i = 0; i < GOAvatars.Length; i++) {
                GOAvatars[i].transform.SetParent(AvailableAvatars.content, false);
                GOAvatars[i].name = (i + 1).ToString();
                RectTransform rect = GOAvatars[i].GetComponent<RectTransform>();
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dim);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dim);
                rect.anchoredPosition = new Vector3((dim * i) + dim / 2, -dim / 2, 0);
            }

            // Visual containers reorder
            refreshActivePlayerList();
        }

        public PlayerProfile CreatePlayer(int _id) {
            PlayerProfile newPlayer = AppManager.I.PlayerProfile.CreateNewPlayer(
                new PlayerProfile() {
                    Key = _id.ToString(),
                    Id = _id,
                    AvatarId = _id,
                }) as PlayerProfile;
            return newPlayer;
        }

        /// <summary>
        /// Selects the player profile.
        /// </summary>
        /// <param name="_ppKey">The pp key.</param>
        public void SelectPlayerProfile(string _ppKey) {
            //AppManager.Instance.Player.CreateOrLoadPlayerProfile(_ppKey);

            // Visual containers reorder
            refreshActivePlayerList();
        }

        /// <summary>
        /// Refreshes the active player list.
        /// </summary>
        void refreshActivePlayerList() {
            for (int i = 0; i < PlayerSelectables.content.childCount; i++) {
                PlayerSelectables.content.GetChild(i).SetParent(AvailableAvatars.content, false);
            }

            foreach (string playerKey in AppManager.I.GameSettings.AvailablePlayers) {
                Transform t = AvailableAvatars.content.FindChild(playerKey);
                if (t)
                    t.SetParent(PlayerSelectables.content, false);
            }
        }

    }
}