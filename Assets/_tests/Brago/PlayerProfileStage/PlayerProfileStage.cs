using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S.Test {
    /// <summary>
    /// Examples of use PlayerProfile module API.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class PlayerProfileStage : MonoBehaviour {

        void Start() {
            List<string> AvailablePlayersId;
            AvailablePlayersId = AppManager.Instance.PlayerProfile.Players.AvailablePlayers;
            if(AvailablePlayersId.Count > 0) Debug.Log(AvailablePlayersId.ToString());

            if (AvailablePlayersId.Count == 0) {
                AppManager.Instance.PlayerProfile.CreateNewPlayer(
                    new MyPlayerProfile() {
                        Id = "A_" + System.DateTime.Now.TimeOfDay.ToString(),
                        Name = "Name_" + System.DateTime.Now.TimeOfDay.ToString(),
                        Lastname = "Lastname_" + System.DateTime.Now.TimeOfDay.ToString(),
                        Age = 10,
                    });
            }

            AvailablePlayersId = AppManager.Instance.PlayerProfile.Players.AvailablePlayers;
            if (AvailablePlayersId.Count > 0)
                AppManager.Instance.PlayerProfile.SetActivePlayer<MyPlayerProfile>(AvailablePlayersId[0]);

            MyPlayerProfile player = AppManager.Instance.PlayerProfile.ActivePlayer as MyPlayerProfile;

            Debug.LogFormat("{1}{0}{2}{0}{3}{0}{4}{0}", Environment.NewLine, player.Id, player.Name, player.Lastname, player.Age);
            //AppManager.Instance.PlayerProfile.DeleteAllPlayerProfiles();
        }
    }

    [Serializable]
    public class MyPlayerProfile : ModularFramework.Modules.IPlayerProfile {
        public string Id { get; set; }

        public string Name;
        public string Lastname;
        public int Age;
    }
}