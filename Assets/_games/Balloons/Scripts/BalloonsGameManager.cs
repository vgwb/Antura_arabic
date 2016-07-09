using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using CGL.Antura;

public class BalloonsGameManager: MonoBehaviour
{

    public WordPromptController wordPrompt;
    public GameObject[] balloonPrefabs;
    public Transform[] balloonLocations;
    public Canvas resultsCanvas;
    public Text resultsText;
    public GameObject nextButton;
    public GameObject retryButton;
    public Animator countdownAnimator;
    public AudioSource music;
    public Balloons.TimerManager timer;

    [HideInInspector]
    public List<BalloonController> balloons;

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
        DestroyBalloons();
    }

    private void BeginGameplay()
    {
        StartCoroutine(BeginRound_Coroutine());
    }

    private IEnumerator BeginRound_Coroutine()
    {
        AnimateCountdown("3");
        yield return new WaitForSeconds(1f);
        AnimateCountdown("2");
        yield return new WaitForSeconds(1f);
        AnimateCountdown("1");
        yield return new WaitForSeconds(1f);

        word = Google2u.words.Instance.Rows.GetRandomElement()._word;
        wordLetters = ArabicAlphabetHelper.LetterDataListFromWord(word, AnturaGameManager.Instance.Letters);
        Debug.Log(word + " Length: " + word.Length);

        wordPrompt.DisplayWord(wordLetters);
        CreateBalloons();

        timer.StartTimer();
        music.Play();
    }

    private void AnimateCountdown(string text)
    {
        countdownAnimator.gameObject.GetComponent<Text>().text = text;
        countdownAnimator.SetTrigger("Count");
    }

    private void CreateBalloons()
    {
        // Create balloons
        for (int i = 0; i < balloonLocations.Length; i++)
        {
            var balloon = Instantiate(balloonPrefabs[Random.Range(0, 3)]);
            balloon.transform.SetParent(balloonLocations[i]);
            balloon.transform.localPosition = Vector3.zero;
            //balloon.GetComponent<BalloonController>()
            var balloonController = balloon.GetComponent<BalloonController>();
            var balloonLetter = balloonController.letter;
            // Get a random letter that is not a required letter
            LetterData randomLetter;
            do
            {
                randomLetter = AnturaGameManager.Instance.Letters.GetRandomElement();
            } while (wordLetters.Contains(randomLetter));

            balloonLetter.Init(randomLetter);
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
                var balloonLetter = balloons[position].GetComponent<BalloonController>().letter;
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

    public void CheckRemainingBalloons()
    {
        int idlePromptsCount = wordPrompt.IdleLetterPrompts.Count;
        bool promptAllWrong = idlePromptsCount == 0;
        bool randomBalloonsExist = balloons.Exists(balloon => balloon.letter.isRequired == false);
        bool requiredBalloonsExist = balloons.Exists(balloon => balloon.letter.isRequired == true);

        if (!requiredBalloonsExist)
        {
            ShowResults(Result.TRYAGAIN);
            DisableBalloons();
        }
        else if (!randomBalloonsExist)
        {
            Debug.Log("IDLE: " + idlePromptsCount + " LETTERS: " + wordLetters.Count);

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
            balloons[i].balloonTop.balloonCollider.enabled = false;
        }
    }

    private void DestroyBalloons()
    {
        for (int i = 0; i < balloons.Count; i++)
        {
            Destroy(balloons[i].gameObject);
        }
        balloons.Clear();
    }

    private void DestroyRandomBalloons()
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
            CheckRemainingBalloons();
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

    public void OnPoppedRequired(int promptIndex)
    {
        wordPrompt.letterPrompts[promptIndex].State = LetterPromptController.PromptState.WRONG;
    }
}
