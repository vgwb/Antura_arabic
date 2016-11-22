using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using TMPro;

using System;
using EA4S;

namespace EA4S.DancingDots
{
    public enum Diacritic { Sokoun, Fatha, Dameh, Kasrah };

    public class DancingDotsGame : MiniGame
    {

        public IntroductionGameState IntroductionState { get; private set; }
        public QuestionGameState QuestionState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        public ResultGameState ResultState { get; private set; }

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new IntroductionGameState(this);
            QuestionState = new QuestionGameState(this);
            PlayState = new PlayGameState(this);
            ResultState = new ResultGameState(this);
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return DancingDotsConfiguration.Instance;
        }

        public static DancingDotsGame instance;
        public Canvas endGameCanvas;
        public StarFlowers starFlowers;

        public const string DANCING_DOTS = "DancingDots_DotZone";
        public const string DANCING_DIACRITICS = "DancingDots_Diacritic";

        public float gameDuration = 60f;
        public DancingDotsLivingLetter dancingDotsLL;
        public GameObject antura;
        public float anturaMinDelay = 3f;
        public float anturaMaxDelay = 10f;
        public float anturaMinScreenTime = 1f;
        public float anturaMaxScreenTime = 2f;
        public GameObject[] diacritics;
        public DancingDotsDiacriticPosition activeDiacritic;

        [HideInInspector]
        public DancingDotsQuestionsManager questionsManager;


        DancingDotsTutorial tutorial;

        public bool isTutRound
        {
            get
            {
                if (numberOfRoundsPlayed == 0)
                    return true;
                else
                    return false;
            }
        }

        public string currentLetter = "";
        private int _dotsCount;
        public int dotsCount
        {
            get
            {
                return _dotsCount;
            }
            set
            {
                _dotsCount = value;
                foreach (DancingDotsDraggableDot dd in dragableDots)
                {
                    dd.isNeeded = dd.dots == _dotsCount;
                }
            }
        }

        public GameObject splatPrefab;
        public Transform splatParent;

        [Range(0, 255)]
        public byte dotHintAlpha = 60;
        [Range(0, 255)]
        public byte diacriticHintAlpha = 60;

        public float hintDotDuration = 2.5f;
        public float hintDiacriticDuration = 3f;

        public GameObject poofPrefab;

        [Range(0, 1)]
        public float pedagogicalLevel = 0;

        public int numberOfRounds = 6;

        public int allowedFailedMoves = 3;

        public DancingDotsDraggableDot[] dragableDots;
        public DancingDotsDraggableDot[] dragableDiacritics;
        public DancingDotsDropZone[] dropZones;

        private bool isCorrectDot = false;
        private bool isCorrectDiacritic = false;

        public int currStarsNum = 0;
        public int numberOfRoundsWon = 0;
        private int numberOfRoundsPlayed = -1;
        private int numberOfFailedMoves = 0;

        enum Level { Level1, Level2, Level3, Level4, Level5, Level6 };

        private Level currentLevel = Level.Level4;
        private List<DancingDotsSplat> splats;
        private bool isPlaying = false;


        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        protected override void Start()
        {

            base.Start();
            tutorial = GetComponent<DancingDotsTutorial>();

            AppManager.Instance.InitDataAI();
            AppManager.Instance.CurrentGameManagerGO = gameObject;
            SceneTransitioner.Close();

            Debug.Log("Before Music");
            AudioManager.I.PlayMusic(Music.MainTheme);
            //			DancingDotsConfiguration.Instance.Context.GetAudioManager().PlayMusic(Music.MainTheme);
            Debug.Log("After Music");

            questionsManager = new DancingDotsQuestionsManager();

            splats = new List<DancingDotsSplat>();

            //StartRound();

            isPlaying = true;

            //			StartCoroutine(AnimateAntura());

        }

        public Color32 SetAlpha(Color32 color, byte alpha)
        {
            if (alpha >= 0 && alpha <= 255)
            {
                return new Color32(color.r, color.g, color.b, alpha);
            }
            else
            {
                return color;
            }
        }

