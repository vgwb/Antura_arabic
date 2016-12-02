using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EA4S;
using System;

namespace EA4S.ThrowBalls
{
    public class GameState : IGameState
    {
        public const int MAX_NUM_ROUNDS = 5;
        public const int NUM_LETTERS_IN_POOL = 7;
        public static int MAX_NUM_BALLS = 5;

        public const float TUTORIAL_UI_PERIOD = 4;

        public bool isRoundOngoing;

        // Round number is 1-based. (Round 1, round 2,...)
        // Round 0 is the tutorial round.
        private int roundNumber = 0;
        private int numBalls = MAX_NUM_BALLS;

        private int numRoundsWon = 0;

        private float timeLeftToShowTutorialUI = TUTORIAL_UI_PERIOD;
        private bool isIdle = true;

        private ILivingLetterData question;
        private int numLettersRemaining;
        private int numLetters = 3;

        private LetterSpawner letterSpawner;
        public GameObject[] letterPool;
        private LetterController[] letterControllers;

        private ThrowBallsGame game;

        public static GameState instance;

        private bool isVoiceOverDone = false;
        private IAudioManager audioManager;
        private IInputManager inputManager;

        public GameState(ThrowBallsGame game)
        {
            this.game = game;

            if (ThrowBallsConfiguration.Instance.Variation == ThrowBallsVariation.lettersinword)
            {
                MAX_NUM_BALLS = 10;
            }

            instance = this;

            inputManager = ThrowBallsConfiguration.Instance.Context.GetInputManager();
            audioManager = game.Context.GetAudioManager();

            inputManager.Enabled = false;
        }
        public void EnterState()
        {
            UnityEngine.Random.InitState(DateTime.Now.GetHashCode());

            // Layer 8 = Terrain. Layer 12 = Ball.
            Physics.IgnoreLayerCollision(8, 10);

            letterSpawner = new LetterSpawner();

            foreach (Collider collider in ThrowBallsGame.instance.environment.GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }

            letterPool = new GameObject[NUM_LETTERS_IN_POOL];
            letterControllers = new LetterController[NUM_LETTERS_IN_POOL];

            for (int i = 0; i < letterPool.Length; i++)
            {
                GameObject letter = ThrowBallsGame.Instantiate(game.letterWithPropsPrefab).GetComponent<LetterWithPropsController>().letter;
                LetterController letterController = letter.GetComponent<LetterController>();

                letterPool[i] = letter;
                letterControllers[i] = letterController;

                letter.SetActive(false);
            }

            ThrowBallsGame.instance.letterWithPropsPrefab.SetActive(false);

            //ResetScene();
            
            switch (ThrowBallsConfiguration.Instance.Variation)
            {
                case ThrowBallsVariation.letters:
                    audioManager.PlayDialogue(Db.LocalizationDataId.ThrowBalls_letters_Title, OnTitleVoiceOverDone);
                    break;
                case ThrowBallsVariation.words:
                    audioManager.PlayDialogue(Db.LocalizationDataId.ThrowBalls_words_Title, OnTitleVoiceOverDone);
                    break;
                case ThrowBallsVariation.lettersinword:
                    audioManager.PlayDialogue(Db.LocalizationDataId.ThrowBalls_letterinword_Title, OnTitleVoiceOverDone);
                    break;
                default:
                    break;
            }

            AudioManager.I.PlayMusic(Music.Theme10);
        }

        private void OnTitleVoiceOverDone()
        {
            audioManager.PlayDialogue(Db.LocalizationDataId.ThrowBalls_letters_Intro, OnIntroVoiceOverDone);
        }

