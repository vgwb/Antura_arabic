using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using ModularFramework.Modules;
using System;
using ArabicSupport;
using UniRx;
using EA4S;
using Google2u;

namespace EA4S.FastCrowd
{

    public class FastCrowd : MiniGameBase
    {

        #region GameSettings

        [Header("Star Rewards")]
        public int ThresholdStar1 = 3;
        public int ThresholdStar2 = 6;
        public int ThresholdStar3 = 9;

        //[Header("Gameplay Info and Config section")]
        #region Overrides

        //new public FastCrowdGameplayInfo GameplayInfo;

        new public static FastCrowd Instance
        {
            get { return SubGame.Instance as FastCrowd; }
        }

        #endregion

        [Header("Letters Env")]
        public LetterObjectView LetterPref;
        public Transform TerrainTrans;

        [Header("Drop Area")]
        public DropSingleArea DropSingleAreaPref;
        public DropContainer DropAreaContainer;

        [Header("Gameplay")]
        public int MinLettersOnField = 10;
        //List<LetterData> letters = LetterDataListFromWord(_word, _vocabulary);

        public List<WordData> CompletedWords = new List<WordData>();

        [Header("References")]
        public StarFlowers StarUI;
        public ActionFeedbackComponent ActionFeedback;
        public PopupMissionComponent PopupMission;
        public Sprite TutorialImage;
        public Sprite TutorialImageWords;

        #endregion

        #region Runtime Variables

        public TMPro.TextMeshProUGUI RightWordsCounter;

        /// <summary>
        /// Actual word.
        /// </summary>
        WordData ActualWord;
        /// <summary>
        /// Actual round element data (letter of word, group of words, etc...).
        /// </summary>
        List<ILivingLetterData> dataList = new List<ILivingLetterData>();
        int round = 0;
        int tutorialState = -1;

        [HideInInspector]
        public string VariationPrefix = string.Empty;

        bool OnTimeOverReserved = false;

        #endregion

        #region Setup and initialization

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            if (SceneTransitioner.IsShown) {
                SceneTransitioner.Close();
            }
        }

        protected override void ReadyForGameplay()
        {
            base.ReadyForGameplay();
            AppManager.Instance.CurrentGameManagerGO = gameObject; // ?? why?
            //if (!UseTestGameplayInfo)
            //    GameplayInfo = AppManager.Instance.Modules.GameplayModule.ActualGameplayInfo as FastCrowdGameplayInfo;
            //if (GameplayInfo == null)
            //    GameplayInfo = new FastCrowdGameplayInfo();
            //if (GameplayInfo.Variant == FastCrowdGameplayInfo.GameVariant.living_words)
            //    VariationPrefix = "_words";

            // New variations
            VariationPrefix = "";
            switch (FastCrowdConfiguration.Instance.Variation) {
                case FastCrowdVariation.Spelling:
                    VariationPrefix = "";
                    break;
                case FastCrowdVariation.Words:
                    VariationPrefix = "_words";
                    break;
                default:
                    FastCrowdConfiguration.Instance.Variation = FastCrowdVariation.Spelling;
                    break;
            }

            AppManager.Instance.InitDataAI();
            // put here start logic

            // LOG: Start //
            LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "start", FastCrowdConfiguration.Instance.PlayTime.ToString());
            LoggerEA4S.Save();
            AudioManager.I.PlayMusic(Music.Relax);

            // Env Setup.
            gameplayBlockSetup();

            //GameplayTimer.Instance.StartTimer(GameplayInfo.PlayTime);
            var AnturaTimea = UnityEngine.Random.Range(30, 50);
            GameplayTimer.Instance.StartTimer(FastCrowdConfiguration.Instance.PlayTime,
                new List<GameplayTimer.CustomEventData>()
                {
                    new GameplayTimer.CustomEventData() { Name = "AnturaStart", Time = AnturaTimea },
                    new GameplayTimer.CustomEventData() { Name = "AnturaEnd", Time = AnturaTimea - 10 }
                }
            );

            AudioManager.I.PlayMusic(Music.Theme3);

