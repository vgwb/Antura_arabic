using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EA4S;

namespace EA4S.ThrowBalls
{
    public class ThrowBallsGameManager : MiniGameBase
    {
        public const int MAX_NUM_ROUNDS = 5;
        public const int NUM_LETTERS_IN_POOL = 3;
        public const int MAX_NUM_BALLS = 5;

        public const float TUTORIAL_UI_PERIOD = 4;

        new public static ThrowBallsGameManager Instance;
        new public ThrowBallsGameplayInfo GameplayInfo;

        public GameObject[] letterPool;
        private LetterController[] letterControllers;

        public GameObject ball;
        public BallController ballController;

        public GameObject letterWithPropsPrefab;

        public GameObject poofPrefab;

        public GameObject environment;

        public bool isRoundOngoing;

        // Round number is 1-based. (Round 1, round 2,...)
        // Round 0 is the tutorial round.
        private int roundNumber = 0;
        private int numBalls = MAX_NUM_BALLS;

        private int numRoundsWon = 0;

        public GameObject endGameCanvas;
        public StarFlowers starFlowers;

        private LetterSpawner letterSpawner;

        private float timeLeftToShowTutorialUI = TUTORIAL_UI_PERIOD;
        private bool isIdle = true;

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

            UnityEngine.Random.InitState(DateTime.Now.GetHashCode());

            // Layer 8 = Terrain. Layer 12 = Ball.
            Physics.IgnoreLayerCollision(8, 12);

            letterSpawner = new LetterSpawner();

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

            letterWithPropsPrefab.SetActive(false);

            ResetScene();

            StartCoroutine("StartNewRound");

            AudioManager.I.PlayMusic(Music.MainTheme);
            
            //LoggerEA4S.Log("minigame", "template", "start", "");
            //LoggerEA4S.Save();
        }

        void Touched()
        {
            isIdle = false;
            TutorialUI.Clear(false);
        }

        void OnMouseUp()
        {
            isIdle = true;
            timeLeftToShowTutorialUI = TUTORIAL_UI_PERIOD;
        }

