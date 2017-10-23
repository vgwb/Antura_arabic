using System.Collections.Generic;
using System.Linq;
using Antura.Core;
using Antura.UI;
using DG.DeInspektor.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.Map
{
    /// <summary>
    /// UI that appears when you select a Pin.
    /// </summary>
    public class MapPlayPanel : MonoBehaviour
    {
        public RectTransform rectTr;

        public Button playBtn;
        public Button lockedBtn;

        public TextRender psNumberTextUI;
        public TextRender psNameTextUI;

        public void SetPin(Pin pin)
        {
            rectTr.anchoredPosition = Camera.main.WorldToScreenPoint(pin.transform.position);

            var lbData =
                AppManager.I.DB.FindLearningBlockData(
                    x => x.Stage == pin.journeyPosition.Stage && x.LearningBlock == pin.journeyPosition.LearningBlock)[0];

            playBtn.gameObject.SetActive(!pin.isLocked);

            lockedBtn.gameObject.SetActive(pin.isLocked);

            psNumberTextUI.SetText(pin.journeyPosition.GetShortTitle());
            psNameTextUI.SetText(lbData.Title_Ar);
        }

    }
}