using UnityEngine;
using EA4S.Core;
using EA4S.Audio;

namespace EA4S.Scenes
{
    public class PlayerCreationScene : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start()
        {
            if (SceneMusic != Music.Custom) {
                AudioManager.I.PlayMusic(SceneMusic);
            }

        }

        public static void CreatePlayer(int age, PlayerGender gender, int avatarID, PlayerTint color)
        {
            Debug.Log(string.Format("Will create player of age {0}, gender {1}, avatarID {2}, color {3}", age, gender, avatarID, color));
            AppManager.Instance.PlayerProfileManager.CreatePlayerProfile(age, gender, avatarID, color);
            LogManager.I.LogInfo(InfoEvent.AppPlay, JsonUtility.ToJson(new DeviceInfo()));
            AppManager.Instance.NavigationManager.GoToNextScene();
        }

    }
}