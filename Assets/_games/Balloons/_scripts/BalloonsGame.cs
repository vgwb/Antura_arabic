using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModularFramework.Core;
using ModularFramework.Helpers;
using ArabicSupport;
using EA4S;
using TMPro;

namespace EA4S.Balloons
{
    public class BalloonsGame: MiniGame
    {
        [Header("References")]
        public WordPromptController wordPrompt;
        public GameObject floatingLetterPrefab;
        public Transform[] floatingLetterLocations;
        public AnimationClip balloonPopAnimation;
        public GameObject runningAntura;
        public Canvas uiCanvas;
        public Canvas endGameCanvas;
        public TextMeshProUGUI roundNumberText;
        public TimerManager timer;
        public Animator countdownAnimator;
        public StarFlowers starFlowers;
        public GameObject FxParticlesPoof;

        [Header("Stage")]
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;

        [Header("Images")]
        public Sprite FailWrongBalloon;
        public Sprite FailTime;

        [Header("Game Parameters")] [Tooltip("e.g.: 6")]
        public static int numberOfRounds = 6;
        public int lives;
        [Range(10, 300)] [Tooltip("e.g.: 30.9")]
        public float roundTime;
        public Color[] balloonColors;

        [HideInInspector]
        public List<FloatingLetterController> floatingLetters;
        [HideInInspector]
        public float letterDropDelay;
        [HideInInspector]
        public float letterAnimationLength = 0.367f;

        public static BalloonsGame instance;

        private IQuestionPack question;
        private LL_WordData wordData;
        private string word;
        private List<LL_LetterData> wordLetters = new List<LL_LetterData>();
        private int currentRound = 0;
        private int remainingLives;

        //        private int _tutorialState;
        //
        //        private int TutorialState
        //        {
        //            get { return _tutorialState; }
        //            set {
        //                _tutorialState = value;
        //                OnTutorialStateChanged();
        //            }
        //        }

        private int _currentScore = 0;

        public int CurrentScore
        {
            get { return _currentScore; }
            set
            {
                _currentScore = value; 
                if (CurrentScore == STARS_1_THRESHOLD)
                {
                    MinigamesUI.Starbar.GotoStar(0);
                }
                else if (CurrentScore == STARS_2_THRESHOLD)
                {
                    MinigamesUI.Starbar.GotoStar(1);
                }
                else if (CurrentScore == STARS_3_THRESHOLD)
                {
                    MinigamesUI.Starbar.GotoStar(2);
                }
            }
        }

        private readonly int STARS_1_THRESHOLD = Mathf.CeilToInt(0.33f * numberOfRounds);
        private readonly int STARS_2_THRESHOLD = Mathf.CeilToInt(0.66f * numberOfRounds);
        private readonly int STARS_3_THRESHOLD = numberOfRounds;

        public int CurrentStars
        {
            get
            {
                if (CurrentScore < STARS_1_THRESHOLD)
                    return 0;
                if (CurrentScore < STARS_2_THRESHOLD)
                    return 1;
                if (CurrentScore < STARS_3_THRESHOLD)
                    return 2;
                return 3;
            }
        }

        enum Result
        {
            PERFECT,
            GOOD,
            CLEAR,
            FAIL
        }

        enum How2Die
        {
            Null,
            TimeEnd,
            WrongBalloon
        }

        How2Die howDied;

        private IPopupWidget Popup { get { return GetConfiguration().Context.GetPopupWidget(); } }

        private IAudioManager AudioManager { get { return GetConfiguration().Context.GetAudioManager(); } }

        public BalloonsIntroductionState IntroductionState { get; private set; }

        public BalloonsQuestionState QuestionState { get; private set; }

        public BalloonsPlayState PlayState { get; private set; }

        public BalloonsResultState ResultState { get; private set; }


        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new BalloonsIntroductionState(this);
            QuestionState = new BalloonsQuestionState(this);
            PlayState = new BalloonsPlayState(this);
            ResultState = new BalloonsResultState(this);
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return BalloonsConfiguration.Instance;
        }

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        protected override void Start()
        {
            base.Start();

            Random.InitState(System.DateTime.Now.GetHashCode());
            remainingLives = lives;
            letterDropDelay = balloonPopAnimation.length;

            ResetScene();
        }