        private void OnIntroVoiceOverDone()
        {
            AnturaController.instance.DoneChasing();
            AnturaController.instance.Disable();

            switch (ThrowBallsConfiguration.Instance.Variation)
            {
                case ThrowBallsVariation.letters:
                    game.StartCoroutine(StartNewRound());
                    break;
                case ThrowBallsVariation.words:
                    game.StartCoroutine(StartNewRound());
                    break;
                case ThrowBallsVariation.lettersinword:
                    game.StartCoroutine(StartNewRound_LettersInWord());
                    break;
                default:
                    break;
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

            question = newQuestionPack.GetQuestion();
            ILivingLetterData correctDatum = newQuestionPack.GetCorrectAnswers().ToList()[0];
            List<ILivingLetterData> wrongData = newQuestionPack.GetWrongAnswers().ToList();

            if (ThrowBallsConfiguration.Instance.Variation == ThrowBallsVariation.words)
            {
                correctDatum = new LL_ImageData(correctDatum.Id);

                for (int i = 0; i < wrongData.Count; i++)
                {
                    wrongData[i] = new LL_ImageData(wrongData[i].Id);
                }
            }

            SayQuestion();

            yield return new WaitForSeconds(1f);

            for (int i = 0; i < numLetters; i++)
            {
                GameObject letterObj = letterPool[i];

                letterObj.SetActive(true);

                letterControllers[i].SetMotionVariation(GetMotionOfRound());

                letterControllers[i].SetPropVariation(GetPropOfRound());

                if (i == 0)
                {
                    letterObj.tag = Constants.TAG_CORRECT_LETTER;
                    letterControllers[i].SetLetter(correctDatum);
                }

                else
                {
                    letterObj.tag = Constants.TAG_WRONG_LETTER;

                    letterControllers[i].SetLetter(wrongData[0]);

                    wrongData.RemoveAt(0);
                }
            }

            isRoundOngoing = true;

            BallController.instance.Enable();

            UIController.instance.Enable();
            UIController.instance.SetLetterHint(question);

            if (IsTutorialLevel())
            {
                switch (ThrowBallsConfiguration.Instance.Variation)
                {
                    case ThrowBallsVariation.letters:
                        audioManager.PlayDialogue(Db.LocalizationDataId.ThrowBalls_letters_Tuto);
                        break;
                    case ThrowBallsVariation.words:
                        audioManager.PlayDialogue(Db.LocalizationDataId.ThrowBalls_words_Tuto);
                        break;
                    case ThrowBallsVariation.lettersinword:
                        audioManager.PlayDialogue(Db.LocalizationDataId.ThrowBalls_letterinword_Tuto);
                        break;
                    default:
                        break;
                }

                inputManager.Enabled = true;
                isVoiceOverDone = true;
                ShowTutorialUI();
            }
        }

        public IEnumerator StartNewRound_LettersInWord()
        {
            IQuestionPack newQuestionPack = ThrowBallsConfiguration.Instance.Questions.GetNextQuestion();

            List<ILivingLetterData> letterData = newQuestionPack.GetCorrectAnswers().ToList();

            numLettersRemaining = letterData.Count;
            numLetters = numLettersRemaining;

            ResetScene();

            if (roundNumber == 1)
            {
                MinigamesUI.Init(MinigamesUIElement.Lives | MinigamesUIElement.Starbar);
                MinigamesUI.Lives.Setup(MAX_NUM_BALLS);
            }

            UIController.instance.Enable();
            
            ILivingLetterData firstLetter = letterData[0];
            letterData.RemoveAt(0);
            List<ILivingLetterData> remainingLetters = letterData;

            question = newQuestionPack.GetQuestion();
            UIController.instance.SetLetterHint(question);

            SayQuestion();

            yield return new WaitForSeconds(1f);

            int numLettersInRound = remainingLetters.Count + 1;

            for (int i = 0; i < numLettersInRound; i++)
            {
                GameObject letterObj = letterPool[i];

                letterObj.SetActive(true);

                letterControllers[i].SetMotionVariation(GetMotionOfRound());

                letterControllers[i].SetPropVariation(GetPropOfRound());

                if (i == 0)
                {
                    letterObj.tag = Constants.TAG_CORRECT_LETTER;
                    letterControllers[i].SetLetter(firstLetter);
                }

                else
                {
                    if (remainingLetters[0].Id == firstLetter.Id)
                    {
                        letterObj.tag = Constants.TAG_CORRECT_LETTER;
                    }

                    else
                    {
                        letterObj.tag = Constants.TAG_WRONG_LETTER;
                    }

                    letterControllers[i].SetLetter(remainingLetters[0]);

                    remainingLetters.RemoveAt(0);
                }
            }

            isRoundOngoing = true;

            BallController.instance.Enable();

            if (IsTutorialLevel())
            {
                switch (ThrowBallsConfiguration.Instance.Variation)
                {
                    case ThrowBallsVariation.letters:
                        audioManager.PlayDialogue(Db.LocalizationDataId.ThrowBalls_letters_Tuto);
                        break;
                    case ThrowBallsVariation.words:
                        audioManager.PlayDialogue(Db.LocalizationDataId.ThrowBalls_words_Tuto);
                        break;
                    case ThrowBallsVariation.lettersinword:
                        audioManager.PlayDialogue(Db.LocalizationDataId.ThrowBalls_letterinword_Tuto);
                        break;
                    default:
                        break;
                }

                inputManager.Enabled = true;
                isVoiceOverDone = true;
                ShowTutorialUI();
            }
        }

        private void SayQuestion()
        {
            if (ThrowBallsConfiguration.Instance.Variation == ThrowBallsVariation.letters)
            {
                AudioManager.I.PlayLetter(question.Id);
            }

            else
            {
                AudioManager.I.PlayWord(question.Id);
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

        private void UpdateLettersForLettersInWord(LetterController correctLetterCntrl)
        {
            correctLetterCntrl.Vanish();
            correctLetterCntrl.Reset();

            ILivingLetterData newCorrectLetter = letterControllers[numLetters - numLettersRemaining].GetLetter();

            for (int i = numLetters - numLettersRemaining; i < numLetters; i++)
            {
                if (letterControllers[i].GetLetter().Id == newCorrectLetter.Id)
                {
                    letterPool[i].tag = Constants.TAG_CORRECT_LETTER;
                }

                else
                {
                    letterPool[i].tag = Constants.TAG_WRONG_LETTER;
                }
            }
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
                if (ThrowBallsConfiguration.Instance.Variation == ThrowBallsVariation.lettersinword)
                {
                    game.StartCoroutine(StartNewRound_LettersInWord());
                }

                else
                {
                    game.StartCoroutine(StartNewRound());
                }
            }
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





        public void OnCorrectLetterHit(LetterController correctLetterCntrl)
        {
            if (ThrowBallsConfiguration.Instance.Variation == ThrowBallsVariation.lettersinword && --numLettersRemaining != 0)
            {
                UpdateLettersForLettersInWord(correctLetterCntrl);
                OnBallLost();
                BallController.instance.Reset();
            }

            else if (isRoundOngoing)
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

                game.StartCoroutine(ShowWinSequence(correctLetterCntrl));
                BallController.instance.Disable();

                isRoundOngoing = false;
            }
        }

        public void OnRoundLost()
        {
            if (isRoundOngoing)
            {
                BallController.instance.Disable();
                isRoundOngoing = false;
                DisableLetters(true);

                game.StartCoroutine(OnRoundLostCoroutine());
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

            SayQuestion();

            correctLetterCntrl.SetMotionVariation(LetterController.MotionVariation.Idle);
            correctLetterCntrl.SetPropVariation(LetterController.PropVariation.Nothing);
            correctLetterCntrl.MoveTo(0, 13.5f, -33f);
            correctLetterCntrl.transform.rotation = Quaternion.Euler(-Camera.main.transform.rotation.eulerAngles.x, 180, 0);

            if (ThrowBallsConfiguration.Instance.Variation == ThrowBallsVariation.lettersinword)
            {
                correctLetterCntrl.SetLetter(question);
            }

            correctLetterCntrl.Show();
            correctLetterCntrl.letterObjectView.DoHorray();
            correctLetterCntrl.ShowVictoryRays();

            AudioManager.I.PlaySfx(Sfx.Win);

            yield return new WaitForSeconds(3f);

            OnRoundConcluded();
        }

        private Vector3 GetCratePosition(Vector3 relativeToLetterPosition)
        {
            return new Vector3(relativeToLetterPosition.x, relativeToLetterPosition.y - 2.1f, relativeToLetterPosition.z);
        }

        private void EndGame()
        {
            game.StartCoroutine(EndGame_Coroutine());
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

            game.EndGame(numberOfStars, 0);
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

        public void ResetScene()
        {
            UIController.instance.Reset();
            UIController.instance.Disable();

            foreach (LetterController letterController in letterControllers)
            {
                letterController.Reset();
                letterController.DisableProps();
            }

            for (int i = 0; i < letterPool.Length; i++)
            {
                GameObject letter = letterPool[i];
                letter.SetActive(false);
            }

            Vector3[] randomPositions = letterSpawner.GenerateRandomPositions(numLetters, roundNumber == 0);

            for (int i = 0; i < numLetters; i++)
            {
                GameObject letter = letterPool[i];
                letter.transform.position = randomPositions[i];
            }

            BallController.instance.Reset();

            numBalls = MAX_NUM_BALLS;

            if (roundNumber > 1)
            {
                MinigamesUI.Lives.ResetToMax();
            }

            isRoundOngoing = false;
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
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

        public void UpdatePhysics(float delta)
        {
            if (isVoiceOverDone && roundNumber == 0 && isIdle && !BallController.instance.IsLaunched())
            {
                timeLeftToShowTutorialUI -= Time.fixedDeltaTime;

                if (timeLeftToShowTutorialUI <= 0)
                {
                    ShowTutorialUI();
                }
            }
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
    }
}