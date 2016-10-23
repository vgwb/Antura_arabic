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
            AnturaGlobalOptions globalOptions = new AnturaGlobalOptions() { AvailablePlayers = new List<string>() { } };
            globalOptions = AppManager.Instance.PlayerProfile.LoadGlobalOptions<AnturaGlobalOptions>(globalOptions) as AnturaGlobalOptions;

            List<string> AvailablePlayersId;
            Debug.Log(JsonUtility.ToJson(AppManager.Instance.PlayerProfile.Options));
            AvailablePlayersId = AppManager.Instance.PlayerProfile.Options.AvailablePlayers;
            if(AvailablePlayersId.Count > 0) Debug.Log(AvailablePlayersId.ToString());

            if (AvailablePlayersId.Count == 0) {
                AppManager.Instance.PlayerProfile.CreateNewPlayer(
                    new AnturaPlayerProfile() {
                        Key = (globalOptions.AvailablePlayers.Count + 1).ToString(),
                        Id = globalOptions.AvailablePlayers.Count + 1,
                        AvatarId = globalOptions.AvailablePlayers.Count + 1,
                    });
            }

            AvailablePlayersId = AppManager.Instance.PlayerProfile.Options.AvailablePlayers;
            if (AvailablePlayersId.Count > 0)
                AppManager.Instance.PlayerProfile.SetActivePlayer<AnturaPlayerProfile>(AvailablePlayersId[0]);

            AnturaPlayerProfile player = AppManager.Instance.PlayerProfile.ActivePlayer as AnturaPlayerProfile;

            //Debug.LogFormat("{1}{0}{2}{0}{3}{0}{4}{0}", Environment.NewLine, player.Id, player.Name, player.Age);
            //AppManager.Instance.PlayerProfile.DeleteAllPlayerProfiles();
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