        IEnumerator AnimateAntura()
        {
            Vector3 pos = antura.transform.position;
            // Move antura off screen because SetActive is reseting the animation to running
            antura.transform.position = new Vector3(-50, pos.y, pos.z);
            do
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(anturaMinDelay, anturaMaxDelay));
                CreatePoof(pos, 2f, false);
                yield return new WaitForSeconds(0.4f);
                antura.transform.position = pos;
                antura.GetComponent<AnturaAnimationController>().DoSniff();
                yield return new WaitForSeconds(UnityEngine.Random.Range(anturaMinScreenTime, anturaMaxScreenTime));
                CreatePoof(pos, 2f, false);
                antura.transform.position = new Vector3(-50, pos.y, pos.z);
            } while (isPlaying);

        }


        private void SetLevel(Level level)
        {
            foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.gameObject.SetActive(false);
            foreach (DancingDotsDraggableDot dDiacritic in dragableDiacritics) dDiacritic.gameObject.SetActive(false);
            foreach (GameObject go in diacritics) go.SetActive(false);

            switch (level)
            {
                case Level.Level1: // Dots alone with visual aid
                                   //				allowedFailedMoves = 2;
                    isCorrectDot = false;
                    isCorrectDiacritic = true;
                    foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.Reset();
                    StartCoroutine(RemoveHintDot());
                    break;

                case Level.Level2: // Diacritics alone with visual aid
                    isCorrectDot = true;
                    isCorrectDiacritic = false;
                    foreach (DancingDotsDraggableDot dDots in dragableDiacritics) dDots.Reset();
                    dancingDotsLL.fullTextGO.SetActive(true); // Show dot
                    StartCoroutine(RandomDiacritic());
                    StartCoroutine(RemoveHintDiacritic());
                    break;

                case Level.Level3: // Dots and diacritics with visual aid
                    isCorrectDot = false;
                    isCorrectDiacritic = false;
                    foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.Reset();
                    foreach (DancingDotsDraggableDot dDiacritic in dragableDiacritics) dDiacritic.Reset();
                    StartCoroutine(RandomDiacritic());
                    StartCoroutine(RemoveHintDot());
                    StartCoroutine(RemoveHintDiacritic());
                    break;

                case Level.Level4: // Dots alone without visual aid
                                   //				allowedFailedMoves = 2;
                    isCorrectDot = false;
                    isCorrectDiacritic = true;
                    foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.Reset();
                    dancingDotsLL.HideText(dancingDotsLL.hintText);
                    break;

                case Level.Level5: // Diacritics alone without visual aid
                    isCorrectDot = true;
                    isCorrectDiacritic = false;
                    foreach (DancingDotsDraggableDot dDots in dragableDiacritics) dDots.Reset();
                    dancingDotsLL.fullTextGO.SetActive(true); // Show dot
                    StartCoroutine(RandomDiacritic());
                    // Level checked in RandomDiacritic instead of activeDiacritic.Hide(); due to frame delay
                    break;

                case Level.Level6: // Dots and diacritics without visual aid
                    isCorrectDot = false;
                    isCorrectDiacritic = false;
                    foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.Reset();
                    foreach (DancingDotsDraggableDot dDiacritic in dragableDiacritics) dDiacritic.Reset();
                    StartCoroutine(RandomDiacritic());
                    dancingDotsLL.HideText(dancingDotsLL.hintText);
                    // Level checked in RandomDiacritic instead of activeDiacritic.Hide(); due to frame delay
                    break;

                default:
                    SetLevel(Level.Level1);
                    break;

            }
        }

        public void StartRound()
        {

            numberOfRoundsPlayed++;

            dancingDotsLL.letterObjectView.SetDancingSpeed(1f);

            splats.Clear();
            dancingDotsLL.HideRainbow();

            Debug.Log("[Dancing Dots] Round: " + numberOfRoundsPlayed);
            numberOfFailedMoves = 0;
            dancingDotsLL.Reset();

            startUI();

            if (pedagogicalLevel == 0f) // TODO for testing only each round increment Level. Remove later!
            {
                switch (numberOfRoundsPlayed)
                {
                    case 1:
                    case 2:
                        currentLevel = Level.Level1;
                        break;
                    case 3:
                        currentLevel = Level.Level4;
                        break;
                    case 4:
                        currentLevel = Level.Level2;
                        break;
                    case 5:
                    case 6:
                        currentLevel = Level.Level3;
                        break;
                    default:
                        currentLevel = Level.Level3;
                        break;
                }
            }
            else
            {
                // TODO Move later to Start method
                var numberOfLevels = Enum.GetNames(typeof(Level)).Length;
                currentLevel = (Level)Mathf.Clamp((int)Mathf.Floor(pedagogicalLevel * numberOfLevels), 0, numberOfLevels - 1);
            }

            Debug.Log("[Dancing Dots] pedagogicalLevel: " + pedagogicalLevel + " Game Level: " + currentLevel);

            SetLevel(currentLevel);

            tutorial.doTutorial();

        }




        private void CreatePoof(Vector3 position, float duration, bool withSound)
        {
            if (withSound) AudioManager.I.PlaySfx(Sfx.BaloonPop);
            GameObject poof = Instantiate(poofPrefab, position, Quaternion.identity) as GameObject;
            Destroy(poof, duration);
        }


        IEnumerator RemoveHintDot()
        {
            yield return new WaitForSeconds(hintDotDuration);
            if (!isCorrectDot)
            {
                // find dot postion
                Vector3 poofPosition = Vector3.zero;
                foreach (DancingDotsDropZone dz in dropZones)
                {
                    if (dz.letters.Contains(currentLetter))
                    {
                        poofPosition = new Vector3(dz.transform.position.x, dz.transform.position.y, -8);
                        break;
                    }
                }
                CreatePoof(poofPosition, 2f, true);
                dancingDotsLL.HideText(dancingDotsLL.hintText);
            }
        }

        IEnumerator RemoveHintDiacritic()
        {
            yield return new WaitForSeconds(hintDiacriticDuration);
            if (!isCorrectDiacritic)
            {
                CreatePoof(activeDiacritic.transform.position, 2f, true);
                activeDiacritic.Hide();
            }
        }

        private IEnumerator RandomDiacritic()
        {


            foreach (GameObject go in diacritics)
            {
                go.SetActive(false);
                go.GetComponent<DancingDotsDiacriticPosition>().Hide();
            }

            int random = UnityEngine.Random.Range(0, diacritics.Length);

            activeDiacritic = diacritics[random].GetComponent<DancingDotsDiacriticPosition>();
            activeDiacritic.gameObject.SetActive(true);

            foreach (DancingDotsDraggableDot dd in dragableDiacritics)
            {
                dd.isNeeded = activeDiacritic.diacritic == dd.diacritic;
            }

            // wait for end of frame to ge correct values for meshes
            yield return new WaitForEndOfFrame();
            activeDiacritic.CheckPosition();

            // Level checked in RandomDiacritic instead of SetLevel due to frame delay
            if (currentLevel != Level.Level5 && currentLevel != Level.Level6)
            {
                activeDiacritic.Show();
            }

        }



        /*protected override void ReadyForGameplay()
        {
            base.ReadyForGameplay();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnMinigameQuit()
        {
            base.OnMinigameQuit();
        }
        */
        IEnumerator CorrectMove(bool roundWon)
        {
            AudioManager.I.PlayDialog("comment_welldone");
            dancingDotsLL.ShowRainbow();
            dancingDotsLL.letterObjectView.SetDancingSpeed(1f);
            

            if (roundWon)
            {
                StartCoroutine(RoundWon());
            }
            else
            {
                dancingDotsLL.letterObjectView.DoHorray(); // ("Jump");
                yield return new WaitForSeconds(1f);
                dancingDotsLL.HideRainbow();
                //                yield return new WaitForSeconds(1f);
                tutorial.doTutorial();
                //startUI();

            }
        }

        IEnumerator PoofOthers(DancingDotsDraggableDot[] draggables)
        {
            foreach (DancingDotsDraggableDot dd in draggables)
            {
                if (dd.gameObject.activeSelf)
                {
                    yield return new WaitForSeconds(0.25f);
                    dd.gameObject.SetActive(false);
                    CreatePoof(dd.transform.position, 2f, true);
                }

            }
        }

        public void CorrectDot()
        {
            // Change font or show correct character
            isCorrectDot = true;
            dancingDotsLL.fullTextGO.SetActive(true);
            StartCoroutine(PoofOthers(dragableDots));
            StartCoroutine(CorrectMove(isCorrectDiacritic));
        }

        public void CorrectDiacritic()
        {
            isCorrectDiacritic = true;
            activeDiacritic.GetComponent<TextMeshPro>().color = new Color32(0, 0, 0, 255);
            StartCoroutine(PoofOthers(dragableDiacritics));
            StartCoroutine(CorrectMove(isCorrectDot));
        }

        public void WrongMove(Vector3 pos)
        {


            numberOfFailedMoves++;
            dancingDotsLL.letterObjectView.SetDancingSpeed(1f - numberOfFailedMoves * 0.25f);
            GameObject splat = (GameObject)Instantiate(splatPrefab);
            splat.transform.parent = splatParent;
            splat.transform.localScale = new Vector3(1f, 1f, 1f);
            splat.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            splat.transform.position = pos;
            splat.transform.localPosition = new Vector3(splat.transform.localPosition.x, splat.transform.localPosition.y, 0f);

            splats.Add(splat.GetComponent<DancingDotsSplat>());

            if (numberOfFailedMoves >= allowedFailedMoves)
            {
                StartCoroutine(RoundLost());
            }

        }

        IEnumerator CheckNewRound()
        {
            dancingDotsLL.letterObjectView.SetDancingSpeed(1f);

            if (numberOfRoundsPlayed >= numberOfRounds)
            {
                EndGame();
            }
            else
            {
                
                dancingDotsLL.letterObjectView.DoTwirl(null);
                foreach (DancingDotsSplat splat in splats) splat.CleanSplat();
                yield return new WaitForSeconds(1f);
                StartRound();
                dancingDotsLL.letterObjectView.ToggleDance();
               
            }
        }

        IEnumerator RoundLost()
        {
            yield return new WaitForSeconds(0.5f);
            AudioManager.I.PlaySfx(Sfx.Lose);
            //			dancingDotsLL.letterObjectView.SetState(LLAnimationStates.LL_idle);


            StartCoroutine(PoofOthers(dragableDots));
            StartCoroutine(PoofOthers(dragableDiacritics));
            dancingDotsLL.letterObjectView.SetDancingSpeed(1f);
            dancingDotsLL.letterObjectView.DoDancingLose(); // ("FallAndStand");
            yield return new WaitForSeconds(1.5f);
            dancingDotsLL.letterObjectView.DoSmallJump();
            dancingDotsLL.letterObjectView.ToggleDance(); //  SetState(LLAnimationStates.LL_dancing);
            StartCoroutine(CheckNewRound());
        }

        IEnumerator RoundWon()
        {
            if (!isTutRound)
            {
                numberOfRoundsWon++;
                currStarsNum = numberOfRoundsWon / 2;
                Context.GetOverlayWidget().SetStarsScore(currStarsNum);
                //Context.GetOverlayWidget().SetStarsThresholds(1, 3, 6);
            }

            yield return new WaitForSeconds(0.25f);
            //			dancingDotsLL.letterObjectView.SetState(LLAnimationStates.LL_walking);
            //			dancingDotsLL.letterObjectView.SetWalkingSpeed(1);

            AudioManager.I.PlaySfx(Sfx.Win);
            yield return new WaitForSeconds(1f);

            StartCoroutine(CheckNewRound());
        }

        public void EndGame()
        {
            dancingDotsLL.letterObjectView.DoDancingWin();
            isPlaying = false;
            dancingDotsLL.letterObjectView.SetState(LLAnimationStates.LL_idle);
            this.SetCurrentState(this.ResultState);
            //StartCoroutine(EndGame_Coroutine());
        }

        IEnumerator EndGame_Coroutine()
        {
            isPlaying = false;

            dancingDotsLL.letterObjectView.SetState(LLAnimationStates.LL_idle); // ("idle");

            yield return new WaitForSeconds(1f);

            endGameCanvas.gameObject.SetActive(true);

            int numberOfStars = 0;

            if (numberOfRoundsWon <= 0)
            {
                numberOfStars = 0;
                WidgetSubtitles.I.DisplaySentence("game_result_retry");
            }
            else if ((float)numberOfRoundsWon / numberOfRounds < 0.5f)
            {
                numberOfStars = 1;
                WidgetSubtitles.I.DisplaySentence("game_result_fair");
            }
            else if (numberOfRoundsWon < numberOfRounds)
            {
                numberOfStars = 2;
                WidgetSubtitles.I.DisplaySentence("game_result_good");
            }
            else
            {
                numberOfStars = 3;
                WidgetSubtitles.I.DisplaySentence("game_result_great");
            }

            LoggerEA4S.Log("minigame", "DancingDots", "correctLetters", numberOfRoundsWon.ToString());
            LoggerEA4S.Log("minigame", "DancingDots", "wrongLetters", (numberOfRounds - numberOfRoundsWon).ToString());
            LoggerEA4S.Save();

            starFlowers.Show(numberOfStars);
        }
        void startUI()
        {
            if (numberOfRoundsPlayed != 1)
                return;
            Debug.Log("UI Started");
            Context.GetOverlayWidget().Initialize(true, true, false);
            Context.GetOverlayWidget().SetClockDuration(gameDuration);
        }
    }

    

    [Serializable]
    public class DancingDotsGamePlayInfo : AnturaGameplayInfo
    {
        [Tooltip("Play session duration in seconds.")]
        public float PlayTime = 0f;
    }
}
