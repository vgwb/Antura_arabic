using UnityEngine;
using System.Collections;

namespace EA4S {

    public class PlayerProfileCleaner : MonoBehaviour {

        public void ResetAllPlayerProfiles() {
            AppManager.I.PlayerProfileManager.DeleteAllProfiles();
            AppManager.I.PlayerProfileManager = new PlayerProfileManager();
            // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public void TotalResetPlayerPref() {
            PlayerPrefs.DeleteAll();
            UnityEngine.SceneManagement.SceneManager.UnloadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, UnityEngine.SceneManagement.LoadSceneMode.Single);
            AppManager.I.PlayerProfileManager = new PlayerProfileManager();
        }

    }
}
