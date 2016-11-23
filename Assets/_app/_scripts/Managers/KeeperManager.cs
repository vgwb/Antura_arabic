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

        public void PlayDialog(Db.LocalizationDataId id)
        {
            PlayDialog(id.ToString());
        }

        public void PlayDialog(string dialog_id)
        {
            WidgetSubtitles.I.DisplaySentence(dialog_id);
        }

    }
}