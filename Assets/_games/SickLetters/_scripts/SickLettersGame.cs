using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;

namespace EA4S.SickLetters
{
    public enum Diacritic { Sokoun, Fatha, Dameh, Kasrah };

    public class SickLettersGame : MiniGame
    {
        public SickLettersLLPrefab LLPrefab;
        public SickLettersVase scale;
        public GameObject DropZonesGO;
        
        public SickLettersCamera slCamera;
        public SickLettersGameManager manager;

        [HideInInspector]
        public int maxRoundsCount = 6, successRoundsCount = 0, wrongDraggCount = 0;

        public int gameDuration = 120 ,  targetScale = 10;
        public float vaseWidth = 5.20906f;
        public bool LLCanDance = false;
        public string dotlessLetters = "أ ا ى ر س ل ص ع ه ح د م ك ط ئ ء ؤ و";

        public SickLettersDraggableDD[] Draggables;

        [HideInInspector]
        public SickLettersDropZone[] DropZones;
        [HideInInspector]
        public List<SickLettersDraggableDD> allWrongDDs = new List<SickLettersDraggableDD>();

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

            manager = GetComponent<SickLettersGameManager>();
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
            checkSucess();

            int i = 0;
            foreach (SickLettersDraggableDD dd in LLPrefab.thisLLWrongDDs)
            {
                if (dd && !dd.deattached)
                    i++;
            }

            if (i == 0)
            {
                successRoundsCount++;
                LLPrefab.letterView.DoHorray();
                SickLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.LetterHappy);
                LLPrefab.jumpOut(1.5f);
                return true;
            }
            else
                return false;
        }

        public void checkSucess()
        {
            if( scale.counter == targetScale)
            {
                successRoundsCount = 6;
                manager.sucess();
            }
        }


        public void setDifficulty(int gameDuration, int targetScale, float vaseWidth, bool LLCanDance)
        {
            this.gameDuration = gameDuration;
            this.targetScale = targetScale;
            this.vaseWidth = vaseWidth;
            this.LLCanDance = LLCanDance;
        }   
    }
}
