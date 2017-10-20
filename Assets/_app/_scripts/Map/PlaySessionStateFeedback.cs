using Antura.Core;
using TMPro;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    /// Gives feedback on the state of a play session.
    /// </summary>
    public class PlaySessionStateFeedback : MonoBehaviour
    {
        private JourneyPosition journeyPosition;
        private PlaySessionState playSessionState;

        [Header("Visuals")]
        public MeshRenderer scoreFeedbackMR;
        public TextMeshPro journeyPosTextUI;

        public void Initialise(JourneyPosition _journeyPosition, PlaySessionState _playSessionState)
        {
            journeyPosition = _journeyPosition;
            playSessionState = _playSessionState;

            HandleJourneyPosition(journeyPosition);
            HandlePlaySessionState(playSessionState);
        }

        private void HandlePlaySessionState(PlaySessionState playSessionState)
        {
            int score = 0;
            if (playSessionState != null && playSessionState.scoreData != null) score = (int)playSessionState.scoreData.GetScore();

            var mat = scoreFeedbackMR.GetComponentInChildren<MeshRenderer>().material;
            switch (score)
            {
                case 0:
                    mat.color = Color.black;
                    break;
                case 1:
                    mat.color = Color.red;
                    break;
                case 2:
                    mat.color = Color.blue;
                    break;
                case 3:
                    mat.color = Color.yellow;
                    break;
            }
        }

        private void HandleJourneyPosition(JourneyPosition journeyPosition)
        {
            journeyPosTextUI.text = journeyPosition.ToString();
        }

        #region Show / Hide

        public void Highlight(bool choice)
        {
            if (choice)
            {
                ShowHighlightedInfo();
            }
            else
            {
                ShowUnhighlightedInfo();
            }
        }

        public void HideAllInfo()
        {
            journeyPosTextUI.gameObject.SetActive(false);
            scoreFeedbackMR.gameObject.SetActive(false);
        }

        public void ShowHighlightedInfo()
        {
            // TODO: show info with the player on that PS
            journeyPosTextUI.gameObject.SetActive(false);
            scoreFeedbackMR.gameObject.SetActive(true);
        }

        public void ShowUnhighlightedInfo()
        {
            // TODO: show info with the player not on that PS
            journeyPosTextUI.gameObject.SetActive(true);
            scoreFeedbackMR.gameObject.SetActive(true);
        }

        #endregion

    }
}