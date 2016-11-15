// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/15

using UnityEngine;

namespace EA4S.Test
{
    public class Tester_GlobalUI : MonoBehaviour
    {
        #region ActionFeedback

        public void ActionFeedback_Show(bool _feedback)
        {
            GlobalUI.I.ActionFeedback.Show(_feedback);
        }

        #endregion

        #region WidgetSubtitles

        public void Subtitles_DisplaySentence(bool _isKeeper)
        {
            GlobalUI.WidgetSubtitles.DisplaySentence(RandomTextId(), 3f, _isKeeper);
        }

        public void Subtitles_Close()
        {
            GlobalUI.WidgetSubtitles.Close();
        }

        string RandomTextId()
        {
            int rnd = UnityEngine.Random.Range(0, 5);
            switch (rnd) {
            case 0: return TextID.ASSESSMENT_START_A1.ToString();
            case 1: return TextID.ASSESSMENT_START_A2.ToString();
            case 2: return TextID.ASSESSMENT_START_A3.ToString();
            case 3: return TextID.ASSESSMENT_RESULT_GOOD.ToString();
            default: return TextID.ASSESSMENT_RESULT_INTRO.ToString();
            }
        }

        #endregion
    }
}