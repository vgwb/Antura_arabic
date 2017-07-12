using Antura.Core;
using Antura.Profile;
using UnityEngine;

namespace Antura.Scenes
{
    public class PlayerCreationScene : SceneBase
    {
        protected override void Start()
        {
        }

        public static void CreatePlayer(int age, PlayerGender gender, int avatarID, PlayerTint color)
        {
            Debug.Log(string.Format("Will create player of age {0}, gender {1}, avatarID {2}, color {3}", age, gender, avatarID, color));
            AppManager.I.PlayerProfileManager.CreatePlayerProfile(age, gender, avatarID, color);
            LogManager.I.LogInfo(InfoEvent.AppPlay, JsonUtility.ToJson(new DeviceInfo()));
            AppManager.I.NavigationManager.GoToNextScene();
        }
    }
}