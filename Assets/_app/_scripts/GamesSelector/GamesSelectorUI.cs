// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/01

using EA4S.Db;
using TMPro;
using UnityEngine;

namespace EA4S
{
    public class GamesSelectorUI : MonoBehaviour
    {
        public GameObject[] Stars;
        public TextMeshProUGUI TfImg, TfTitle;

        #region Unity

        void Start()
        {
            // Fill with data
            JourneyPosition journeyPos = AppManager.I.Player.CurrentJourneyPosition;
            TfTitle.text = journeyPos.ToString();
//            PlaySessionData playSessionData = AppManager.I.DB.GetPlaySessionDataById(journeyPos.PlaySession);
        }

        #endregion
    }
}