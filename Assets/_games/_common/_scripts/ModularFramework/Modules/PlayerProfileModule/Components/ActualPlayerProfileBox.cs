
using UnityEngine;
using UnityEngine.UI;

namespace EA4S.Core
{
    
    public class ActualPlayerProfileBox : MonoBehaviour {

        #region UI
        public Text UsernameLable;
        public Text ProgressionLevelLable;
        #endregion

        void OnEnable() {
            // Remove UniRx refactoring request: now OnActivePlayerChanged must be called manually.
        }

       /* public void OnActivePlayerChanged() {
            if (AppManager.Instance != null && AppManager.Instance.Modules.PlayerProfile.ActivePlayer != null) {
                UsernameLable.text = AppManager.Instance.Modules.PlayerProfile.ActivePlayer.Key;

                Modules.PlayerProfile profile = AppManager.Instance.Modules.PlayerProfile.ActivePlayer as Modules.PlayerProfile;
                ProgressionLevelLable.text = profile.ProgressionRate.ToString();
            }
        }*/

    }

}