        public void OnRoundStartPressed()
        {
            Popup.Hide();
            BeginGameplay();
        }

        public void OnRoundResultPressed()
        {
            AudioManager.PlaySound(Sfx.UIButtonClick);
            Popup.Hide();
            Play();
        }

        public void Play()
        {
            currentRound++;
            if (currentRound <= numberOfRounds)
            {
                StartNewRound();
            }
            else
            {
                EndGame();
            }
        }

        public void StartNewRound()
        {
            ResetScene();
            question = GetConfiguration().Questions.GetNextQuestion();
            SetNewWord();
            StartCoroutine(StartNewRound_Coroutine());
        }

        private IEnumerator StartNewRound_Coroutine()
        {
            float delay = 0.75f;
            yield return new WaitForSeconds(delay);

            AudioManager.PlayLetterData(wordData);
            //WidgetPopupWindow.I.ShowStringAndWord(OnRoundStartPressed, "#" + currentRound, wordData);
            Popup.Show();
            Popup.SetButtonCallback(OnRoundStartPressed);
            Popup.SetWord(wordData);

            uiCanvas.gameObject.SetActive(true);
        }

        private void EndRound(Result result)
        {
            AudioManager.PlayMusic(Music.Relax);
            DisableFloatingLetters();
            timer.StopTimer();
            ProcessRoundResult(result);
        }


        private void EndGame()
        {
            StartCoroutine(EndGame_Coroutine());
        }

        private IEnumerator EndGame_Coroutine()
        {
            var delay = 1f;
            yield return new WaitForSeconds(delay);

            ResetScene();
            uiCanvas.gameObject.SetActive(false);
                
            PlayState.OnResult();
        }

        private void ResetScene()
        {
            timer.ResetTimer();
            timer.DisplayTime();
            roundNumberText.text = "#" + currentRound.ToString();
            wordPrompt.Reset();
            uiCanvas.gameObject.SetActive(false);
            DestroyAllBalloons();
            howDied = How2Die.Null;
            question = null;
            word = "";
            wordData = null;
            wordLetters.Clear();
        }

        private void BeginGameplay()
        {
            timer.DisplayTime();
            CreateFloatingLetters(currentRound);
            runningAntura.SetActive(true);
            timer.StartTimer();
            AudioManager.PlayMusic(Music.MainTheme);
        }

        private void AnimateCountdown(string text)
        {
            countdownAnimator.gameObject.GetComponent<TextMeshProUGUI>().text = text;
            countdownAnimator.SetTrigger("Count");
        }

        private void SetNewWord()
        {
            //wordData = AppManager.Instance.Teacher.GimmeAGoodWordData();
            wordData = question.GetQuestion() as LL_WordData;
            word = wordData.Data.Arabic;
            //wordLetters = ArabicAlphabetHelper.LetterDataListFromWord(word, AppManager.Instance.Letters);
            wordLetters = question.GetCorrectAnswers().Cast<LL_LetterData>().ToList();
            wordPrompt.DisplayWord(wordLetters);

            Debug.Log("[New Round] Word: " + ArabicFixer.Fix(word) + ", Letters (" + wordLetters.Count + "): " + string.Join(" / ", wordLetters.Select(x => x.Data.Isolated).Reverse().ToArray()));
        }

