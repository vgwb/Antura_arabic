using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;

namespace EA4S.ThrowBalls
{
    public class UIController : MonoBehaviour
    {
        public static UIController instance;
        
        public GameObject letterHint;
        public TMP_Text letterHintText;

        private int numPokeballs;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {

        }

        public void SetLetterHint(LL_LetterData _data)
        {
            letterHintText.text = _data.TextForLivingLetter;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }
    }
}

