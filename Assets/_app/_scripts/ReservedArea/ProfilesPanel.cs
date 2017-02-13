using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EA4S.UI;
using EA4S.Profile;

namespace EA4S.ReservedArea
{
    public class ProfilesPanel : MonoBehaviour
    {
        public GameObject PlayerIconContainer;
        public GameObject PlayerIconPrefab;
        public GameObject ProfileCommandsContainer;
        string SelectedPlayerId;

        void Start()
        {
            ResetAll();
        }

        void ResetAll()
        {
            SelectedPlayerId = "";
            RefreshPlayerIcons();
            RefreshUI();
        }

        void RefreshPlayerIcons()
        {
            GameObject newIcon;

            foreach (Transform t in PlayerIconContainer.transform) {
                Destroy(t.gameObject);
            }

            List<PlayerIconData> players = AppManager.I.PlayerProfileManager.GetSavedPlayers();
            //List<PlayerIconData> players = new List<PlayerIconData>();
            //players.Add(new PlayerIconData("UUID-test1", 2, PlayerGender.F, PlayerTint.Green, false));
            //players.Add(new PlayerIconData("UUID-test2", 3, PlayerGender.M, PlayerTint.Yellow, false));
            //players.Add(new PlayerIconData("UUID-test3", 4, PlayerGender.F, PlayerTint.Red, false));
            //players.Add(new PlayerIconData("UUID-test-DEMO", 1, PlayerGender.F, PlayerTint.Green, true));

            // reverse the list for RIGHT 2 LEFT layout
            players.Reverse();
            foreach (var player in players) {
                newIcon = Instantiate(PlayerIconPrefab);
                newIcon.transform.SetParent(PlayerIconContainer.transform, false);
                newIcon.GetComponent<PlayerIcon>().Init(player);
                newIcon.GetComponent<UIButton>().Bt.onClick.AddListener(() => OnSelectPlayerProfile(player.Uuid));
            }

        }

        void RefreshUI()
        {
            ProfileCommandsContainer.SetActive(SelectedPlayerId != "");
            // highlight selected profile
            foreach (Transform t in PlayerIconContainer.transform) {
                t.GetComponent<PlayerIcon>().Select(SelectedPlayerId);
            }
        }

        public void OnSelectPlayerProfile(string uuid)
        {
            Debug.Log("OnSelectPlayerProfile " + uuid);
            if (SelectedPlayerId != uuid) {
                SelectedPlayerId = uuid;
            } else {
                SelectedPlayerId = "";
            }
            RefreshUI();
        }

        public void OnOpenSelectedPlayerProfile()
        {
            Debug.Log("OPEN " + SelectedPlayerId);
            AppManager.I.PlayerProfileManager.SetPlayerAsCurrentByUUID(SelectedPlayerId);
            AppManager.I.NavigationManager.GoToPlayerBook();
        }

        public void OnDeleteSelectPlayerProfile()
        {
            DoDeleteSelectPlayerProfile();
        }

        void DoDeleteSelectPlayerProfile()
        {
            Debug.Log("DELETE " + SelectedPlayerId);
            AppManager.I.PlayerProfileManager.DeletePlayerProfile(SelectedPlayerId);
            ResetAll();
        }

        public void OnExportSelectPlayerProfile()
        {
            Debug.Log("EXPORT " + SelectedPlayerId);
        }

        public void OnCreateDemoPlayer()
        {
            Debug.Log("DEMO");
        }

        public void OnImportProfile()
        {
            Debug.Log("IMPORT");
        }


    }
}