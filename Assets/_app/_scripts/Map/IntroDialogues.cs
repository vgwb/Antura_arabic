using Antura.Core;
using Antura.Database;
using UnityEngine;

namespace Antura.Map
{
    public class IntroDialogues : MonoBehaviour
    {
        public int numberStage;
        public bool dialoguePlayed;

        private void OnTriggerEnter(Collider other)
        {
            var isMaxPosition = AppManager.I.Player.IsAtMaxJourneyPosition();
            if (other.gameObject.CompareTag("Player") && isMaxPosition && numberStage > 1 && !dialoguePlayed)
            {
                var data = new LocalizationDataId[7];
                data[2] = LocalizationDataId.Map_Intro_Map2;
                data[3] = LocalizationDataId.Map_Intro_Map3;
                data[4] = LocalizationDataId.Map_Intro_Map4;
                data[5] = LocalizationDataId.Map_Intro_Map5;
                data[6] = LocalizationDataId.Map_Intro_Map6;
                KeeperManager.I.PlayDialog(data[numberStage], true, true);
                dialoguePlayed = true;
            }
        }
    }
}