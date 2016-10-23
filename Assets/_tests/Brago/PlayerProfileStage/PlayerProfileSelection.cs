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
            int dim = 100;
            for (int i = 0; i < GOAvatars.Length; i++) {
                GOAvatars[i].transform.SetParent(AvailableAvatars.content, false);
                GOAvatars[i].name = (i + 1).ToString();
                RectTransform rect = GOAvatars[i].GetComponent<RectTransform>();
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dim);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dim);
                rect.anchoredPosition = new Vector3((dim * i) + dim / 2, - dim / 2, 0);
            }
            
            AppManager.Instance.GameSettings = new AppSettings() { AvailablePlayers = new List<string>() { } };
            AppManager.Instance.GameSettings = AppManager.Instance.PlayerProfile.LoadGlobalOptions<AppSettings>(new AppSettings()) as AppSettings;

            // Visual containers reorder
            foreach (string playerKey in AppManager.Instance.GameSettings.AvailablePlayers) {
                Transform t = AvailableAvatars.content.FindChild(playerKey);
                if (t)
                    t.SetParent(PlayerSelectables.content, false);
            }
        }

        public void CreatePlayer(int _id) {
            AppManager.Instance.PlayerProfile.CreateNewPlayer(
                new PlayerProfile() {
                    Key = _id.ToString(),
                    Id = _id,
                    AvatarId = _id,
                });
        }

        public void SelectPlayerProfile(string _ppKey) {
            if (AppManager.Instance.GameSettings.AvailablePlayers.Contains(_ppKey)) {
                AppManager.Instance.PlayerProfile.SetActivePlayer<PlayerProfile>(_ppKey);
            } else {
                CreatePlayer(int.Parse(_ppKey));
                AppManager.Instance.GameSettings = AppManager.Instance.PlayerProfile.LoadGlobalOptions<AppSettings>(new AppSettings()) as AppSettings;
            }
            // Visual containers reorder
            foreach (string playerKey in AppManager.Instance.GameSettings.AvailablePlayers) {
                Transform t = AvailableAvatars.content.FindChild(playerKey);
                if(t)
                    t.SetParent(PlayerSelectables.content, false);
            }
        }

    }
}