        private void CreateFloatingLetters(int numberOfExtraLetters)
        {
            var numberOfLetters = Mathf.Clamp(wordLetters.Count + numberOfExtraLetters, 0, floatingLetterLocations.Length);
            var wrongLettersList = question.GetWrongAnswers().Cast<LL_LetterData>().ToList();
            var wrongLetters = question.GetWrongAnswers().Cast<LL_LetterData>().GetEnumerator();

            Debug.Log("Random Letters (" + wrongLettersList.Count + "): " + string.Join(" / ", wrongLettersList.Select(x => x.Data.Isolated).Reverse().ToArray()));

            // Determine indices of required letters
            List<int> requiredLetterIndices = new List<int>();
            for (int i = 0; i < wordLetters.Count; i++)
            {
                var index = Random.Range(0, numberOfLetters);

                if (!requiredLetterIndices.Contains(index))
                {
                    requiredLetterIndices.Add(index);
                }
                else
                {
                    i--;
                }
            }

            // Create floating letters
            for (int i = 0; i < numberOfLetters; i++)
            {
                var instance = Instantiate(floatingLetterPrefab);
                instance.SetActive(true);
                instance.transform.SetParent(floatingLetterLocations[i]);
                instance.transform.localPosition = Vector3.zero;

                int requiredLetterIndex = requiredLetterIndices.IndexOf(i); 
                bool isRequiredLetter = requiredLetterIndex > -1 ? true : false;

                var floatingLetter = instance.GetComponent<FloatingLetterController>();

                // Set variation, set at least 2 balloons for required letters
                if (isRequiredLetter)
                {
                    floatingLetter.SetActiveVariation(Random.Range(1, floatingLetter.variations.Length));
                }
                else
                {
                    floatingLetter.SetActiveVariation(Random.Range(0, floatingLetter.variations.Length));
                }

                var balloons = floatingLetter.Balloons;
                var letter = floatingLetter.Letter;

                // Set random balloon colors without repetition if possible
                var usedColorIndexes = new List<int>();
                for (int j = 0; j < balloons.Length; j++)
                {
                    int randomColorIndex; 

                    if (balloons.Length <= balloonColors.Length)
                    {
                        do
                        {
                            randomColorIndex = Random.Range(0, balloonColors.Length);
                        } while(usedColorIndexes.Contains(randomColorIndex));
                    }
                    else
                    {
                        randomColorIndex = Random.Range(0, balloonColors.Length);
                    }

                    usedColorIndexes.Add(randomColorIndex);
                    balloons[j].SetColor(balloonColors[randomColorIndex]);
                }

                // Set letters
                if (isRequiredLetter)
                {
                    // Set required letter
                    letter.isRequired = true;
                    letter.associatedPromptIndex = requiredLetterIndex;
                    letter.Init(wordLetters[requiredLetterIndex]);
                    Debug.Log("Create word balloon with: " + wordLetters[requiredLetterIndex].Data.Isolated);
                }
                else
                {
                    // Set a random letter that is not a required letter
                    LL_LetterData randomLetter;
                    bool invalid = false;
                    do
                    {
                        //randomLetter = AppManager.Instance.Letters.GetRandomElement();
                        randomLetter = wrongLetters.Current;
                        wrongLetters.MoveNext();
                        invalid = randomLetter == null || wordLetters.Exists(x => x.Key == randomLetter.Key);
                    } while (invalid);

                    if (invalid)
                    {
                        Debug.LogError("Error getting valid random letter for balloon!");
                    }
                    else
                    {
                        letter.Init(randomLetter);
                        Debug.Log("Create random balloon with: " + randomLetter.Data.Isolated);
                    }
                }
                    
                floatingLetters.Add(floatingLetter);
            }
        }

        public void OnDroppedLetter(BalloonsLetterController letter = null)
        {
            bool isRequired = false;
            int promptIndex = -1;
            string letterKey = "";

            if (letter != null)
            {
                isRequired = letter.isRequired;
                promptIndex = letter.associatedPromptIndex;
                if (letter.letterData != null && !string.IsNullOrEmpty(letter.letterData.Key))
                {
                    letterKey = letter.letterData.Key;
                }
            }

            if (isRequired)
            {
                OnDroppedRequiredLetter(promptIndex);
            }
            else
            {
                //
            }

            CheckRemainingBalloons();
        }

        public void OnDroppedRequiredLetter(int promptIndex)
        {
            remainingLives--;
            wordPrompt.letterPrompts[promptIndex].State = LetterPromptController.PromptState.WRONG;
            AudioManager.PlaySound(Sfx.LetterSad);

            if (remainingLives <= 0)
            {
                howDied = How2Die.WrongBalloon;
                EndRound(Result.FAIL);
            }
        }

        public void OnPoppedRequiredBalloon(int promptIndex)
        {
            AudioManager.PlaySound(Sfx.KO);
            wordPrompt.letterPrompts[promptIndex].animator.SetTrigger("Flash");
        }

        private void CheckRemainingBalloons()
        {
            int idlePromptsCount = wordPrompt.IdleLetterPrompts.Count;
            bool randomBalloonsExist = floatingLetters.Exists(balloon => balloon.Letter.isRequired == false);
            bool requiredBalloonsExist = floatingLetters.Exists(balloon => balloon.Letter.isRequired == true);

            if (!requiredBalloonsExist)
            {
                EndRound(Result.FAIL);
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
                EndRound(result);
            }
        }

