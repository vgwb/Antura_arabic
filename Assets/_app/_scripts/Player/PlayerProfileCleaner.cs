using EA4S.Core;
using UnityEngine;

namespace EA4S.Profile
{

    /// <summary>
    /// Handles cleanup of player profiles.
    /// </summary>
    public class PlayerProfileCleaner : MonoBehaviour
    {
        public void ResetAllPlayerProfiles() {
            (AppManager.Instance as AppManager).PlayerProfileManager.DeleteAllProfiles();
            (AppManager.Instance as AppManager).PlayerProfileManager = new PlayerProfileManager();
            // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public void TotalResetPlayerPref() {
            PlayerPrefs.DeleteAll();
            //UnityEngine.SceneManagement.SceneManager.UnloadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, UnityEngine.SceneManagement.LoadSceneMode.Single);
            (AppManager.Instance as AppManager).PlayerProfileManager = new PlayerProfileManager();
        }
    }
}
