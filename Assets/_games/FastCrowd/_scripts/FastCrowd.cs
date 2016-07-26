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

namespace EA4S.FastCrowd {

    public class FastCrowd : MiniGameBase {

        #region GameSettings

        [Header("Star Rewards")]
        public int ThresholdStar1 = 3;
        public int ThresholdStar2 = 6;
        public int ThresholdStar3 = 9;

        [Header("Gameplay Info and Config section")]
        #region Overrides

        new public FastCrowdGameplayInfo GameplayInfo;

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
        public WordData ActualWord;
        public List<WordData> CompletedWords = new List<WordData>();

        [Header("Manager Settings")]
        public StarFlowers StarUI;
        public ActionFeedbackComponent ActionFeedback;
        public PopupMissionComponent PopupMission;
        public Button TutorialNextStepButton;

        #endregion

        #region Runtime Variables

        public TMPro.TextMeshProUGUI RightWordsCounter;
        public bool IsAnturaMoment = false;

        int tutorialState = 3;

        [HideInInspector]
        public string VariationPrefix = string.Empty;

        #endregion

        #region Setup and initialization

        protected override void ReadyForGameplay() {
            base.ReadyForGameplay();
            GameplayInfo = AppManager.Instance.Modules.GameplayModule.ActualGameplayInfo as FastCrowdGameplayInfo;
            if (GameplayInfo.Variant == FastCrowdGameplayInfo.GameVariant.living_words)
                VariationPrefix = "-2";

            AppManager.Instance.InitDataAI();
            // put here start logic

            // LOG: Start //
            LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "start", GameplayInfo.PlayTime.ToString());
            LoggerEA4S.Save();
            AudioManager.I.PlayMusic(Music.Relax);
        }



        /// <summary>
        /// 
        /// </summary>
        void gameplayBlockSetup() {
            // Get letters and word
            ActualWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
            AudioManager.I.PlayWord(ActualWord.Key);
            LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "newWord", ActualWord.Key);
            List<LetterData> gameLetters = ArabicAlphabetHelper.LetterDataListFromWord(ActualWord.Word, AppManager.Instance.Letters);

