using UnityEngine;
using TMPro;

namespace EA4S.MixedLetters
{
    public class UIController : MonoBehaviour
    {
        public static UIController instance;
        public TMP_Text timerText;

        void Awake()
        {
            instance = this;
        }
        
        public void SetTimer(int time)
        {
            timerText.SetText(time + "");
        }

        public void EnableTimer()
        {
            timerText.gameObject.SetActive(true);
        }

        public void DisableTimer()
        {
            timerText.gameObject.SetActive(false);
        }
    }
}