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
                case AppScene.Mood:
                    if ((AppManager.Instance.Player.CurrentJourneyPosition.PlaySession) > 2) {
                        PlayDialog("mood_how_do_you_feel");
                    } else {
                        PlayDialog("mood_how_are_you_today");
                    }
                    break;
                default:
                    break;
            }
        }

        public void PlayDialog(string dialog_id)
        {
            WidgetSubtitles.I.DisplaySentence(dialog_id);
        }

    }
}