        void Update()
        {
            if (roundNumber == 0)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            Touched();
                            break;
                        case TouchPhase.Ended:
                            OnMouseUp();
                            break;
                    }
                }

                else if (Input.GetMouseButtonDown(0))
                {
                    Touched();
                }

                else if (Input.GetMouseButtonUp(0))
                {
                    OnMouseUp();
                }
            }
        }

        void FixedUpdate()
        {
            if (isIdle && !BallController.instance.IsLaunched)
            {
                timeLeftToShowTutorialUI -= Time.fixedDeltaTime;

                if (timeLeftToShowTutorialUI <= 0)
                {
                    ShowTutorialUI();
                }
            }
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
            UIController.instance.Disable();

            foreach (LetterController letterController in letterControllers)
            {
                letterController.Reset();
                letterController.DisableProps();
            }

            if (roundNumber == 0)
            {
                Vector3 tutorialPosition = letterSpawner.GetTutorialPosition();
                GameObject letter = letterPool[0];
                letter.transform.position = tutorialPosition;
                letter.SetActive(false);
            }

            else
            {
                Vector3[] randomPositions = letterSpawner.GenerateRandomPositions(3);

                for (int i = 0; i < GetNumLettersInRound(); i++)
                {
                    GameObject letter = letterPool[i];
                    letter.transform.position = randomPositions[i];
                    letter.SetActive(false);
                }
            }

            ballController.Reset();

            numBalls = MAX_NUM_BALLS;

            if (roundNumber > 1)
            {
                MinigamesUI.Lives.ResetToMax();
            }
            
            isRoundOngoing = false;
        }

        public IEnumerator StartNewRound()
        {
            ResetScene();

            if (roundNumber == 1)
            {
                MinigamesUI.Init(MinigamesUIElement.Lives | MinigamesUIElement.Starbar);
                MinigamesUI.Lives.Setup(MAX_NUM_BALLS);
            }

            IQuestionPack newQuestionPack = ThrowBallsConfiguration.Instance.Questions.GetNextQuestion();

            LL_LetterData correctLetter = (LL_LetterData)newQuestionPack.GetCorrectAnswers().ToList()[0];
            List<ILivingLetterData> wrongLetters = newQuestionPack.GetWrongAnswers().ToList();

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
                }

                else
                {
                    letterObj.tag = Constants.TAG_WRONG_LETTER;

                    letterControllers[i].SetLetter((LL_LetterData)wrongLetters[0]);

                    wrongLetters.RemoveAt(0);
                }
            }

            isRoundOngoing = true;

            BallController.instance.Enable();

            if (roundNumber > 0)
            {
                UIController.instance.Enable();
                UIController.instance.SetLetterHint(correctLetter);
            }

            else
            {
                ShowTutorialUI();
            }
        }

        private void ShowTutorialUI()
        {
            TutorialUI.Clear(false);

            Vector3 worldToScreen = Camera.main.WorldToScreenPoint(new Vector3(0, 8, -20));
            Vector3 fromPoint = Camera.main.ScreenToWorldPoint(new Vector3(worldToScreen.x, worldToScreen.y, 20f));
            worldToScreen = Camera.main.WorldToScreenPoint(new Vector3(-0.75f, 4.5f, -22));
            Vector3 toPoint = Camera.main.ScreenToWorldPoint(new Vector3(worldToScreen.x, worldToScreen.y, 20f));
            TutorialUI.DrawLine(fromPoint, toPoint, TutorialUI.DrawLineMode.FingerAndArrow);
            timeLeftToShowTutorialUI = TUTORIAL_UI_PERIOD;
        }

        public void OnCorrectLetterHit(LetterController correctLetterCntrl)
        {
            if (isRoundOngoing)
            {
                if (roundNumber > 0)
                {
                    numRoundsWon++;

                    if (numRoundsWon == 2)
                    {
                        MinigamesUI.Starbar.GotoStar(0);
                    }

                    else if (numRoundsWon == 4)
                    {
                        MinigamesUI.Starbar.GotoStar(1);
                    }

                    else if (numRoundsWon == 5)
                    {
                        MinigamesUI.Starbar.GotoStar(2);
                    }
                }

                else
                {
                    TutorialUI.Clear(true);
                }

                StartCoroutine(ShowWinSequence(correctLetterCntrl));
                ballController.Disable();

                isRoundOngoing = false;
            }
        }

        public void OnRoundLost()
        {
            if (isRoundOngoing)
            {
                DisplayRoundResult(false);
                ballController.Disable();

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
            correctLetterCntrl.MoveTo(0, 15.7f, -31.6f);
            correctLetterCntrl.transform.rotation = Quaternion.Euler(-Camera.main.transform.rotation.eulerAngles.x, 180, 0);
            correctLetterCntrl.Show();

            yield return new WaitForSeconds(1.3f);

            DisplayRoundResult(true);
        }

        public void OnBallLost()
        {
            if (isRoundOngoing && roundNumber > 0)
            {
                numBalls--;

                MinigamesUI.Lives.SetCurrLives(numBalls);

                if (numBalls == 0)
                {
                    BallController.instance.Disable();
                    OnRoundLost();
                }
            }

            else if (roundNumber == 0)
            {
                ShowTutorialUI();
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
            if (roundNumber == 0)
            {
                return 1;
            }

            else
            {
                return 3;
            }
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
            switch (numRoundsWon + 1)
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
            switch (numRoundsWon + 1)
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

        public Vector3 GetPositionOfLetter(int index)
        {
            return letterControllers[index].gameObject.transform.position;
        }
    }

    [Serializable]
    public class ThrowBallsGameplayInfo : AnturaGameplayInfo
    {
        [Tooltip("Play session duration in seconds.")]
        public float PlayTime = 0f;
    }
}