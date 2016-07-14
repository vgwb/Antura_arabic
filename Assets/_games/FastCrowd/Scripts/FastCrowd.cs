using UnityEngine;
using System.Collections.Generic;
using EA4S;
using ModularFramework.Core;
using ModularFramework.Helpers;
using Google2u;
using System;
using ModularFramework.Modules;
using ArabicSupport;

namespace EA4S.FastCrowd {

    public class FastCrowd : MiniGameBase {

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
        public wordsRow ActualWord;
        public List<wordsRow> CompletedWords = new List<wordsRow>();

        [Header("Manager Settings")]
        public StarFlowers StarUI;
        public ActionFeedbackComponent ActionFeedback;
        public PopupMissionComponent PopupMission;

        protected override void ReadyForGameplay() {
            base.ReadyForGameplay();
            AppManager.Instance.InitDataAI();
            // put here start logic

            // LOG: Start //
            LoggerEA4S.Log("minigame", "fastcrowd", "start", GameplayInfo.PlayTime.ToString());
            LoggerEA4S.Save();


            // Gameplay Settings Override

            gameplayBlockSetup();

            GameplayTimer.Instance.StartTimer(GameplayInfo.PlayTime);
            AudioManager.I.PlayMusic(Music.MainTheme);
        }

        /// <summary>
        /// 
        /// </summary>
        void gameplayBlockSetup() {
            // Get letters and word
            ActualWord = AppManager.Instance.Teacher.GimmeAGoodWord();
            LoggerEA4S.Log("minigame", "fastcrowd", "newWord", ActualWord._id);
            LoggerEA4S.Log("minigame", "fastcrowd", "wordFinished", ActualWord._id);
            List<LetterData> gameLetters = ArabicAlphabetHelper.LetterDataListFromWord(ActualWord._word, AppManager.Instance.Letters);

            // popup info 
            string sepLetters = string.Empty;
            foreach (var item in gameLetters) {
                sepLetters += ArabicAlphabetHelper.GetLetterFromUnicode(item.Initial_Unicode) + " ";
            }
            PopupMission.Show(string.Format("{0} : {1}", ArabicFixer.Fix(ActualWord._word, false, false), sepLetters)
                , false);

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

        #region event subscription delegates

        /// <summary>
        /// Called when the time is over.
        /// </summary>
        /// <param name="_time"></param>
        private void GameplayTimer_OnTimeOver(float _time) {
            LoggerEA4S.Log("minigame", "fastcrowd", "completedWords", CompletedWords.Count.ToString());
            int starCount = 0;
            // Open stars evaluation
            if (CompletedWords.Count >= ThresholdStar1 && CompletedWords.Count < ThresholdStar2) {
                starCount = 1;
            } else if (CompletedWords.Count >= ThresholdStar2 && CompletedWords.Count < ThresholdStar3) {
                starCount = 2;
            } else if (CompletedWords.Count > ThresholdStar3) {
                starCount = 3;
            } else {
                starCount = 0;
            }

            LoggerEA4S.Log("minigame", "fastcrowd", "endScoreStars", starCount.ToString());
            StarUI.Show(starCount);
            LoggerEA4S.Save();
            AudioManager.I.PlayMusic(Music.Relax);
        }

        /// <summary>
        /// Called when objective block is completed (entire word).
        /// </summary>
        private void DropContainer_OnObjectiveBlockCompleted() {
            LoggerEA4S.Log("minigame", "fastcrowd", "wordFinished", ActualWord._id);
            LoggerEA4S.Save();
            AudioManager.I.PlayWord(ActualWord._id);
            CompletedWords.Add(ActualWord);
            PopupMission.Show(ActualWord._word, true, delegate() {
                ActualWord = null;
                // Recall gameplayBlockSetup
                sceneClean();
                gameplayBlockSetup();
            });
        }

        /// <summary>
        /// Risen on letter or world match.
        /// </summary>
        private void Droppable_OnWrongMatch(LetterObjectView _letterView) {
            LoggerEA4S.Log("minigame", "fastcrowd", "badLetterDrop", _letterView.Model.Data.Key);
            ActionFeedback.Show(false);
            AudioManager.I.PlaySound("LivingLetters/Angry");
        }

        /// <summary>
        /// Risen on letter or world match.
        /// </summary>
        private void Droppable_OnRightMatch(LetterObjectView _letterView) {
            LoggerEA4S.Log("minigame", "fastcrowd", "goodLetterDrop", _letterView.Model.Data.Key);
            ActionFeedback.Show(true);

        }

        /// <summary>
        /// Hang catch.
        /// </summary>
        /// <param name="_letterView"></param>
        private void Hangable_OnLetterHangOn(LetterObjectView _letterView) {
            if(_letterView.Model.Data.Key == DropAreaContainer.GetActualDropArea().Data.Key)
                LoggerEA4S.Log("minigame", "fastcrowd", "goodLetterHold", _letterView.Model.Data.Key);
            else
                LoggerEA4S.Log("minigame", "fastcrowd", "badLetterHold", _letterView.Model.Data.Key);
        }

        /// <summary>
        /// Hang relese.
        /// </summary>
        /// <param name="_letterView"></param>
        private void Hangable_OnLetterHangOff(LetterObjectView _letterView) {
            
        }

        #endregion

        #region events subscription

        void OnEnable() {
            DropContainer.OnObjectiveBlockCompleted += DropContainer_OnObjectiveBlockCompleted;
            GameplayTimer.OnTimeOver += GameplayTimer_OnTimeOver;

            Droppable.OnRightMatch += Droppable_OnRightMatch;
            Droppable.OnWrongMatch += Droppable_OnWrongMatch;

            Hangable.OnLetterHangOn += Hangable_OnLetterHangOn;
            Hangable.OnLetterHangOff += Hangable_OnLetterHangOff;
        }

        void OnDisable() {
            DropContainer.OnObjectiveBlockCompleted -= DropContainer_OnObjectiveBlockCompleted;
            GameplayTimer.OnTimeOver -= GameplayTimer_OnTimeOver;

            Droppable.OnRightMatch -= Droppable_OnRightMatch;
            Droppable.OnWrongMatch -= Droppable_OnWrongMatch;

            Hangable.OnLetterHangOn -= Hangable_OnLetterHangOn;
            Hangable.OnLetterHangOff -= Hangable_OnLetterHangOff;
        }

        #endregion
    }

    /// <summary>
    /// Gameplay info class data structure.
    /// </summary>
    [Serializable]
    public class FastCrowdGameplayInfo : AnturaGameplayInfo
    {

        [Tooltip("Play session duration in seconds.")]
        public float PlayTime = 10;

        [Tooltip("Max number of addictional letter besides the word letters. First stage start with 0 addictional letters and every stage add 1 letter until max value reached.")]
        [Range(0, 9)]
        public int MaxNumbOfWrongLettersNoise = 3;

        public LetterBehaviour.BehaviourSettings BehaviourSettings = new LetterBehaviour.BehaviourSettings();

    }
}