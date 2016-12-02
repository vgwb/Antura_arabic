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
            if (!journeyPos.isMinor(AppManager.I.Player.MaxJourneyPosition)) {
                // First time playing this session: 0 stars
                SetStars(0);
            } else {
                int unlockedRewards = RewardSystemManager.GetUnlockedRewardForPlaysession(AppManager.I.Player.CurrentJourneyPosition.ToString());
                SetStars(unlockedRewards + 1);
            }
//            PlaySessionData playSessionData = AppManager.I.DB.GetPlaySessionDataById(journeyPos.PlaySession);
        }

        #endregion

        #region Methods

        void SetStars(int _tot)
        {
            if (_tot > 3) _tot = 3;
            foreach (GameObject star in Stars) star.SetActive(false);
            for (int i = 0; i < _tot; ++i) Stars[i].SetActive(true);
        }

        #endregion
    }
}