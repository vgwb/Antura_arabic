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

        public Image[] pokeballImages;
        public Sprite pokeballOnSprite;
        public Sprite pokeballOffSprite;
        public GameObject letterHint;
        public TMP_Text letterHintText;

        private int numPokeballs;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            Reset();
        }

        public void Reset()
        {
            numPokeballs = ThrowBallsGameManager.MAX_NUM_POKEBALLS;

            foreach (Image image in pokeballImages)
            {
                image.sprite = pokeballOnSprite;
            }
            
            letterHint.SetActive(false);

            StopAllCoroutines();
        }

        public void OnPokeballLost()
        {
            //pokeballImages[--numPokeballs].sprite = pokeballOffSprite;
        }

        public void OnRoundStarted(LL_LetterData _data)
        {
            letterHint.SetActive(true);
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

