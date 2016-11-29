using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class KeeperManager : MonoBehaviour
    {
        public static KeeperManager I;

        void Start()
        {
            I = this;
        }

        public void PlaySceneIntroduction(AppScene scene)
        {
            switch (scene) {
                default:
                    break;
            }
        }

        public void PlayDialog(string localizationData_id, bool isKeeper = true, bool autoClose = true, System.Action _callback = null)
        {
            PlayDialog(LocalizationManager.GetLocalizationData(localizationData_id), isKeeper, autoClose, _callback);
        }

        public void PlayDialog(Db.LocalizationDataId id, bool isKeeper = true, bool autoClose = true, System.Action _callback = null)
        {
            PlayDialog(LocalizationManager.GetLocalizationData(id), isKeeper, autoClose, _callback);
        }

        public void PlayDialog(Db.LocalizationData data, bool isKeeper = true, bool autoClose = true, System.Action _callback = null)
        {
            if (autoClose) {
                WidgetSubtitles.I.DisplaySentence(data, 2, isKeeper, null);
                var callback = _callback;
                AudioManager.I.PlayDialog(data, () => {
                    CloseDialog();
                    if (callback != null)
                        callback();
                });
            } else {
                WidgetSubtitles.I.DisplaySentence(data, 2, true, null);
                AudioManager.I.PlayDialog(data, _callback);
            }
        }

        public void CloseDialog()
        {
            WidgetSubtitles.I.Close();
        }

    }
}