using EA4S.Audio;
using EA4S.Core;
using EA4S.UI;
using UnityEngine;

namespace EA4S.Scenes
{
    public class ReservedAreaScene : MonoBehaviour
    {

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true);
        }


        public void BtnOpenUrlWebsite()
        {
            Application.OpenURL(AppConstants.UrlWebsite);
        }

        public void BtnOpenUrlPrivacy()
        {
            Application.OpenURL(AppConstants.UrlPrivacy);
        }

    }
}