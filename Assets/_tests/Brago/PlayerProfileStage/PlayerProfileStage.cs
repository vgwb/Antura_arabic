using UnityEngine;
using System.Collections.Generic;
using System;
using EA4S.Core;

namespace EA4S.Test {
    /// <summary>
    /// Examples of use PlayerProfile module API.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class PlayerProfileStage : MonoBehaviour {

        void Start() {

            //List<string> AvailablePlayersId;
            //Debug.Log(JsonUtility.ToJson(AppManager.I.PlayerProfile.Options));
            //AvailablePlayersId = AppManager.I.PlayerProfile.Options.AvailablePlayers;
            //if(AvailablePlayersId.Count > 0) Debug.Log(AvailablePlayersId.ToString());

            //if (AvailablePlayersId.Count == 0) {
            //    AppManager.I.PlayerProfile.CreateNewPlayer(
            //        new PlayerProfile() {
            //            Key = (globalOptions.AvailablePlayers.Count + 1).ToString(),
            //            Id = globalOptions.AvailablePlayers.Count + 1,
            //            AvatarId = globalOptions.AvailablePlayers.Count + 1,
            //        });
            //}

            //AvailablePlayersId = AppManager.I.PlayerProfile.Options.AvailablePlayers;
            //if (AvailablePlayersId.Count > 0)
            //    AppManager.I.PlayerProfile.SetActivePlayer<PlayerProfile>(AvailablePlayersId[0]);

            //PlayerProfile player = AppManager.I.PlayerProfile.ActivePlayer as PlayerProfile;

            //Debug.LogFormat("{1}{0}{2}{0}{3}{0}{4}{0}", Environment.NewLine, player.Id, player.Name, player.Age);
            //AppManager.I.PlayerProfile.DeleteAllPlayerProfiles();
             
            Debug.LogFormat("P: {0} -> {1}", AppManager.I.Player.Uuid, AppManager.I.Player.CurrentJourneyPosition.ToString());

        }

        public void SetActualProfileJourney(int _ps) {
            AppManager.I.Player.CurrentJourneyPosition = new JourneyPosition(_ps, 1, 1);
            AppManager.I.Player.Save();
            Debug.LogFormat("P: {0} -> {1}", AppManager.I.Player.Uuid, AppManager.I.Player.CurrentJourneyPosition.ToString());
        }
    }

    [Serializable]
    public class MyPlayerProfile : ModularFramework.Modules.IPlayerProfile {
        public string Key { get; set; }

        public string Name;
        public string Lastname;
        public int Age;
    }

    [Serializable]
    public class MyGlobalOptions : ModularFramework.Modules.GlobalOptions {
        public bool HiRes;

        public MyGlobalOptions() {
            AvailablePlayers = new List<string>();
        }
    }
}