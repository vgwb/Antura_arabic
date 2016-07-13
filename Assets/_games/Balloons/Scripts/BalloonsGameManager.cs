using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using EA4S;

namespace Balloons
{
    public class BalloonsGameManager: MonoBehaviour
    {
        public WordPromptController wordPrompt;
        public GameObject balloonPrefab;
        public Transform[] floatingLetterLocations;
        public Canvas resultsCanvas;
        public Text resultsText;
        public GameObject nextButton;
        public GameObject retryButton;
        public Animator countdownAnimator;
        public AudioSource music;
        public TimerManager timer;
        public AnimationClip balloonPopAnimation;
        public Color[] balloonColors;


        [HideInInspector]
        public List<FloatingLetterController> balloons;
        [HideInInspector]
        public float letterDropDelay;
        [HideInInspector]
        public float letterAnimationLength = 0.367f;

        private string word;
        private List<LetterData> wordLetters;
        private float fullMusicVolume;

        public static BalloonsGameManager instance;

        private enum Result
        {
            PERFECT,
            GOOD,
            CLEAR,
            TRYAGAIN
        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            Random.seed = System.DateTime.Now.GetHashCode();
            letterDropDelay = balloonPopAnimation.length;
            fullMusicVolume = music.volume;
            ResetScene();
            BeginGameplay();
        }

        public void StartNewRound()
        {
            ResetScene();
            BeginGameplay();
        }

        private void ResetScene()
        {
            music.Stop();
            music.volume = fullMusicVolume;
            timer.StopTimer();
            timer.ResetTimer();
            wordPrompt.Reset();
            resultsCanvas.gameObject.SetActive(false);
            DestroyAllBalloons();
        }

        private void BeginGameplay()
        {
            StartCoroutine(BeginGameplay_Coroutine());
        }

        private IEnumerator BeginGameplay_Coroutine()
        {
            AnimateCountdown("3");
            yield return new WaitForSeconds(1f);
            AnimateCountdown("2");
            yield return new WaitForSeconds(1f);
            AnimateCountdown("1");
            yield return new WaitForSeconds(1f);

            SetNewWord();
            CreateBalloons();

            timer.StartTimer();
            music.Play();
        }

        private void AnimateCountdown(string text)
        {
            countdownAnimator.gameObject.GetComponent<Text>().text = text;
            countdownAnimator.SetTrigger("Count");
        }

        private void SetNewWord()
        {
            word = Google2u.words.Instance.Rows.GetRandomElement()._word;
            wordLetters = ArabicAlphabetHelper.LetterDataListFromWord(word, AppManager.Instance.Letters);
            wordPrompt.DisplayWord(wordLetters);

            Debug.Log(word + " Length: " + word.Length);
        }

        private void CreateBalloons()
        {
            // Create balloons
            for (int i = 0; i < floatingLetterLocations.Length; i++)
            {
                var balloon = Instantiate(balloonPrefab);
                balloon.transform.SetParent(floatingLetterLocations[i]);
                balloon.transform.localPosition = Vector3.zero;
                var balloonController = balloon.GetComponent<FloatingLetterController>();

                balloonController.SetActiveVariation(Random.Range(0, balloonController.variations.Length));

                var balloonTops = balloonController.ActiveVariation.balloonTops;
                var letter = balloonController.letter;

                // Set random balloon colors
                for (int j = 0; j < balloonTops.Length; j++)
                {
                    balloonTops[j].SetColor(balloonColors[Random.Range(0, balloonColors.Length)]);
                }

                // Get a random letter that is not a required letter
                LetterData randomLetter;
                do
                {
                    randomLetter = AppManager.Instance.Letters.GetRandomElement();
                } while (wordLetters.Contains(randomLetter));
                letter.Init(randomLetter);

                balloons.Add(balloonController);
            }

            // Assign required letters
            List<int> positions = new List<int>();
            for (int i = 0; i < wordLetters.Count; i++)
            {
                var position = Random.Range(0, balloons.Count);

                if (!positions.Contains(position))
                {
                    positions.Add(position);
                    var balloonLetter = balloons[position].GetComponent<FloatingLetterController>().letter;
                    balloonLetter.associatedPromptIndex = i;
                    balloonLetter.Init(wordLetters[i]);
                    balloonLetter.isRequired = true;
                }
                else
                {
                    i--;
                }
            }
        }

        public void OnPoppedGroup()
        {
            CheckRemainingBalloons();
        }

        public void OnPoppedRequired(int promptIndex)
        {
            wordPrompt.letterPrompts[promptIndex].State = LetterPromptController.PromptState.WRONG;
        }

        private void CheckRemainingBalloons()
        {
            int idlePromptsCount = wordPrompt.IdleLetterPrompts.Count;
            bool randomBalloonsExist = balloons.Exists(balloon => balloon.letter.isRequired == false);
            bool requiredBalloonsExist = balloons.Exists(balloon => balloon.letter.isRequired == true);

            if (!requiredBalloonsExist)
            {
                ShowResults(Result.TRYAGAIN);
                DisableBalloons();
            }
            else if (!randomBalloonsExist)
            {
                Result result;
                if (idlePromptsCount == wordLetters.Count)
                {
                    result = Result.PERFECT;
                }
                else if (idlePromptsCount >= 2)
                {
                    result = Result.GOOD;
                }
                else
                {
                    result = Result.CLEAR;
                }
                ShowResults(result);
                DisableBalloons();
            }
        }

        private void DisableBalloons()
        {
            for (int i = 0; i < balloons.Count; i++)
            {
                foreach (var balloonTop in balloons[i].ActiveVariation.balloonTops)
                {
                    balloonTop.balloonCollider.enabled = false;
                }
            }
        }

        private void DestroyAllBalloons()
        {
            for (int i = 0; i < balloons.Count; i++)
            {
                Destroy(balloons[i].gameObject);
            }
            balloons.Clear();
        }

        private void DestroyUnrequiredBalloons()
        {
            for (int i = 0; i < balloons.Count; i++)
            {
                if (!balloons[i].letter.isRequired)
                {
                    Destroy(balloons[i]);
                }
            }
        }

        public void OnTimeUp()
        {
            bool randomBalloonsExist = balloons.Exists(balloon => balloon.letter.isRequired == false);

            if (randomBalloonsExist)
            {
                ShowResults(Result.TRYAGAIN);
            }
            else
            {
                OnPoppedGroup();
            }
        }

        private void ShowResults(Result result)
        {
            music.volume = 0.25f * music.volume;
            timer.StopTimer();

            resultsCanvas.gameObject.SetActive(true);

            switch (result)
            {
                case Result.PERFECT:
                    resultsText.text = "PERFECT! (3 Stars)";
                    nextButton.SetActive(true);
                    retryButton.SetActive(false);
                    break;
                case Result.GOOD:
                    resultsText.text = "GOOD! (2 Stars)";
                    nextButton.SetActive(true);
                    retryButton.SetActive(false);
                    break;
                case Result.CLEAR:
                    resultsText.text = "CLEAR! (1 Star)";
                    nextButton.SetActive(true);
                    retryButton.SetActive(false);
                    break;
                case Result.TRYAGAIN:
                    resultsText.text = "TRY AGAIN";
                    nextButton.SetActive(false);
                    retryButton.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}
