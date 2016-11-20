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

            str = "hello player (ID: " + AppManager.Instance.Player.Id + ")\n";
            str += "you're now in LB " + AppManager.Instance.Player.CurrentJourneyPosition + "\n";
            str += "your max LB is " + AppManager.Instance.Player.MaxJourneyPosition + "\n";
            str += "Mood " + AppManager.Instance.Player.MainMood + "\n";

            if (AppManager.Instance.Player.Precision != 0f) { str += "Precision " + AppManager.Instance.Player.Precision + "\n"; }
            if (AppManager.Instance.Player.Reaction != 0f) { str += "Reaction " + AppManager.Instance.Player.Reaction + "\n"; }
            if (AppManager.Instance.Player.Memory != 0f) { str += "Memory " + AppManager.Instance.Player.Memory + "\n"; }
            if (AppManager.Instance.Player.Logic != 0f) { str += "Logic " + AppManager.Instance.Player.Logic + "\n"; }
            if (AppManager.Instance.Player.Rhythm != 0f) { str += "Rhythm " + AppManager.Instance.Player.Rhythm + "\n"; }
            if (AppManager.Instance.Player.Musicality != 0f) { str += "Musicality " + AppManager.Instance.Player.Musicality + "\n"; }
            if (AppManager.Instance.Player.Sight != 0f) { str += "Sight " + AppManager.Instance.Player.Sight + "\n"; }

            output.text = str;
            //AppManager.Instance.DB.GetLocalizationDataById("Game_Title").Arabic;
            //output.text += "\n" + AppManager.Instance.DB.GetLocalizationDataById("Game_Title2").Arabic;

        }

    }
}