using EA4S.Audio;
using EA4S.UI;
using UnityEngine;

namespace EA4S.Core
{
    /// <summary>
    /// Manages the Keeper throughout the application. The Keeper gives hints and explains minigames to the player.
    /// </summary>
    public class KeeperManager : MonoBehaviour
    {
        public static KeeperManager I;

        void Start()
        {
            I = this;
        }

        // refactor: remove or complete this
        public void PlaySceneIntroduction(AppScene scene)
        {
            switch (scene) {
                default:
                    break;
            }
        }

        public void StopDialog()
        {
            AudioManager.I.StopDialogue(true);
        }

        public void PlayDialog(string localizationData_id, bool isKeeper = true, bool autoClose = true, System.Action _callback = null)
        {
            PlayDialog(LocalizationManager.GetLocalizationData(localizationData_id), isKeeper, autoClose, _callback);
        }

        public void PlayDialog(Database.LocalizationDataId id, bool isKeeper = true, bool autoClose = true, System.Action _callback = null)
        {
            PlayDialog(LocalizationManager.GetLocalizationData(id), isKeeper, autoClose, _callback);
        }

        public void PlayDialog(Database.LocalizationData data, bool isKeeper = true, bool autoClose = true, System.Action _callback = null)
        {
            if (autoClose) {
                WidgetSubtitles.I.DisplaySentence(data, 2, isKeeper, null);
                var callback = _callback;
                AudioManager.I.PlayDialogue(data, () => {
                    CloseDialog();
                    if (callback != null)
                        callback();
                });
            } else {
                WidgetSubtitles.I.DisplaySentence(data, 2, true, null);
                AudioManager.I.PlayDialogue(data, _callback);
            }
        }

        public void CloseDialog(bool _immediate = false)
        {
            WidgetSubtitles.I.Close(_immediate);
        }

    }
}