        private void DisableFloatingLetters()
        {
            for (int i = 0; i < floatingLetters.Count; i++)
            {
                floatingLetters[i].Disable();
                floatingLetters[i].Letter.keepFocusingLetter = true;
            }
        }

        private void MakeWordPromptGreen()
        {
            for (int i = 0; i < wordPrompt.letterPrompts.Length; i++)
            {
                wordPrompt.letterPrompts[i].State = LetterPromptController.PromptState.CORRECT;
            }
        }

        private void DestroyAllBalloons()
        {
            for (int i = 0; i < floatingLetters.Count; i++)
            {
                Destroy(floatingLetters[i].gameObject);
            }
            floatingLetters.Clear();
        }

        private void DestroyUnrequiredBalloons()
        {
            for (int i = 0; i < floatingLetters.Count; i++)
            {
                if (!floatingLetters[i].Letter.isRequired)
                {
                    Destroy(floatingLetters[i]);
                }
            }
        }

        public void OnTimeUp()
        {
            bool randomBalloonsExist = floatingLetters.Exists(balloon => balloon.Letter.isRequired == false);
            howDied = How2Die.TimeEnd;

            if (randomBalloonsExist)
            {
                EndRound(Result.FAIL);
            }
            else
            {
                OnDroppedLetter();
            }
        }

        private void ProcessRoundResult(Result result)
        {
            bool win = false;

            switch (result)
            {
                case Result.PERFECT:
                    CurrentScore++;
                    win = true;
                    AudioManager.PlaySound(Sfx.Win);
                    break;
                case Result.GOOD:
                    CurrentScore++;
                    win = true;
                    AudioManager.PlaySound(Sfx.Win);
                    break;
                case Result.CLEAR:
                    CurrentScore++;
                    win = true;
                    AudioManager.PlaySound(Sfx.Win);
                    break;
                case Result.FAIL:
                    win = false;
                    AudioManager.PlaySound(Sfx.Lose);
                    break;
            }

            DisplayRoundResult(win);
        }

        private void DisplayRoundResult(bool win)
        {
            StartCoroutine(DisplayRoundResult_Coroutine(win));
        }

        private IEnumerator DisplayRoundResult_Coroutine(bool win)
        {
            if (win)
            {
                MakeWordPromptGreen();

                var winInitialDelay = 2f;
                yield return new WaitForSeconds(winInitialDelay);

                AudioManager.PlayDialogue(TextID.WELL_DONE);
                var winPopUpDelay = 0.25f;
                yield return new WaitForSeconds(winPopUpDelay);

                //WidgetPopupWindow.I.ShowSentenceAndWordWithMark(OnRoundResultPressed, "comment_welldone", wordData, true);
                Popup.Show();
                Popup.SetButtonCallback(OnRoundResultPressed);
                Popup.SetTitle(TextID.WELL_DONE);
                Popup.SetWord(wordData);
                Popup.SetMark(true, true);

                var winSpeakWordDelay = 0.75f;
                yield return new WaitForSeconds(winSpeakWordDelay);

                AudioManager.PlayLetterData(wordData);

            }
            else
            {
                var failDelay = 1f;
                yield return new WaitForSeconds(failDelay);

                string sentence;

                switch (howDied)
                {
                    case How2Die.TimeEnd:
                        sentence = TextID.TIMES_UP.ToString();
                        //WidgetPopupWindow.I.ShowSentenceWithMark(OnRoundResultPressed, sentence, false, FailTime);
                        Popup.ShowTimeUp(OnRoundResultPressed);
                        break;
                    case How2Die.WrongBalloon:
                        var sentenceOptions = new[]{ "game_balloons_commentA", "game_balloons_commentB" };
                        sentence = sentenceOptions[Random.Range(0, sentenceOptions.Length)];
                        //WidgetPopupWindow.I.ShowSentenceWithMark(OnRoundResultPressed, sentence, false, FailWrongBalloon);
                        Popup.Show();
                        Popup.SetButtonCallback(OnRoundResultPressed);
                        Popup.SetMark(true, false);
                        Popup.SetImage(FailWrongBalloon);
                        Popup.SetTitle(TextID.GAME_RESULT_RETRY);
                        break;
                }
            }
        }
    }
}
