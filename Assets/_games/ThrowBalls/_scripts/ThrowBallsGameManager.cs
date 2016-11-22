using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EA4S;

namespace EA4S.ThrowBalls
{
    public class ThrowBallsGameManager : MiniGame
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

        private LetterSpawner letterSpawner;

        private float timeLeftToShowTutorialUI = TUTORIAL_UI_PERIOD;
        private bool isIdle = true;

        protected override void OnInitialize(IGameContext context)
        { }

        protected override IGameConfiguration GetConfiguration()
        {
            return ThrowBallsConfiguration.Instance;
        }

        protected override IGameState GetInitialState()
        {
            return null;
        }

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
            Physics.IgnoreLayerCollision(8, 10);

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

        public bool IsTutorialLevel()
        {
            return roundNumber == 0;
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
            if (roundNumber == 0 && isIdle && !BallController.instance.IsLaunched())
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
            UIController.instance.Reset();
            UIController.instance.Disable();

            foreach (LetterController letterController in letterControllers)
            {
                letterController.Reset();
                letterController.DisableProps();
            }

            Vector3[] randomPositions = letterSpawner.GenerateRandomPositions(3, roundNumber == 0);

            for (int i = 0; i < GetNumLettersInRound(); i++)
            {
                GameObject letter = letterPool[i];
                letter.transform.position = randomPositions[i];
                letter.SetActive(false);
            }

            ballController.Reset();

            numBalls = MAX_NUM_BALLS;

            if (roundNumber > 1)
            {
                MinigamesUI.Lives.ResetToMax();
            }

            isRoundOngoing = false;
        }

        private void DisableLetters(bool disablePropsToo)
        {
            foreach (LetterController letterController in letterControllers)
            {
                letterController.Disable();
                if (disablePropsToo)
                {
                    letterController.DisableProps();
                }
            }
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

            AudioManager.I.PlayLetter(correctLetter.Id);

            yield return new WaitForSeconds(1f);

            int numLettersInRound = GetNumLettersInRound();

            for (int i = 0; i < numLettersInRound; i++)
            {
                GameObject letterObj = letterPool[i];

                letterObj.SetActive(true);

                letterControllers[i].SetMotionVariation(GetMotionOfRound());

                letterControllers[i].SetPropVariation(GetPropOfRound());

                if (i == 0)
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

            UIController.instance.Enable();
            UIController.instance.SetLetterHint(correctLetter);

            if (roundNumber == 0)
            {
                ShowTutorialUI();
            }
        }

        private void ShowTutorialUI()
        {
            TutorialUI.Clear(false);

            Vector3 worldToScreen = Camera.main.WorldToScreenPoint(new Vector3(0, 8, -20));
            Vector3 fromPoint = Camera.main.ScreenToWorldPoint(new Vector3(worldToScreen.x, worldToScreen.y, 20f));
            worldToScreen = Camera.main.WorldToScreenPoint(new Vector3(-1.5f, 4.5f, -22));
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

                    if (numRoundsWon == 1)
                    {
                        MinigamesUI.Starbar.GotoStar(0);
                    }

                    else if (numRoundsWon == 3)
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
                ballController.Disable();
                isRoundOngoing = false;
                DisableLetters(true);

                StartCoroutine(OnRoundLostCoroutine());
            }
        }

        private IEnumerator OnRoundLostCoroutine()
        {
            AudioManager.I.PlaySfx(Sfx.Lose);
            yield return new WaitForSeconds(3f);
            OnRoundConcluded();
        }

        private IEnumerator ShowWinSequence(LetterController correctLetterCntrl)
        {
            yield return new WaitForSeconds(0.1f);

            correctLetterCntrl.Vanish();
            correctLetterCntrl.Reset();

            yield return new WaitForSeconds(0.7f);

            AudioManager.I.PlayLetter(correctLetterCntrl.GetLetter().Id);

            correctLetterCntrl.SetMotionVariation(LetterController.MotionVariation.Idle);
            correctLetterCntrl.SetPropVariation(LetterController.PropVariation.Nothing);
            correctLetterCntrl.MoveTo(0, 13.5f, -33f);
            correctLetterCntrl.transform.rotation = Quaternion.Euler(-Camera.main.transform.rotation.eulerAngles.x, 180, 0);
            correctLetterCntrl.Show();
            correctLetterCntrl.letterObjectView.DoHorray();
            correctLetterCntrl.ShowVictoryRays();

            AudioManager.I.PlaySfx(Sfx.Win);

            yield return new WaitForSeconds(3f);

            OnRoundConcluded();
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

        public void OnRoundConcluded()
        {
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

            int numberOfStars = 2;

            if (numRoundsWon == 0)
            {
                numberOfStars = 0;
            }
            else if (numRoundsWon == 1 || numRoundsWon == 2)
            {
                numberOfStars = 1;
            }
            else if (numRoundsWon == 3 || numRoundsWon == 4)
            {
                numberOfStars = 2;
            }
            else
            {
                numberOfStars = 3;
            }

            EndGame(numberOfStars, 0);
        }

        private LetterController.MotionVariation GetMotionOfRound()
        {
            if (roundNumber == 0)
            {
                return LetterController.MotionVariation.Idle;
            }

            float normalizedDifficulty = (numRoundsWon + 1) * 0.8f * ThrowBallsConfiguration.Instance.Difficulty;

            if (normalizedDifficulty <= 0.6f)
            {
                return LetterController.MotionVariation.Idle;
            }

            else if (normalizedDifficulty <= 1.2f)
            {
                return LetterController.MotionVariation.Idle;
            }

            else if (normalizedDifficulty <= 1.8f)
            {
                return LetterController.MotionVariation.Popping;
            }

            else if (normalizedDifficulty <= 2.4f)
            {
                return LetterController.MotionVariation.Jumping;
            }

            else
            {
                return LetterController.MotionVariation.Idle;
            }
        }

        private LetterController.PropVariation GetPropOfRound()
        {
            if (roundNumber == 0)
            {
                return LetterController.PropVariation.Nothing;
            }

            float normalizedDifficulty = (numRoundsWon + 1) * 0.8f * ThrowBallsConfiguration.Instance.Difficulty;

            if (normalizedDifficulty <= 0.6f)
            {
                return LetterController.PropVariation.Nothing;
            }

            else if (normalizedDifficulty <= 1.2f)
            {
                return LetterController.PropVariation.StaticPileOfCrates;
            }

            else if (normalizedDifficulty <= 1.8f)
            {
                return LetterController.PropVariation.Bush;
            }

            else if (normalizedDifficulty <= 2.4f)
            {
                return LetterController.PropVariation.StaticPileOfCrates;
            }

            else
            {
                return LetterController.PropVariation.SwervingPileOfCrates;
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