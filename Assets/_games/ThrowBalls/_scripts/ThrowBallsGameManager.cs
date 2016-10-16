using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using EA4S;

namespace EA4S.ThrowBalls
{
    public class ThrowBallsGameManager : MiniGameBase
    {
        public const int MAX_NUM_ROUNDS = 5;
        public const int NUM_LETTERS_IN_POOL = 3;
        public const int MAX_NUM_POKEBALLS = 3;

        new public static ThrowBallsGameManager Instance;
        new public ThrowBallsGameplayInfo GameplayInfo;

        public GameObject[] letterPool;
        private LetterController[] letterControllers;

        public GameObject pokeBall;
        public PokeballController pokeBallController;

        public GameObject letterWithPropsPrefab;

        public GameObject poofPrefab;

        public GameObject environment;

        private bool isRoundOngoing;

        // Round number is 1-based. (Round 1, round 2,...)
        private int roundNumber = 1;
        private int numPokeballs = MAX_NUM_POKEBALLS;

        private int numRoundsWon = 0;

        public GameObject endGameCanvas;
        public StarFlowers starFlowers;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        protected override void Start()
        {
            base.Start();

            AppManager.Instance.InitDataAI();
            AppManager.Instance.CurrentGameManagerGO = gameObject;
            SceneTransitioner.Close();

            UnityEngine.Random.seed = DateTime.Now.GetHashCode();

            // Layer 8 = Terrain. Layer 12 = Pokeball.
            Physics.IgnoreLayerCollision(8, 12);

            foreach (Collider collider in environment.GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }

            letterPool = new GameObject[NUM_LETTERS_IN_POOL];
            letterControllers = new LetterController[NUM_LETTERS_IN_POOL];

            for (int i = 0; i < letterPool.Length; i++)
            {
                GameObject letter = Instantiate(letterWithPropsPrefab).GetComponent<LetterWithPropsController>().letter;
                LetterController letterController = letter.GetComponent<LetterController>();

                letterPool[i] = letter;
                letterControllers[i] = letterController;

                letter.SetActive(false);
            }

            ResetScene();

            StartCoroutine("StartNewRound");

            //LoggerEA4S.Log("minigame", "template", "start", "");
            //LoggerEA4S.Save();
        }
        
        protected override void ReadyForGameplay()
        {
            base.ReadyForGameplay();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public void ResetScene()
        {
            UIController.instance.Reset();

            ArrayList positionIndices = new ArrayList();
            for (int i = 0; i < Constants.LETTER_POSITIONS.Length; i++)
            {
                positionIndices.Add(i);
            }

            foreach (LetterController letterController in letterControllers)
            {
                letterController.Reset();
                letterController.DisableProps();
            }

            foreach (GameObject letter in letterPool)
            {
                int randIndex = UnityEngine.Random.Range(0, positionIndices.Count);
                letter.transform.position = Constants.LETTER_POSITIONS[(int)positionIndices[randIndex]];
                positionIndices.RemoveAt(randIndex);
                letter.SetActive(false);
            }

            // Sort the letters according to y axis position:
            for (int i = 0; i < letterPool.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (letterPool[i].transform.position.y < letterPool[j].transform.position.y)
                    {
                        GameObject temp = letterPool[i];
                        letterPool[i] = letterPool[j];
                        letterPool[j] = temp;

                        // Important! Sort the controllers too:
                        LetterController tempController = letterControllers[i];
                        letterControllers[i] = letterControllers[j];
                        letterControllers[j] = tempController;
                    }
                }
            }

            pokeBallController.Reset();

            numPokeballs = MAX_NUM_POKEBALLS;

            isRoundOngoing = false;
        }

        public IEnumerator StartNewRound()
        {
            ResetScene();

            List<string> currentLettersInPlay = new List<string>();

            LetterData correctLetter = AppManager.Instance.Teacher.GimmeARandomLetter();

            AudioManager.I.PlayLetter(correctLetter.Key);

            yield return new WaitForSeconds(1);

            int numLettersInRound = GetNumLettersInRound();
            bool evenLevelsMoveRight = UnityEngine.Random.Range(0, 2) % 2 == 0;

            // Pick a random letter as the correct letter:
            int correctLetterIndex = UnityEngine.Random.Range(0, numLettersInRound);

            for (int i = 0; i < numLettersInRound; i++)
            {
                GameObject letterObj = letterPool[i];

                letterObj.SetActive(true);

                letterControllers[i].SetMotionVariation(GetMotionOfRound());

                letterControllers[i].SetPropVariation(GetPropOfRound());

                if (i == correctLetterIndex)
                {
                    letterObj.tag = Constants.TAG_CORRECT_LETTER;
                    letterControllers[i].SetLetter(correctLetter);

                    currentLettersInPlay.Add(correctLetter.Key);
                }

                else
                {
                    letterObj.tag = Constants.TAG_WRONG_LETTER;

                    LetterData wrongLetter;

                    do
                    {
                        wrongLetter = AppManager.Instance.Teacher.GimmeARandomLetter();
                    } while (currentLettersInPlay.Contains(wrongLetter.Key) || wrongLetter.Key == correctLetter.Key);

                    letterControllers[i].SetLetter(wrongLetter);

                    currentLettersInPlay.Add(wrongLetter.Key);
                }
            }

            isRoundOngoing = true;

            switch (roundNumber)
            {
                case 1:
                    UIController.instance.OnRoundStarted(correctLetter);
                    break;
                case 2:
                    UIController.instance.OnRoundStarted(correctLetter);
                    break;
                case 3:
                    UIController.instance.OnRoundStarted(correctLetter);
                    break;
                case 4:
                    UIController.instance.OnRoundStarted(correctLetter);
                    break;
                case 5:
                    UIController.instance.OnRoundStarted(correctLetter);
                    break;
                default:
                    break;
            }

            PokeballController.instance.Enable();
            UIController.instance.Enable();
        }

