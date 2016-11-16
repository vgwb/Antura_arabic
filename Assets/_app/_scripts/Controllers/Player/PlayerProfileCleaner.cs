using UnityEngine;
using System.Collections;

namespace EA4S {

    public class PlayerProfileCleaner : MonoBehaviour {

        public void ResetAllPlayerProfiles() {
            AppManager.Instance.PlayerProfileManager.DeleteAllProfiles();
            AppManager.Instance.PlayerProfileManager = new PlayerProfileManager();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public void TotalResetPlayerPref() {
            PlayerPrefs.DeleteAll();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

    }
}
