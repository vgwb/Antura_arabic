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
            output.text = AppManager.Instance.DB.GetLocalizationDataById("Game_Title").Arabic;
            output.text += "\n" + AppManager.Instance.DB.GetLocalizationDataById("Game_Title2").Arabic;

        }

    }
}