        public void OnCorrectLetterHit(LetterController correctLetterCntrl)
        {
            if (isRoundOngoing)
            {
                numRoundsWon++;
                StartCoroutine(ShowWinSequence(correctLetterCntrl));
                pokeBallController.Disable();

                isRoundOngoing = false;
            }
        }

        public void OnRoundLost()
        {
            if (isRoundOngoing)
            {
                DisplayRoundResult(false);
                pokeBallController.Disable();

                isRoundOngoing = false;
            }
        }

        private IEnumerator ShowWinSequence(LetterController correctLetterCntrl)
        {
            yield return new WaitForSeconds(0.1f);

            correctLetterCntrl.Vanish();
            correctLetterCntrl.Reset();

            yield return new WaitForSeconds(0.7f);

            AudioManager.I.PlayLetter(correctLetterCntrl.GetLetter().Key);

            correctLetterCntrl.SetMotionVariation(LetterController.MotionVariation.Idle);
            correctLetterCntrl.SetPropVariation(LetterController.PropVariation.Nothing);
            correctLetterCntrl.MoveTo(0, 6.44f, -29.4f);
            correctLetterCntrl.Show();

            yield return new WaitForSeconds(1.3f);

            DisplayRoundResult(true);
        }

        public void OnPokeballLost()
        {
            if (isRoundOngoing)
            {
                numPokeballs--;
                UIController.instance.OnPokeballLost();

                if (numPokeballs == 0)
                {
                    PokeballController.instance.Disable();
                    OnRoundLost();
                }
            }
        }

        private void DisplayRoundResult(bool win)
        {
            UIController.instance.Disable();

            if (win)
            {
                WidgetPopupWindow.I.ShowSentenceWithMark(OnRoundResultPressed, "comment_welldone", true, null);
            }

            else
            {
                WidgetPopupWindow.I.ShowSentenceWithMark(OnRoundResultPressed, "game_balloons_commentA", false, null);
            }
        }

        public void OnRoundResultPressed()
        {
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);
            WidgetPopupWindow.I.Close();

            roundNumber++;

            if (roundNumber > MAX_NUM_ROUNDS)
            {
                EndGame();
            }

            else
            {
                StartCoroutine("StartNewRound");
            }
        }

        private Vector3 GetCratePosition(Vector3 relativeToLetterPosition)
        {
            return new Vector3(relativeToLetterPosition.x, relativeToLetterPosition.y - 2.1f, relativeToLetterPosition.z);
        }

        private int GetNumLettersInRound()
        {
            return 3;
            return (roundNumber - 1) % 2 + 2;
        }

        private void EndGame()
        {
            StartCoroutine(EndGame_Coroutine());
        }

        private IEnumerator EndGame_Coroutine()
        {
            ResetScene();

            UIController.instance.Disable();

            yield return new WaitForSeconds(1f);

            endGameCanvas.gameObject.SetActive(true);

            int numberOfStars = 2;

            if (numRoundsWon <= 0)
            {
                numberOfStars = 0;
                WidgetSubtitles.I.DisplaySentence("game_result_retry");
            }
            else if (numRoundsWon <= 2)
            {
                numberOfStars = 1;
                WidgetSubtitles.I.DisplaySentence("game_result_fair");
            }
            else if (numRoundsWon <= 4)
            {
                numberOfStars = 2;
                WidgetSubtitles.I.DisplaySentence("game_result_good");
            }
            else
            {
                numberOfStars = 3;
                WidgetSubtitles.I.DisplaySentence("game_result_great");
            }

            Debug.Log("Stars: " + numberOfStars);
            starFlowers.Show(numberOfStars);
        }

        private LetterController.MotionVariation GetMotionOfRound()
        {
            switch (roundNumber)
            {
                case 1:
                    return LetterController.MotionVariation.Idle;
                case 2:
                    return LetterController.MotionVariation.Idle;
                case 3:
                    return LetterController.MotionVariation.Popping;
                case 4:
                    return LetterController.MotionVariation.Jumping;
                case 5:
                    return LetterController.MotionVariation.Idle;
                default:
                    return LetterController.MotionVariation.Idle;
            }
        }

        private LetterController.PropVariation GetPropOfRound()
        {
            switch (roundNumber)
            {
                case 1:
                    return LetterController.PropVariation.Nothing;
                case 2:
                    return LetterController.PropVariation.StaticPileOfCrates;
                case 3:
                    return LetterController.PropVariation.Bush;
                case 4:
                    return LetterController.PropVariation.StaticPileOfCrates;
                case 5:
                    return LetterController.PropVariation.SwervingPileOfCrates;
                default:
                    return LetterController.PropVariation.Nothing;
            }
        }
    }

    [Serializable]
    public class ThrowBallsGameplayInfo : AnturaGameplayInfo
    {
        [Tooltip("Play session duration in seconds.")]
        public float PlayTime = 0f;
    }
}