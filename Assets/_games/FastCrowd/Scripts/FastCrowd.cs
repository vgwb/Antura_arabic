using UnityEngine;
using System.Collections.Generic;
using EA4S;
using ModularFramework.Core;
using ModularFramework.Helpers;
using Google2u;
using System;
using ModularFramework.Modules;

namespace EA4S.FastCrowd
{

    public class FastCrowd : MiniGameBase
    {

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
        public string ActualWord;
        public List<string> CompletedWords = new List<string>();

        protected override void ReadyForGameplay() {
            base.ReadyForGameplay();
            // put here start logic
            Debug.LogFormat("Game {0} ready!", GameplayInfo.GameId);

            // Gameplay Settings Override

            gameplayBlockSetup();

            GameplayTimer.Instance.StartTimer(GameplayInfo.PlayTime);
        }

        /// <summary>
        /// 
        /// </summary>
        void gameplayBlockSetup() {
            // Get letters and word
            // TODO: Only for pre-alpha. This logic must be in Antura app logic.
            ActualWord = words.Instance.Rows.GetRandomElement()._word;
            List<LetterData> gameLetters = ArabicAlphabetHelper.LetterDataListFromWord(ActualWord, AppManager.Instance.Letters);

            int count = 0;
            // Letter from db filtered by some parameters
            foreach (LetterData letterData in gameLetters) {
                LetterObjectView letterObjectView = Instantiate(LetterPref);
                //letterObjectView.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // TODO: check for alternative solution!
                letterObjectView.transform.SetParent(TerrainTrans, true);
                Vector3 newPosition = Vector3.zero;
                GameplayHelper.RandomPointInWalkableArea(TerrainTrans.position, 100f, out newPosition);
                letterObjectView.Init(letterData, GameplayInfo.BehaviourSettings);
                PlaceDropAreaElement(letterData, count);
                count++;
            }

            // Add other random letters
            for (int i = 0; i < GameplayInfo.NumbOfWrongLettersNoise; i++) {
                LetterObjectView letterObjectView = Instantiate(LetterPref);
                //letterObjectView.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // TODO: check for alternative solution!
                letterObjectView.transform.SetParent(TerrainTrans, true);
                Vector3 newPosition = Vector3.zero;
                GameplayHelper.RandomPointInWalkableArea(TerrainTrans.position, 100f, out newPosition);
                // TODO: the selection is curiously only between the letters of the word... to be checked.
                letterObjectView.Init(AppManager.Instance.Letters.GetRandomElement(), GameplayInfo.BehaviourSettings);
            }
            DropAreaContainer.SetupDone();
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
            Debug.Log("Time is over");
            // Open stars evaluation
        }

        /// <summary>
        /// Called when objective block is completed (entire word).
        /// </summary>
        private void DropContainer_OnObjectiveBlockCompleted() {
            Debug.Log("Word completed: " + ActualWord);
            CompletedWords.Add(ActualWord);
            ActualWord = string.Empty;
            // Recall gameplayBlockSetup
            sceneClean();
            gameplayBlockSetup();
        }
        #endregion

        #region events subscription
        void OnEnable() {
            DropContainer.OnObjectiveBlockCompleted += DropContainer_OnObjectiveBlockCompleted;
            GameplayTimer.OnTimeOver += GameplayTimer_OnTimeOver;
        }

        void OnDisable() {
            DropContainer.OnObjectiveBlockCompleted -= DropContainer_OnObjectiveBlockCompleted;
            GameplayTimer.OnTimeOver -= GameplayTimer_OnTimeOver;
        }

        #endregion
    }

    /// <summary>
    /// Gameplay info class data structure.
    /// </summary>
    [Serializable]
    public class FastCrowdGameplayInfo : AnturaGameplayInfo
    {
        public float PlayTime = 10;
        [Range(0, 6)]
        public int NumbOfWrongLettersNoise = 3;
        public LetterBehaviour.BehaviourSettings BehaviourSettings = new LetterBehaviour.BehaviourSettings();
    }
}