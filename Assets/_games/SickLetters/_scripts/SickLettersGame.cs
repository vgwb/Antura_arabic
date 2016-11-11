using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;

namespace EA4S.SickLetters
{
    public enum Diacritic { Sokoun, Fatha, Dameh, Kasrah, None };

    public class SickLettersGame : MiniGame
    {
        public SickLettersLLPrefab LLPrefab;
        public SickLettersAntura antura;
        public SickLettersVase scale;
        public GameObject DropZonesGO;
        public UnityEngine.Animation hole;

        public SickLettersCamera slCamera;
        public SickLettersGameManager manager;

        [HideInInspector]
        public MinigamesUIStarbar uiSideBar;
        [HideInInspector]
        public MinigamesUITimer uiTimer;
        //[HideInInspector]
        public int maxRoundsCount = 6, roundsCount = 1, wrongDraggCount = 0, currentStars = 0;
        [HideInInspector]
        public bool disableInput;

        public int gameDuration = 120 ,  targetScale = 10;
        public float vaseWidth = 5.20906f;
        public bool LLCanDance = false, with7arakat;
        public string dotlessLetters = "أ ا ى ر س ل ص ع ه ح د م ك ط ئ ء ؤ و";

        public SickLettersDraggableDD[] Draggables;

        [HideInInspector]
        public SickLettersDropZone[] DropZones;

       

        [HideInInspector]
        public List<SickLettersDraggableDD> allWrongDDs = new List<SickLettersDraggableDD>();
        [HideInInspector]
        public QuestionsManager questionsManager;


        public IntroductionGameState IntroductionState { get; private set; }
        public QuestionGameState QuestionState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        public ResultGameState ResultState { get; private set; }

        public QuestionsManager questionManager;

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new IntroductionGameState(this);
            QuestionState = new QuestionGameState(this);
            PlayState = new PlayGameState(this);
            ResultState = new ResultGameState(this);
            questionManager = new QuestionsManager();

            manager = GetComponent<SickLettersGameManager>();
            //anturaAnimator = GetComponent<Animator>();
            DropZones = DropZonesGO.GetComponentsInChildren<SickLettersDropZone>();
            scale.game = this;
            scale.transform.localScale = new Vector3(vaseWidth, scale.transform.localScale.y, scale.transform.localScale.z);
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return SickLettersConfiguration.Instance;
        }

        public SickLettersDraggableDD createNewDragable(GameObject go)
        {
            return Instantiate(go).GetComponent<SickLettersDraggableDD>();
        }

        public void Poof(Vector3 atPos)
        {
            SickLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Poof);
            var puffGo = GameObject.Instantiate(LLPrefab.GetComponent<LetterObjectView>().poofPrefab);
            puffGo.AddComponent<AutoDestroy>().duration = 2;
            puffGo.SetActive(true);
            puffGo.transform.position = atPos - Vector3.forward * 2;
            puffGo.transform.localScale *= 0.75f;
        }

        public bool checkForNextRound()
        {
            if (checkSucess())
                return false;

            if (StateManager.CurrentState == ResultState)
                return false;

            int i = 0;
            foreach (SickLettersDraggableDD dd in LLPrefab.thisLLWrongDDs)
            {
                if (dd && !dd.deattached)
                    i++;
            }

            if (i == 0)
            {
                if (roundsCount == maxRoundsCount)
                {
                    this.SetCurrentState(ResultState);
                    return false;
                } 

                roundsCount++;
                //Context.GetOverlayWidget().SetStarsScore(roundsCount / 2);
                LLPrefab.letterAnimator.SetBool("dancing", false);
                LLPrefab.letterAnimator.Play("LL_idle_1", -1);
                LLPrefab.letterView.DoHorray();
                SickLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.LetterHappy);
                LLPrefab.jumpOut(1.5f);
                return true;
            }
            else
                return false;
        }

        public bool checkSucess()
        {
            if (scale.counter == targetScale)
            {
                manager.sucess();
                return true;
            }
            else
                return false;
        }

        

        public void setDifficulty(float diff, int gameDuration, int targetScale, float vaseWidth, bool LLCanDance, bool with7arakat)
        {
            this.gameDuration = gameDuration;
            Context.GetOverlayWidget().SetClockDuration(gameDuration);
            this.targetScale = targetScale;

            if(diff> 0.666f)
                scale.transform.localScale = new Vector3(vaseWidth, scale.transform.localScale.y, 7.501349f);
            else
                scale.transform.localScale = new Vector3(vaseWidth, scale.transform.localScale.y, scale.transform.localScale.z);

            this.LLCanDance = LLCanDance;
            this.with7arakat = with7arakat;
        }

        float prevDiff = -1;
        public void peocessDifiiculties(float diff)
        {
            if (prevDiff == diff)
                return;
            else
                prevDiff = diff;


            if (diff < 0.333f)
                setDifficulty(diff, 120, 18, 5.20906f, false, false);
            else if (diff < 0.666f)
                setDifficulty(diff, 120, 42, 4.0f, false, true);
            else
                setDifficulty(diff, 160, 42, 3.0f, true, true);
        }

        
    }
}
