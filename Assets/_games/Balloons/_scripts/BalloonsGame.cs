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
        public WordFlexibleContainer wordFlexibleContainer;
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
        public WinCelebrationController winCelebration;

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

        private IQuestionPack questionPack;
        private ILivingLetterData question;
        private IEnumerable<ILivingLetterData> correctAnswers;
        private IEnumerable<ILivingLetterData> wrongAnswers;
        //private LL_WordData wordData;
        //private string word;
        //private List<LL_LetterData> wordLetters = new List<LL_LetterData>();
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
            TimeUp,
            WrongBalloon
        }

        How2Die howDied;

        private IPopupWidget Popup { get { return GetConfiguration().Context.GetPopupWidget(); } }

        private IAudioManager AudioManager { get { return GetConfiguration().Context.GetAudioManager(); } }

        private BalloonsVariation ActiveGameVariation { get { return (GetConfiguration() as BalloonsConfiguration).Variation; } }

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

            // Get next question
            questionPack = GetConfiguration().Questions.GetNextQuestion();
            question = questionPack.GetQuestion();
            correctAnswers = questionPack.GetCorrectAnswers();
            wrongAnswers = questionPack.GetWrongAnswers();

            switch (ActiveGameVariation)
            {
                case BalloonsVariation.Spelling:
                    var spellingWordData = question as LL_WordData;
                    var spellingWord = ArabicFixer.Fix(spellingWordData.Data.Arabic);

                    // Display
                    wordPrompt.DisplayWord(correctAnswers.Cast<LL_LetterData>().ToList());

                    // Debug
                    Debug.Log("[New Round] Spelling Word: " + spellingWord);
                    Debug.Log("Letters (" + correctAnswers.Count() + "): " + string.Join(" / ", correctAnswers.Cast<LL_LetterData>().ToList().Select(x => x.TextForLivingLetter).Reverse().ToArray()));
                    Debug.Log("Random Letters (" + wrongAnswers.Count() + "): " + string.Join(" / ", wrongAnswers.Cast<LL_LetterData>().ToList().Select(x => x.TextForLivingLetter).Reverse().ToArray()));

                    // Start round
                    StartCoroutine(StartNewRound_Coroutine());
                    break;

                case BalloonsVariation.Words:
                    correctAnswers = new List<ILivingLetterData>() { question };
                    var wordToKeepData = question as LL_WordData;
                    var wordToKeep = ArabicFixer.Fix(wordToKeepData.Data.Arabic);

                    // Display
                    wordFlexibleContainer.gameObject.SetActive(true);
                    wordFlexibleContainer.SetText(wordToKeepData.TextForLivingLetter, true);

                    // Debug
                    Debug.Log("[New Round] Word To Keep: " + wordToKeep);
                    Debug.Log(" Matching word: " + ArabicFixer.Fix(correctAnswers.Cast<LL_WordData>().ToList()[0].Data.Arabic));
                    Debug.Log(" Random words (" + wrongAnswers.Count() + "): " + string.Join(" / ", wrongAnswers.Cast<LL_WordData>().ToList().Select(x => ArabicFixer.Fix(x.Data.Arabic)).ToArray()));

                    // Start round
                    StartCoroutine(StartNewRound_Coroutine());
                    break;

                case BalloonsVariation.Letter:
                    var letterToKeepData = question as LL_LetterData;
                    var letterToKeep = letterToKeepData.TextForLivingLetter;

                    // Display
                    wordFlexibleContainer.gameObject.SetActive(true);
                    wordFlexibleContainer.SetText(letterToKeep, false);

                    // Debug
                    Debug.Log("[New Round] Letter To Keep: " + letterToKeep);
                    Debug.Log(" Words with letter (" + correctAnswers.Count() + "): " + string.Join(" / ", correctAnswers.Cast<LL_WordData>().ToList().Select(x => x.TextForLivingLetter).ToArray()));
                    Debug.Log(" Random words without letter (" + wrongAnswers.Count() + "): " + string.Join(" / ", wrongAnswers.Cast<LL_WordData>().ToList().Select(x => x.TextForLivingLetter).ToArray()));

                    // Start round
                    StartCoroutine(StartNewRound_Coroutine());
                    break;

                default:
                    Debug.LogError("Invalid Balloons Game Variation!");
                    break;
            }
        }

        private IEnumerator StartNewRound_Coroutine()
        {
            float delay = 0.25f;
            yield return new WaitForSeconds(delay);

            switch (ActiveGameVariation)
            {
                case BalloonsVariation.Spelling:
                    AudioManager.PlayLetterData(question);
                    Popup.Show();
                    Popup.SetButtonCallback(OnRoundStartPressed);
                    if (question.DataType == LivingLetterDataType.Word)
                    {
                        Popup.SetLetterData(question as LL_WordData);
                    }
                    uiCanvas.gameObject.SetActive(true);
                    break;

                case BalloonsVariation.Words:
                    AudioManager.PlayLetterData(question);
                    Popup.Show();
                    Popup.SetButtonCallback(OnRoundStartPressed);
                    if (question.DataType == LivingLetterDataType.Word)
                    {
                        Popup.SetLetterData(question as LL_WordData);
                    }
                    uiCanvas.gameObject.SetActive(true);
                    break;

                case BalloonsVariation.Letter:
                    AudioManager.PlayLetterData(question);
                    Popup.Show();
                    Popup.SetButtonCallback(OnRoundStartPressed);
                    Popup.SetLetterData(question);
                    uiCanvas.gameObject.SetActive(true);
                    break;

                default:
                    Debug.LogError("Invalid Balloons Game Variation!");
                    break;
                    
            }
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
            wordFlexibleContainer.Reset();
            wordFlexibleContainer.gameObject.SetActive(false);
            uiCanvas.gameObject.SetActive(false);
            DestroyAllBalloons();
            howDied = How2Die.Null;
            questionPack = null;
            winCelebration.Hide();
        }

        private void BeginGameplay()
        {
            switch (ActiveGameVariation)
            {
                case BalloonsVariation.Spelling:
                    timer.DisplayTime();
                    CreateFloatingLetters_Spelling(currentRound);
                    runningAntura.SetActive(true);
                    timer.StartTimer();
                    AudioManager.PlayMusic(Music.MainTheme);
                    break;
                
                case BalloonsVariation.Words:
                    timer.DisplayTime();
                    CreateFloatingLetters_Words(currentRound);
                    runningAntura.SetActive(true);
                    timer.StartTimer();
                    AudioManager.PlayMusic(Music.MainTheme);
                    break;

                case BalloonsVariation.Letter:
                    timer.DisplayTime();
                    CreateFloatingLetters_Letter(currentRound);
                    runningAntura.SetActive(true);
                    timer.StartTimer();
                    AudioManager.PlayMusic(Music.MainTheme);
                    break;

                default:
                    Debug.LogError("Invalid Balloons Game Variation!");
                    break;
            }
        }

        private void AnimateCountdown(string text)
        {
            countdownAnimator.gameObject.GetComponent<TextMeshProUGUI>().text = text;
            countdownAnimator.SetTrigger("Count");
        }

        private void CreateFloatingLetters_Spelling(int numberOfExtraLetters)
        {
            var wordData = question as LL_WordData;
            var wordLetters = correctAnswers.Cast<LL_LetterData>().ToList();
            var randomLetters = wrongAnswers.Cast<LL_LetterData>().GetEnumerator();

            var numberOfLetters = Mathf.Clamp(wordLetters.Count + numberOfExtraLetters, 0, floatingLetterLocations.Length);

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
                    Debug.Log("Create word balloon with: " + wordLetters[requiredLetterIndex].TextForLivingLetter);
                }
                else
                {
                    // Set a random letter that is not a required letter
                    LL_LetterData randomLetter;
                    bool invalid = false;
                    do
                    {
                        randomLetter = randomLetters.Current;
                        invalid = randomLetter == null || wordLetters.Exists(x => x.Id == randomLetter.Id);
                    } while (randomLetters.MoveNext() && invalid);

                    if (invalid)
                    {
                        Debug.LogError("Error getting valid random letter for balloon!");
                    }
                    else
                    {
                        letter.Init(randomLetter);
                        Debug.Log("Create random balloon with: " + randomLetter.TextForLivingLetter);
                    }
                }
                    
                floatingLetters.Add(floatingLetter);
            }
        }

        private void CreateFloatingLetters_Words(int numberOfExtraWords)
        {
            var numberOfWords = Mathf.Clamp(correctAnswers.Count() + numberOfExtraWords, 0, floatingLetterLocations.Length);
            var correctWord = correctAnswers.Cast<LL_WordData>().ToList()[0];
            var wrongWords = wrongAnswers.Cast<LL_WordData>().GetEnumerator();

            // Determine index of required word
            int requiredWordIndex = Random.Range(0, numberOfWords);

            // Create floating letters
            for (int i = 0; i < numberOfWords; i++)
            {
                var instance = Instantiate(floatingLetterPrefab);
                instance.SetActive(true);
                instance.transform.SetParent(floatingLetterLocations[i]);
                instance.transform.localPosition = Vector3.zero;

                bool isRequiredWord = (i == requiredWordIndex ? true : false);

                var floatingLetter = instance.GetComponent<FloatingLetterController>();

                // Set variation, set at least 2 balloons for required word
                if (isRequiredWord)
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

                // Set words
                if (isRequiredWord)
                {
                    // Set correct word
                    LL_WordData word = correctWord;
                    bool invalid = (word == null);

                    if (invalid)
                    {
                        Debug.LogError("Error getting valid word (correct answer) for balloon!");
                    }
                    letter.isRequired = true;
                    letter.associatedPromptIndex = -1;
                    letter.Init(word);
                    Debug.Log("Create word balloon with: " + letter.LLPrefab.Data.TextForLivingLetter);
                }
                else
                {
                    // Set a random letter that is not a required letter
                    LL_WordData randomWord;
                    bool invalid = false;
                    do
                    {
                        randomWord = wrongWords.Current;
                        invalid = (randomWord == null);
                    } while (wrongWords.MoveNext() && invalid);

                    if (invalid)
                    {
                        Debug.LogError("Error getting valid random word (wrong answer) for balloon!");
                    }
                    letter.Init(randomWord);
                    Debug.Log("Create random balloon with: " + letter.LLPrefab.Data.TextForLivingLetter);
                }

                floatingLetters.Add(floatingLetter);
            }
        }

        private void CreateFloatingLetters_Letter(int numberOfExtraWords)
        {
            var numberOfWords = Mathf.Clamp(correctAnswers.Count() + numberOfExtraWords, 0, floatingLetterLocations.Length);
            var correctWords = correctAnswers.Cast<LL_WordData>().GetEnumerator();
            var wrongWords = wrongAnswers.Cast<LL_WordData>().GetEnumerator();

            // Determine indices of required words
            var requiredWordIndices = new List<int>();
            for (int i = 0; i < correctAnswers.Count(); i++)
            {
                var index = Random.Range(0, numberOfWords);

                if (!requiredWordIndices.Contains(index))
                {
                    requiredWordIndices.Add(index);
                }
                else
                {
                    i--;
                }
            }

            // Create floating letters
            for (int i = 0; i < numberOfWords; i++)
            {
                var instance = Instantiate(floatingLetterPrefab);
                instance.SetActive(true);
                instance.transform.SetParent(floatingLetterLocations[i]);
                instance.transform.localPosition = Vector3.zero;

                int requiredWordIndex = requiredWordIndices.IndexOf(i); 
                bool isRequiredWord = requiredWordIndex > -1 ? true : false;

                var floatingLetter = instance.GetComponent<FloatingLetterController>();

                // Set variation, set at least 2 balloons for required words
                if (isRequiredWord)
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

                // Set words
                if (isRequiredWord)
                {
                    // Set correct word
                    LL_WordData word;
                    bool invalid = false;
                    do
                    {
                        word = correctWords.Current;
                        invalid = (word == null);
                    } while (correctWords.MoveNext() && invalid);

                    if (invalid)
                    {
                        Debug.LogError("Error getting valid word (correct answer) for balloon!");
                    }
                    letter.isRequired = true;
                    letter.associatedPromptIndex = -1;
                    letter.Init(word);
                    Debug.Log("Create word balloon with: " + letter.LLPrefab.Data.TextForLivingLetter);
                }
                else
                {
                    // Set a random letter that is not a required letter
                    LL_WordData randomWord;
                    bool invalid = false;
                    do
                    {
                        randomWord = wrongWords.Current;
                        invalid = (randomWord == null);
                    } while (wrongWords.MoveNext() && invalid);

                    if (invalid)
                    {
                        Debug.LogError("Error getting valid random word (wrong answer) for balloon!");
                    }
                    letter.Init(randomWord);
                    Debug.Log("Create random balloon with: " + letter.LLPrefab.Data.TextForLivingLetter);
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
                if (letter.letterData != null && !string.IsNullOrEmpty(letter.letterData.Id))
                {
                    letterKey = letter.letterData.Id;
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
            AudioManager.PlaySound(Sfx.LetterSad);
            if (promptIndex > -1)
            {
                wordPrompt.letterPrompts[promptIndex].State = LetterPromptController.PromptState.WRONG;
            }
                
            if (remainingLives <= 0)
            {
                howDied = How2Die.WrongBalloon;
                EndRound(Result.FAIL);
            }
        }

        public void OnPoppedRequiredBalloon(int promptIndex)
        {
            AudioManager.PlaySound(Sfx.KO);
            if (promptIndex > -1)
            {
                wordPrompt.letterPrompts[promptIndex].animator.SetTrigger("Flash");
            }
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
                if (idlePromptsCount == wordPrompt.activePromptsCount)
                {
                    result = Result.PERFECT;
                }
                else if (idlePromptsCount > 1)
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

        private void DestroyRandomBalloons()
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
            howDied = How2Die.TimeUp;

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
                case Result.GOOD:
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

                var winInitialDelay = 1f;
                yield return new WaitForSeconds(winInitialDelay);

                //AudioManager.PlayDialogue(TextID.WELL_DONE);
                //var winPopUpDelay = 0.25f;
                //yield return new WaitForSeconds(winPopUpDelay);
                //Popup.Show();
                //Popup.SetButtonCallback(OnRoundResultPressed);
                //Popup.SetTitle(TextID.WELL_DONE);
                //Popup.SetMark(true, true);
                //if (question.DataType == LivingLetterDataType.Word)
                //{
                //    Popup.SetWord(question as LL_WordData);
                //}
                winCelebration.Show(question);
                //var winSpeakWordDelay = 0.75f;
                //yield return new WaitForSeconds(winSpeakWordDelay);
                AudioManager.PlayLetterData(question);

                var resumePlayingDelay = 1.5f;
                yield return new WaitForSeconds(resumePlayingDelay);
                Play();
            }
            else
            {
                var failDelay = 1f;
                yield return new WaitForSeconds(failDelay);

                //string sentence;

                switch (howDied)
                {
                    case How2Die.TimeUp:
                        //sentence = TextID.TIMES_UP.ToString();
                        //WidgetPopupWindow.I.ShowSentenceWithMark(OnRoundResultPressed, sentence, false, FailTime);
                        Popup.ShowTimeUp(OnRoundResultPressed);
                        break;
                    case How2Die.WrongBalloon:
                        //var sentenceOptions = new[]{ "game_balloons_commentA", "game_balloons_commentB" };
                        //sentence = sentenceOptions[Random.Range(0, sentenceOptions.Length)];
                        //WidgetPopupWindow.I.ShowSentenceWithMark(OnRoundResultPressed, sentence, false, FailWrongBalloon);
                        Popup.Show();
                        Popup.SetButtonCallback(OnRoundResultPressed);
                        Popup.SetMark(true, false);
                        Popup.SetImage(FailWrongBalloon);
                        //Popup.SetTitle(TextID.GAME_RESULT_RETRY);
                        break;
                }
            }
        }
    }
}
