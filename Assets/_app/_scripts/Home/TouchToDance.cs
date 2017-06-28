using EA4S.Audio;
using System.Collections;
using System.Collections.Generic;
using EA4S.Core;
using UnityEngine;

namespace EA4S.LivingLetters
{
    public class TouchToDance : MonoBehaviour
    {
        public void OnMouseDown()
        {
            var view = GetComponent<LivingLetterController>();
            view.ToggleDance();
            var letter = AppManager.I.Teacher.GetRandomTestLetterLL(useMaxJourneyData: true);
            view.Init(letter);

            AudioManager.I.PlayLetter(letter.Data, true);
        }
    }
}