using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EA4S.Core;

namespace EA4S.Map
{
    public class Dialogues : MonoBehaviour
    {
        public int numberStage;
        int IsBeginningNewStage;
        void OnTriggerEnter(Collider other)
        {
            IsBeginningNewStage = PlayerPrefs.GetInt("IsNewStage"+numberStage);
            if ((other.gameObject.tag == "Player") && (IsBeginningNewStage==0))
            {
                Database.LocalizationDataId[] data = new Database.LocalizationDataId[7];
                data[2] = Database.LocalizationDataId.Map_Intro_Map2;
                data[3] = Database.LocalizationDataId.Map_Intro_Map3;
                data[4] = Database.LocalizationDataId.Map_Intro_Map4;
                data[5] = Database.LocalizationDataId.Map_Intro_Map5;
                data[6] = Database.LocalizationDataId.Map_Intro_Map6;
                KeeperManager.I.PlayDialog(data[numberStage], true, true);
                PlayerPrefs.SetInt("IsNewStage"+numberStage, 1);
            }
        }
    }
}
