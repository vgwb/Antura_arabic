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
   
        public int gameDuration = 120 , targetScale = 10, successRoundsCount = 0 ,wrongDraggCount = 0;
        public string dotlessLetters = "أ ا ى ر س ل ص ع ه ح د م ك ط ئ ء ؤ و";

        public SickLettersDropZone[] DropZones;
        public SickLettersDraggableDD[] Draggables;

        

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

            
            DropZones = DropZonesGO.GetComponentsInChildren<SickLettersDropZone>();
            scale.game = this;
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

        public bool startNextRound()
        {
            checkScaleTargetReach();

            int i = 0;
            foreach (SickLettersDraggableDD dd in LLPrefab.thisLetterDD)
            {
                if (dd && !dd.destroyOnNewRound)
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

        public void checkScaleTargetReach()
        {
            if( scale.counter >= targetScale)
            {
                successRoundsCount = 6;
                sucess();
            }
        }

        private void sucess() { }

        public void failure() { }
    }
}
