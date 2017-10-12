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
        public MeshRenderer feedbackMR;
        public TextMeshPro textUI;

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

            var mat = feedbackMR.GetComponentInChildren<MeshRenderer>().material;
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
            textUI.text = journeyPosition.ToString();
        }

        #region Show / Hide

        public void Highlight(bool choice)
        {
            if (choice)
            {
                ShowFullInfo();
            }
            else
            {
                ShowBasicInfo();
            }
        }

        private void ShowFullInfo()
        {
            // TODO: show stars and numbers and text and so on...
            textUI.gameObject.SetActive(false);
        }

        private void ShowBasicInfo()
        {
            // TODO: show just some small info
            textUI.gameObject.SetActive(true);
        }

        #endregion

    }
}