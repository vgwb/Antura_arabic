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

    [HideInInspector]
    public Plane inputPlane;

    [HideInInspector]
    public List<BalloonController> balloons;

    private string word;
    private List<LetterData> wordLetters;

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
        PrepareRound();
        inputPlane = new Plane(Vector3.back, Vector3.zero);
    }

    void PrepareRound()
    {
        Random.seed = System.DateTime.Now.GetHashCode();

        word = Google2u.words.Instance.Rows.GetRandomElement()._word;
        wordLetters = ArabicAlphabetHelper.LetterDataListFromWord(word, AnturaGameManager.Instance.Letters);

        Debug.Log(word + " Length: " + word.Length);
        Debug.Log("Letters: " + wordLetters.Count);

        wordPrompt.Reset();
        wordPrompt.DisplayWord(wordLetters);
        CreateBalloons();

    }

    void CreateBalloons()
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
            Result result;
            if (idlePromptsCount == wordLetters.Count)
            {
                result = Result.PERFECT;
            }
            else if (idlePromptsCount > 2)
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
        //for (int i = 0; i < balloons.Count; i++)
        //{
        //    balloons[i].balloonTop.balloonCollider.enabled = false;
        //}
    }

    private void ShowResults(Result result)
    {
        resultsCanvas.gameObject.SetActive(true);

        switch (result)
        {
            case Result.PERFECT:
                resultsText.text = "PERFECT!";
                nextButton.SetActive(true);
                retryButton.SetActive(false);
                break;
            case Result.GOOD:
                resultsText.text = "GOOD!";
                nextButton.SetActive(true);
                retryButton.SetActive(false);
                break;
            case Result.CLEAR:
                resultsText.text = "CLEAR";
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

    public void Restart()
    {
        SceneManager.LoadScene("game_Balloons");
    }
}
