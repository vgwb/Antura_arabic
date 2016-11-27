using UnityEngine;
using UnityEngine.UI;
using EA4S.Db;

namespace EA4S
{


    public class PlayerPanel : MonoBehaviour
    {
        public TextRender output;

        void Start()
        {
            var str = "";

            str = "hello player (ID: " + AppManager.I.Player.Id + ")\n";
            str += "you're now in LB " + AppManager.I.Player.CurrentJourneyPosition + "\n";
            str += "your max LB is " + AppManager.I.Player.MaxJourneyPosition + "\n";
            str += "Mood " + AppManager.I.Player.MainMood + "\n";

            if (AppManager.I.Player.Precision != 0f) { str += "Precision " + AppManager.I.Player.Precision + "\n"; }
            if (AppManager.I.Player.Reaction != 0f) { str += "Reaction " + AppManager.I.Player.Reaction + "\n"; }
            if (AppManager.I.Player.Memory != 0f) { str += "Memory " + AppManager.I.Player.Memory + "\n"; }
            if (AppManager.I.Player.Logic != 0f) { str += "Logic " + AppManager.I.Player.Logic + "\n"; }
            if (AppManager.I.Player.Rhythm != 0f) { str += "Rhythm " + AppManager.I.Player.Rhythm + "\n"; }
            if (AppManager.I.Player.Musicality != 0f) { str += "Musicality " + AppManager.I.Player.Musicality + "\n"; }
            if (AppManager.I.Player.Sight != 0f) { str += "Sight " + AppManager.I.Player.Sight + "\n"; }

            output.text = str;
            //AppManager.I.DB.GetLocalizationDataById("Game_Title").Arabic;
            //output.text += "\n" + AppManager.I.DB.GetLocalizationDataById("Game_Title2").Arabic;

        }

    }
}