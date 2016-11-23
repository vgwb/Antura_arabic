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
                    if ((AppManager.Instance.Player.CurrentJourneyPosition.PlaySession) < 2) {
                        PlayDialog(Db.LocalizationDataId.Mood_Question_2);
                    } else {
                        int rnd = Random.Range(1, 3);
                        switch (rnd) {
                            case 1:
                                PlayDialog(Db.LocalizationDataId.Mood_Question_1);
                                break;
                            case 3:
                                PlayDialog(Db.LocalizationDataId.Mood_Question_3);
                                break;
                            default:
                                PlayDialog(Db.LocalizationDataId.Mood_Question_2);
                                break;
                        }
                    }
                    break;
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