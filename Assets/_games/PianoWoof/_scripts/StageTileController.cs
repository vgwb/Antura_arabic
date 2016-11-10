using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using EA4S;
using TMPro;

namespace EA4S.PianoWoof
{
    public class StageTileController : MonoBehaviour
    {
        public GameObject[] lights;
        public ILivingLetterData letterObject;
        public TMPro.TMP_Text letterText;
        public TMPro.TMP_Text tileText;


        public void SetLetter(LL_LetterData _data)
        {
            letterObject = _data;
            var text = _data.TextForLivingLetter;
            letterText.text = text;
            //tileText.text = text;
        }

        public void TurnLightOn()
        {
            SetLightVisibility(true);
        }

        public void TurnLightOff()
        {
            SetLightVisibility(false);
        }

        private void SetLightVisibility(bool lightsOn)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].SetActive(lightsOn);
            }
        }
    }
}