            // popup info 
            // Separated letters
            if (GameplayInfo.Variant == FastCrowdGameplayInfo.GameVariant.living_letters) {
                string sepLetters = string.Empty;
                foreach (var item in gameLetters) {
                    sepLetters += ArabicAlphabetHelper.GetLetterFromUnicode(item.Isolated_Unicode) + " ";
                }
                PopupMission.Show(new PopupMissionComponent.Data() {
                    Title = string.Format("Find the word {0}!", CompletedWords.Count + 1),
                    MainTextToDisplay = string.Format("{1} - {0}", ArabicAlphabetHelper.ParseWord(ActualWord.Word, AppManager.Instance.Letters), sepLetters),
                    Type = PopupMissionComponent.PopupType.New_Mission,
                    DrawSprite = GameplayInfo.Variant == FastCrowdGameplayInfo.GameVariant.living_words ? null : Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + ActualWord.Key),
                });
            } else {
                PopupMission.Show(new PopupMissionComponent.Data() {
                    Title = string.Format("Find the word {0}!", CompletedWords.Count + 1),
                    MainTextToDisplay = string.Format("{0}", ArabicAlphabetHelper.ParseWord(ActualWord.Word, AppManager.Instance.Letters), CompletedWords.Count + 1),
                    Type = PopupMissionComponent.PopupType.New_Mission,
                    DrawSprite = GameplayInfo.Variant == FastCrowdGameplayInfo.GameVariant.living_words ? null : Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + ActualWord.Key),
                });
            }

            int count = 0;
            // Letter from db filtered by some parameters
            foreach (LetterData letterData in gameLetters) {
                LetterObjectView letterObjectView = Instantiate(LetterPref);
                //letterObjectView.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // TODO: check for alternative solution!
                letterObjectView.transform.SetParent(TerrainTrans, true);
                Vector3 newPosition = Vector3.zero;
                GameplayHelper.RandomPointInWalkableArea(TerrainTrans.position, 20f, out newPosition);
                letterObjectView.transform.position = newPosition;
                letterObjectView.Init(letterData, GameplayInfo.BehaviourSettings);
                PlaceDropAreaElement(letterData, count);
                count++;
            }

            // Add other random letters
            for (int i = 0; i < (CompletedWords.Count < GameplayInfo.MaxNumbOfWrongLettersNoise ? CompletedWords.Count : GameplayInfo.MaxNumbOfWrongLettersNoise); i++) {
                LetterObjectView letterObjectView = Instantiate(LetterPref);
                //letterObjectView.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // TODO: check for alternative solution!
                letterObjectView.transform.SetParent(TerrainTrans, true);
                Vector3 newPosition = Vector3.zero;
                GameplayHelper.RandomPointInWalkableArea(TerrainTrans.position, 20f, out newPosition);
                letterObjectView.transform.position = newPosition;
                letterObjectView.Init(AppManager.Instance.Letters.GetRandomElement(), GameplayInfo.BehaviourSettings);
            }
            DropAreaContainer.SetupDone();
        }

        private void LetterDataListFromWord(object _word, object _vocabulary) {
            throw new NotImplementedException();
        }

        void sceneClean() {
            DropAreaContainer.clean();
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
        void PlaceDropAreaElement(LetterData _letterData, int position) {
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
        void OnTutorialStateChanged() {
            switch (tutorialState) {
                case 3:
                    WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro1");
                    TutorialNextStepButton.gameObject.SetActive(true);
                    break;
                case 2:
                    WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro2");
                    TutorialNextStepButton.gameObject.SetActive(true);
                    break;
                case 1:
                    WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro3");
                    TutorialNextStepButton.gameObject.SetActive(true);
                    break;
                default:
                    // play
                    WidgetSubtitles.I.DisplaySentence(string.Empty);
                    TutorialNextStepButton.gameObject.SetActive(false);
                    // Env Setup.
                    gameplayBlockSetup();

                    //GameplayTimer.Instance.StartTimer(GameplayInfo.PlayTime);
                    var AnturaTimea = UnityEngine.Random.Range(30, 50);
                    GameplayTimer.Instance.StartTimer(GameplayInfo.PlayTime,
                        new List<GameplayTimer.CustomEventData>()
                        {
                    new GameplayTimer.CustomEventData() { Name = "AnturaStart", Time = AnturaTimea },
                    new GameplayTimer.CustomEventData() { Name = "AnturaEnd", Time = AnturaTimea - 10 }
                        }
                    );

                    AudioManager.I.PlayMusic(Music.Theme3);
                    break;
            }
        }

        void TutorialNextStep() {
            tutorialState--;
        }


        /// <summary>
        /// Called when the time is over.
        /// </summary>
        /// <param name="_time"></param>
        private void GameplayTimer_OnTimeOver(float _time) {
            TutorialNextStepButton.gameObject.SetActive(true);

            LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "completedWords", CompletedWords.Count.ToString());
            int starCount = 0;
            // Open stars evaluation
            if (CompletedWords.Count >= ThresholdStar1 && CompletedWords.Count < ThresholdStar2) {
                starCount = 1;
                WidgetSubtitles.I.DisplaySentence("game_result_fair");
            } else if (CompletedWords.Count >= ThresholdStar2 && CompletedWords.Count < ThresholdStar3) {
                starCount = 2;
                WidgetSubtitles.I.DisplaySentence("game_result_good");
            } else if (CompletedWords.Count > ThresholdStar3) {
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
        private void DropContainer_OnObjectiveBlockCompleted() {
            LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "wordFinished", ActualWord.Key);
            LoggerEA4S.Save();
            AudioManager.I.PlayWord(ActualWord.Key);
            CompletedWords.Add(ActualWord);
            if (RightWordsCounter)
                RightWordsCounter.GetComponent<TMPro.TextMeshProUGUI>().text = CompletedWords.Count.ToString();
            PopupMission.Show(new PopupMissionComponent.Data() {
                                Title = string.Format("Word {0} Completed!", CompletedWords.Count + 1),
                                MainTextToDisplay = ActualWord.Word,
                                Type = PopupMissionComponent.PopupType.Mission_Completed,
                                DrawSprite = GameplayInfo.Variant == FastCrowdGameplayInfo.GameVariant.living_words ? null : Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + ActualWord.Key),
                            }
                            , delegate() {
                                ActualWord = null;
                                // Recall gameplayBlockSetup
                                sceneClean();
                                gameplayBlockSetup();
                            } );
        }

        /// <summary>
        /// Risen on letter or world match.
        /// </summary>
        private void Droppable_OnWrongMatch(LetterObjectView _letterView) {
            LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "badLetterDrop", _letterView.Model.Data.Key);
            ActionFeedback.Show(false);
            AudioManager.I.PlaySfx(Sfx.LetterSad);
        }

        /// <summary>
        /// Risen on letter or world match.
        /// </summary>
        private void Droppable_OnRightMatch(LetterObjectView _letterView) {
            LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "goodLetterDrop", _letterView.Model.Data.Key);
            ActionFeedback.Show(true);
            AudioManager.I.PlayLetter(_letterView.Model.Data.Key);
        }

        /// <summary>
        /// Hang catch.
        /// </summary>
        /// <param name="_letterView"></param>
        private void Hangable_OnLetterHangOn(LetterObjectView _letterView) {
            if (_letterView.Model.Data.Key == DropAreaContainer.GetActualDropArea().Data.Key) {
                LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "goodLetterHold", _letterView.Model.Data.Key);
                AudioManager.I.PlaySfx(Sfx.LetterHappy);
            } else {
                LoggerEA4S.Log("minigame", "fastcrowd" + VariationPrefix, "badLetterHold", _letterView.Model.Data.Key);
                AudioManager.I.PlaySfx(Sfx.LetterAngry);
            }
        }

        /// <summary>
        /// Hang relese.
        /// </summary>
        /// <param name="_letterView"></param>
        private void Hangable_OnLetterHangOff(LetterObjectView _letterView) {
            
        }

        /// <summary>
        /// Timer custom events delegates.
        /// </summary>
        /// <param name="_data"></param>
        private void GameplayTimer_OnCustomEvent(GameplayTimer.CustomEventData _data) {
            //Debug.LogFormat("Custom Event {0} at {1} sec.", _data.Name, _data.Time);
            switch (_data.Name) {
                case "AnturaStart":
                    IsAnturaMoment = true;
                    AudioManager.I.PlayMusic(Music.MainTheme);
                    break;
                case "AnturaEnd":
                    IsAnturaMoment = false;
                    AudioManager.I.PlayMusic(Music.Theme3);
                    break;
                default:
                    break;
            }
        }


        #endregion

        #region events subscription

        void OnEnable() {
            DropContainer.OnObjectiveBlockCompleted += DropContainer_OnObjectiveBlockCompleted;
            GameplayTimer.OnTimeOver += GameplayTimer_OnTimeOver;
            GameplayTimer.OnCustomEvent += GameplayTimer_OnCustomEvent;

            Droppable.OnRightMatch += Droppable_OnRightMatch;
            Droppable.OnWrongMatch += Droppable_OnWrongMatch;

            Hangable.OnLetterHangOn += Hangable_OnLetterHangOn;
            Hangable.OnLetterHangOff += Hangable_OnLetterHangOff;


            /// <summary>
            /// Monitoring Model property XXX value changes.
            /// </summary>
            this.transform.ObserveEveryValueChanged(x => tutorialState).Subscribe(_ => {
                OnTutorialStateChanged();
            }).AddTo(this);

            /// Tutorial button 
            TutorialNextStepButton.onClick.AddListener(() => TutorialNextStep());
        }

        void OnDisable() {
            DropContainer.OnObjectiveBlockCompleted -= DropContainer_OnObjectiveBlockCompleted;
            GameplayTimer.OnTimeOver -= GameplayTimer_OnTimeOver;
            GameplayTimer.OnCustomEvent -= GameplayTimer_OnCustomEvent;

            Droppable.OnRightMatch -= Droppable_OnRightMatch;
            Droppable.OnWrongMatch -= Droppable_OnWrongMatch;

            Hangable.OnLetterHangOn -= Hangable_OnLetterHangOn;
            Hangable.OnLetterHangOff -= Hangable_OnLetterHangOff;

            TutorialNextStepButton.onClick.RemoveAllListeners();
        }

        #endregion

        #region events

        public delegate void ObjectiveSetup(WordData _wordData);

        /// <summary>
        /// Called every time a new word objective is created.
        /// </summary>
        public static event ObjectiveSetup OnNewWordObjective;

        #endregion
    }

    #region AnturaGameplayInfo

    /// <summary>
    /// Gameplay info class data structure.
    /// </summary>
    [Serializable]
    public class FastCrowdGameplayInfo : AnturaGameplayInfo {

        [Tooltip("Game Variant")]
        public GameVariant Variant = GameVariant.living_letters;
        public enum GameVariant { living_letters, living_words }

        [Tooltip("Play session duration in seconds.")]
        public float PlayTime = 90;

        [Tooltip("Max number of addictional letter besides the word letters. First stage start with 0 addictional letters and every stage add 1 letter until max value reached.")]
        [Range(0, 9)]
        public int MaxNumbOfWrongLettersNoise = 3;

        public LetterBehaviour.BehaviourSettings BehaviourSettings = new LetterBehaviour.BehaviourSettings();

    }

    #endregion
}