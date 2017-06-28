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
            bool isMaxPosition = AppManager.I.Player.IsAtMaxJourneyPosition();
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
       
    }
}