            if (OnReadyForGameplayDone != null)
                OnReadyForGameplayDone(GameplayInfo);


        }


        /// <summary>
        /// 
        /// </summary>
        void gameplayBlockSetup()
        {
            // data from db 
            dataList = new List<ILivingLetterData>();
            string sepLetters = string.Empty;
            if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Spelling) {
                ActualWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                AudioManager.I.PlayWord(ActualWord.Key);
                LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "newWord", ActualWord.Key);
                foreach (var data in ArabicAlphabetHelper.LetterDataListFromWord(ActualWord.Word, AppManager.Instance.Letters)) {
                    dataList.Add(data);
                    sepLetters += ArabicAlphabetHelper.GetLetterFromUnicode(data.Isolated_Unicode) + " ";
                }
                ;
            } else { // word variation
                for (int i = 0; i < FastCrowdConfiguration.Instance.MaxNumbOfWrongLettersNoise; i++) {
                    WordData newWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    if (!dataList.Contains(newWord)) {
                        dataList.Add(newWord);
                    } else
                        i--;
                }
            }

            // popup info 
            if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Spelling) {
                LocalizationDataRow rowLetters = LocalizationData.Instance.GetRow("game_fastcrowd_findword");
                //row.GetStringData("English");
                PopupMission.Show(new PopupMissionComponent.Data()
                    {
                        // Find the word
                        Title = string.Format("{1}", ArabicFixer.Fix(rowLetters.GetStringData("Arabic"), false, false), CompletedWords.Count + 1),
                        MainTextToDisplay = string.Format("{0}", ArabicAlphabetHelper.ParseWord(ActualWord.Word, AppManager.Instance.Letters), sepLetters),
                        Type = PopupMissionComponent.PopupType.New_Mission,
                        DrawSprite = ActualWord.DrawForLivingLetter,
                    });
            } else {
                string stringListOfWords = string.Empty;
                foreach (var w in dataList)
                    stringListOfWords += w.TextForLivingLetter + " ";
                LocalizationDataRow rowWords = LocalizationData.Instance.GetRow("game_fastcrowd_findwordgroup");
                PopupMission.Show(new PopupMissionComponent.Data()
                    {
                        // Find the word group
                        Title = string.Format("{1}", ArabicFixer.Fix(rowWords.GetStringData("Arabic"), false, false), CompletedWords.Count + 1),
                        MainTextToDisplay = string.Format("{0}", stringListOfWords),
                        Type = PopupMissionComponent.PopupType.New_Mission,
                    });
            }

            int count = 0;
            // Living letters
            foreach (ILivingLetterData data in dataList) {
                LetterObjectView letterObjectView = Instantiate(LetterPref);
                //letterObjectView.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // TODO: check for alternative solution!
                letterObjectView.transform.SetParent(TerrainTrans, true);
                Vector3 newPosition = Vector3.zero;
                GameplayHelper.RandomPointInWalkableArea(TerrainTrans.position, 20f, out newPosition);
                letterObjectView.transform.position = newPosition;
                letterObjectView.Init(data, FastCrowdConfiguration.Instance.BehaviourSettings);
                PlaceDropAreaElement(data, count);
                count++;
            }

            // Add other random livingletters
            for (int i = 0; i < (round < FastCrowdConfiguration.Instance.MaxNumbOfWrongLettersNoise ? round : FastCrowdConfiguration.Instance.MaxNumbOfWrongLettersNoise); i++) {
                LetterObjectView letterObjectView = Instantiate(LetterPref);
                //letterObjectView.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // TODO: check for alternative solution!
                letterObjectView.transform.SetParent(TerrainTrans, true);
                Vector3 newPosition = Vector3.zero;
                GameplayHelper.RandomPointInWalkableArea(TerrainTrans.position, 20f, out newPosition);
                letterObjectView.transform.position = newPosition;
                // Get random data element of different type according with game variation type
                if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Spelling) {
                    letterObjectView.Init(AppManager.Instance.Letters.GetRandomElement(), FastCrowdConfiguration.Instance.BehaviourSettings);
                } else {
                    letterObjectView.Init(WordData.GetWordCollection().GetRandomElement(), FastCrowdConfiguration.Instance.BehaviourSettings);
                }
            }
            DropAreaContainer.SetupDone();
        }

        void sceneClean()
        {
            DropAreaContainer.Clean();
            foreach (LetterObjectView item in TerrainTrans.GetComponentsInChildren<LetterObjectView>()) {
                GameObject.Destroy(item.gameObject);
            }
            foreach (DropSingleArea item in DropAreaContainer.GetComponentsInChildren<DropSingleArea>()) {
                GameObject.Destroy(item.gameObject);
            }
        }

        /// <summary>
        /// Place drop object area in drop object area container.
        /// </summary>
        /// <param name="_letterData"></param>
        void PlaceDropAreaElement(ILivingLetterData _letterData, int position)
        {
            DropSingleArea dropSingleArea = Instantiate(DropSingleAreaPref);
            dropSingleArea.transform.SetParent(DropAreaContainer.transform, false);
            dropSingleArea.transform.position = Camera.main.transform.position;
            dropSingleArea.Init(_letterData, DropAreaContainer);

        }

        #endregion

        #region event subscription delegates

        /// <summary>
        /// Called when tutorial state changes.
        /// </summary>
        void OnTutorialStateChanged()
        {
            switch (tutorialState) {
                case 3:
                    if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Spelling) {
                        WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro1");
                        WidgetPopupWindow.I.ShowTutorial(TutorialNextStep, TutorialImage);
                    } else {
                        WidgetSubtitles.I.DisplaySentence("game_fastcrowd_A_intro1");
                        WidgetPopupWindow.I.ShowTutorial(TutorialNextStep, TutorialImageWords);
                    }
                    break;
                case 2:
                    if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Spelling) {
                        WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro2");
                        WidgetPopupWindow.I.ShowTutorial(TutorialNextStep, TutorialImage);
                    } else {
                        WidgetSubtitles.I.DisplaySentence("game_fastcrowd_A_intro2");
                        WidgetPopupWindow.I.ShowTutorial(TutorialNextStep, TutorialImageWords);
                    }
                    break;
                case 1:
                    WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro3");
                    if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Spelling) {
                        WidgetPopupWindow.I.ShowTutorial(TutorialNextStep, TutorialImage);
                    } else {
                        WidgetPopupWindow.I.ShowTutorial(TutorialNextStep, TutorialImageWords);
                    }
                    break;
                default:
                    // play
                    WidgetSubtitles.I.DisplaySentence(string.Empty);
                    WidgetPopupWindow.I.Close();

                    break;
            }
        }

        void TutorialNextStep()
        {
            tutorialState--;
        }


        /// <summary>
        /// Called when the time is over.
        /// </summary>
        /// <param name="_time"></param>
        private void GameplayTimer_OnTimeOver(float _time)
        {
            // during normal gameplay call doTimeOverActions
            if (DropAreaContainer.GetActualDropArea() != null) { 
                doTimeOverActions();
            } else { 
                // during objective evaluation
                OnTimeOverReserved = true;
            }
        }

        /// <summary>
        /// Action for timeover.
        /// </summary>
        void doTimeOverActions()
        {
            sceneClean();
            LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "completedWords", round.ToString());
            int starCount = 0;
            // Open stars evaluation
            if (round >= ThresholdStar1 && round < ThresholdStar2) {
                starCount = 1;
                WidgetSubtitles.I.DisplaySentence("game_result_fair");
            } else if (round >= ThresholdStar2 && round < ThresholdStar3) {
                starCount = 2;
                WidgetSubtitles.I.DisplaySentence("game_result_good");
            } else if (round >= ThresholdStar3) {
                starCount = 3;
                WidgetSubtitles.I.DisplaySentence("game_result_great");
            } else {
                starCount = 0;
                WidgetSubtitles.I.DisplaySentence("game_result_retry");
            }

            LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "endScoreStars", starCount.ToString());

            StarUI.Show(starCount);
            LoggerEA4S.Save();
            AudioManager.I.PlayMusic(Music.Relax);
        }

        /// <summary>
        /// Called when objective block is completed (entire word).
        /// </summary>
        private void DropContainer_OnObjectiveBlockCompleted()
        {
            ObjectiveBlockCompletedSequence(FastCrowdConfiguration.Instance.Variation);
        }

        /// <summary>
        /// All action for BlockCompleted event for any variation.
        /// </summary>
        /// <param name="_variant"></param>
        void ObjectiveBlockCompletedSequence(FastCrowdVariation _variant)
        {
            round++;
            switch (_variant) {

                case FastCrowdVariation.Spelling:
                    LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "wordFinished", ActualWord.Key);
                    LoggerEA4S.Save();
                    AudioManager.I.PlayWord(ActualWord.Key);
                    CompletedWords.Add(ActualWord);
                    if (RightWordsCounter)
                        RightWordsCounter.GetComponent<TMPro.TextMeshProUGUI>().text = CompletedWords.Count.ToString();
                    LocalizationDataRow rowLetters = LocalizationData.Instance.GetRow("comment_welldone");
                    PopupMission.Show(new PopupMissionComponent.Data()
                        {
                            Title = string.Format("{0}", ArabicFixer.Fix(rowLetters.GetStringData("Arabic"), false, false), CompletedWords.Count),
                            MainTextToDisplay = ActualWord.TextForLivingLetter,
                            Type = PopupMissionComponent.PopupType.Mission_Completed,
                            DrawSprite = FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Words ? null : Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + ActualWord.Key),
                        }
                        , delegate ()
                        {
                            ActualWord = null;
                            // Recall gameplayBlockSetup
                            sceneClean();
                            if (OnTimeOverReserved)
                                doTimeOverActions();
                            else
                                gameplayBlockSetup();
                        });
                    break;

                case FastCrowdVariation.Words:
                    string stringListOfWords = string.Empty;
                    foreach (var w in dataList)
                        stringListOfWords += w.TextForLivingLetter + " ";
                    AudioManager.I.PlayDialog("comment_welldone");
                    LocalizationDataRow rowWords = LocalizationData.Instance.GetRow("comment_welldone");
                    PopupMission.Show(new PopupMissionComponent.Data()
                        {
                            Title = string.Format("{0}", ArabicFixer.Fix(rowWords.GetStringData("Arabic"), false, false), CompletedWords.Count),
                            MainTextToDisplay = stringListOfWords,
                            Type = PopupMissionComponent.PopupType.Mission_Completed,
                            DrawSprite = FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Words ? null : Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + ActualWord.Key),
                        }
                        , delegate ()
                        {
                            ActualWord = null;
                            // Recall gameplayBlockSetup
                            sceneClean();
                            if (OnTimeOverReserved)
                                doTimeOverActions();
                            else
                                gameplayBlockSetup();
                        });
                    if (RightWordsCounter)
                        RightWordsCounter.GetComponent<TMPro.TextMeshProUGUI>().text = round.ToString();
                    break;

            }

        }

        /// <summary>
        /// Risen on letter or world match.
        /// </summary>
        private void Droppable_OnWrongMatch(LetterObjectView _letterView)
        {
            LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "badLetterDrop", _letterView.Model.Data.Key);
            ActionFeedback.Show(false);
            AudioManager.I.PlaySfx(Sfx.LetterSad);
        }

        /// <summary>
        /// Risen on letter or world match.
        /// </summary>
        private void Droppable_OnRightMatch(LetterObjectView _letterView)
        {
            LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "goodLetterDrop", _letterView.Model.Data.Key);
            if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Spelling) {
                ActionFeedback.Show(true);
                AudioManager.I.PlayLetter(_letterView.Model.Data.Key);
            } else {
                LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "wordFinished", _letterView.Model.Data.Key);
                LoggerEA4S.Save();
                ActionFeedback.Show(true);
                AudioManager.I.PlayWord(_letterView.Model.Data.Key);
                CompletedWords.Add(_letterView.Model.Data as WordData);
            }
        }

        /// <summary>
        /// Hang catch.
        /// </summary>
        /// <param name="_letterView"></param>
        private void Hangable_OnLetterHangOn(LetterObjectView _letterView)
        {
            if (!DropAreaContainer.GetActualDropArea())
                return;
            if (_letterView.Model.Data.Key == DropAreaContainer.GetActualDropArea().Data.Key) {
                LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "goodLetterHold", _letterView.Model.Data.Key);
                //AudioManager.I.PlaySfx(Sfx.LetterHappy);
            } else {
                LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "badLetterHold", _letterView.Model.Data.Key);
                //AudioManager.I.PlaySfx(Sfx.LetterAngry);
            }
        }

        /// <summary>
        /// Hang relese.
        /// </summary>
        /// <param name="_letterView"></param>
        private void Hangable_OnLetterHangOff(LetterObjectView _letterView)
        {

        }

        /// <summary>
        /// Timer custom events delegates.
        /// </summary>
        /// <param name="_data"></param>
        private void GameplayTimer_OnCustomEvent(GameplayTimer.CustomEventData _data)
        {
            //Debug.LogFormat("Custom Event {0} at {1} sec.", _data.Name, _data.Time);
            switch (_data.Name) {
                case "AnturaStart":
                    foreach (LetterNavBehaviour item in TerrainTrans.GetComponentsInChildren<LetterNavBehaviour>())
                    {
                        item.isAnturaMoment = true;
                    }
                    AudioManager.I.PlayMusic(Music.MainTheme);
                    break;
                case "AnturaEnd":
                    foreach (LetterNavBehaviour item in TerrainTrans.GetComponentsInChildren<LetterNavBehaviour>())
                    {
                        item.isAnturaMoment = false;
                    }
                    AudioManager.I.PlayMusic(Music.Theme3);
                    break;
                default:
                    break;
            }
        }


        #endregion

        #region events subscription

        void OnEnable()
        {
            DropContainer.OnObjectiveBlockCompleted += DropContainer_OnObjectiveBlockCompleted;
            GameplayTimer.OnTimeOver += GameplayTimer_OnTimeOver;
            GameplayTimer.OnCustomEvent += GameplayTimer_OnCustomEvent;

            Droppable.OnRightMatch += Droppable_OnRightMatch;
            Droppable.OnWrongMatch += Droppable_OnWrongMatch;

            Hangable.OnLetterHangOn += Hangable_OnLetterHangOn;
            Hangable.OnLetterHangOff += Hangable_OnLetterHangOff;


            ///// <summary>
            ///// Monitoring Model property XXX value changes.
            ///// </summary>
            //this.transform.ObserveEveryValueChanged(x => tutorialState).Subscribe(_ =>
            //    {
            //        OnTutorialStateChanged();
            //    }).AddTo(this);

        }

        protected override void OnDisable()
        {
            base.OnDisable();

            DropContainer.OnObjectiveBlockCompleted -= DropContainer_OnObjectiveBlockCompleted;
            GameplayTimer.OnTimeOver -= GameplayTimer_OnTimeOver;
            GameplayTimer.OnCustomEvent -= GameplayTimer_OnCustomEvent;

            Droppable.OnRightMatch -= Droppable_OnRightMatch;
            Droppable.OnWrongMatch -= Droppable_OnWrongMatch;

            Hangable.OnLetterHangOn -= Hangable_OnLetterHangOn;
            Hangable.OnLetterHangOff -= Hangable_OnLetterHangOff;
        }

        #endregion

        #region events

        public delegate void ObjectiveSetup(WordData _wordData);

        /// <summary>
        /// Called every time a new word objective is created.
        /// </summary>
        public static event ObjectiveSetup OnNewWordObjective;


        public delegate void SubGameEvent(IGameplayInfo _gameplayInfo);

        /// <summary>
        /// Called after OnReadyForGameplay event in sub game.
        /// </summary>
        public static event SubGameEvent OnReadyForGameplayDone;


        #endregion

        #region debug

        public void DebugForceEndGame()
        {
            GameplayTimer.Instance.EndTimeRemaning();
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                DebugForceEndGame();
        }

        #endregion
    }

    #region AnturaGameplayInfo

    /// <summary>
    /// Gameplay info class data structure.
    /// </summary>
    [Serializable]
    public class FastCrowdGameplayInfo : AnturaGameplayInfo
    {

        [Tooltip("Game Variant")]
        public GameVariant Variant = GameVariant.living_letters;

        public enum GameVariant {
            living_letters,
            living_words

        }

        [Tooltip("Play session duration in seconds.")]
        public float PlayTime = 90;

        [Tooltip("Max number of addictional letter besides the word letters. First stage start with 0 addictional letters and every stage add 1 letter until max value reached.")]
        [Range(0, 9)]
        public int MaxNumbOfWrongLettersNoise = 3;

        public LetterBehaviour.BehaviourSettings BehaviourSettings = new LetterBehaviour.BehaviourSettings();

    }

    #endregion
}