// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/15

using UnityEngine;

namespace EA4S.Test
{
    public class Tester_GlobalUI : MonoBehaviour
    {
        #region ActionFeedback

        public void Show(bool _feedback)
        {
            GlobalUI.I.ActionFeedback.Show(_feedback);
        }

        #endregion
    }
}