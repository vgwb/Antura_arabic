using UnityEngine;
using EA4S.Core;

namespace EA4S.Map
{
    public class Dialogues : MonoBehaviour
    {
        public int numberStage;
        public bool dialoguePlayed;
        void OnTriggerEnter(Collider other)
        {
            bool isMaxPosition = IsMaxJourneyPosition();
            if ((other.gameObject.tag == "Player") && (isMaxPosition) && (numberStage > 1) && (!dialoguePlayed)) {
                Database.LocalizationDataId[] data = new Database.LocalizationDataId[7];
                data[2] = Database.LocalizationDataId.Map_Intro_Map2;
                data[3] = Database.LocalizationDataId.Map_Intro_Map3;
                data[4] = Database.LocalizationDataId.Map_Intro_Map4;
                data[5] = Database.LocalizationDataId.Map_Intro_Map5;
                data[6] = Database.LocalizationDataId.Map_Intro_Map6;
                KeeperManager.I.PlayDialog(data[numberStage], true, true);
                dialoguePlayed = true;
            }
        }

        bool IsMaxJourneyPosition()
        {
            if (((AppManager.Instance as AppManager).Player.CurrentJourneyPosition.Stage == (AppManager.Instance as AppManager).Player.MaxJourneyPosition.Stage) &&
                ((AppManager.Instance as AppManager).Player.CurrentJourneyPosition.LearningBlock == (AppManager.Instance as AppManager).Player.MaxJourneyPosition.LearningBlock) &&
                ((AppManager.Instance as AppManager).Player.CurrentJourneyPosition.PlaySession == (AppManager.Instance as AppManager).Player.MaxJourneyPosition.PlaySession)) {
                return true;
            } else {
                return false;
            }
        }
    }
}
