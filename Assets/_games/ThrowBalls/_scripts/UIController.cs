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
        public Text messageText;
        public Text variationText;

        public GameObject letterHint;
        public TMP_Text letterHintText;

        private int numPokeballs;

        void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {
            string message = "Here's some extra pokeballs because it's ";

            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    message += "Monday.";
                    break;
                case DayOfWeek.Tuesday:
                    message += "Tuesday.";
                    break;
                case DayOfWeek.Wednesday:
                    message += "Wednesday.";
                    break;
                case DayOfWeek.Thursday:
                    message += "Thursday.";
                    break;
                case DayOfWeek.Friday:
                    message += "Friday.";
                    break;
                case DayOfWeek.Saturday:
                    message += "Saturday.";
                    break;
                case DayOfWeek.Sunday:
                    message += "Sunday.";
                    break;
            }

            messageText.text = message;

            Reset();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnPokeballLost()
        {
            pokeballImages[--numPokeballs].sprite = pokeballOffSprite;
        }

        public void Reset()
        {
            numPokeballs = ThrowBallsGameManager.MAX_NUM_POKEBALLS;

            foreach (Image image in pokeballImages)
            {
                image.sprite = pokeballOnSprite;
            }

            messageText.enabled = false;

            letterHint.SetActive(false);

            StopAllCoroutines();
        }

        public void GrantedFreeBalls()
        {
            StartCoroutine("ShowMessage");
        }

        public void OnRoundStarted(string roundText, LetterData _data)
        {
            variationText.text = roundText;
            letterHint.SetActive(true);
            letterHintText.text = _data.TextForLivingLetter;
        }

        private IEnumerator ShowMessage()
        {
            messageText.enabled = true;
            yield return new WaitForSeconds(1.5f);
            messageText.enabled = false;
        }

        public void HideMessage()
        {
            messageText.enabled = false;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}

