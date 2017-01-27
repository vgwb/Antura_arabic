using EA4S.UI;
using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// Just used to test stuff here and there. Not to be included in final build
    /// </summary>
    public class DTester : MonoBehaviour
    {
        #if UNITY_EDITOR
        void Update()
        {
            // SceneTransitioner - SPACE to show/hide
            if (Input.GetKeyDown(KeyCode.Space)) {
                SceneTransitioner.Show(!SceneTransitioner.IsShown);
            }

            // Subtitles - T to show text, SHIFT+T to show keeper text, CTRL/CMD+T to close
            if (WidgetSubtitles.I != null) {
                if (Input.GetKeyDown(KeyCode.T)) {
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
                    {
                        WidgetSubtitles.I.Close();
                    }
                    else
                    {
                        var testData = new Database.LocalizationData();
                        testData.Arabic = "من فضلك، حاول اصطياد البعض منها. من فضلك، حاول التقاطها.";
                        WidgetSubtitles.I.DisplaySentence(testData, 2, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                    }
                }
            }

            // Continue button - C to show, SHIFT+C to show fullscreen-button on the side
            if (WidgetSubtitles.I != null) {
                if (Input.GetKeyDown(KeyCode.C)) {
                    ContinueScreenMode continueScreenMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)
//                        ? ContinueScreenMode.FullscreenBg : ContinueScreenMode.ButtonWithBgFullscreen;
                        ? ContinueScreenMode.ButtonFullscreen : ContinueScreenMode.ButtonWithBg;
                    ContinueScreen.Show(null, continueScreenMode);
                }
            }

            // Popup - P to show/hide
            if (Input.GetKeyDown(KeyCode.P)) {
                WidgetPopupWindow.I.Show(!WidgetPopupWindow.IsShown);
            }
        }
        #endif
    }
}