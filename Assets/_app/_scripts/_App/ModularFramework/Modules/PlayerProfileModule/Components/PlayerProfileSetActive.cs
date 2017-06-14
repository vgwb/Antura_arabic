
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EA4S.Core
{

    public class PlayerProfileSetActive : MonoBehaviour {
        #region UI
        public Text ProfileIDLable;
        #endregion

        #region functionalities
        [Tooltip("Any of this functionality is enabled only if value not null, otherwise will be ignored.")]
        /// <summary>
        /// Button that will activate this profile
        /// </summary>
        public Button SetActiveProfileButton;
        public Button DeleteProfileButton;
        #endregion


        public IPlayerProfile Player { get; protected set; }

        /// <summary>
        /// Subscribe for click event.
        /// </summary>
        void OnEnable() {
            // Remove UniRx refactoring request: any reactive interaction within this class must be called manually.
        }

        /// <summary>
        /// Init component ui with player data.
        /// </summary>
        /// <param name="_player"></param>
        public void Init(IPlayerProfile _player) {
            Player = _player;
            ProfileIDLable.text = Player.Key;
        }
    }
}