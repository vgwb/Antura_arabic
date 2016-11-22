using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class IntroManager : MonoBehaviour
    {
        public IntroFactory factory;

        CountdownTimer countDown;

        public float m_StateDelay = 2.0f;
        public float m_EndDelay = 2.0f;

        bool m_Start = true;
        bool m_End = false;

        void Start() {
            GlobalUI.ShowPauseMenu(false);
            countDown = new CountdownTimer(m_EndDelay);
        }

        private void CountDown_onTimesUp() {
            AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Map");
        }

        void OnDisable() {
            countDown.onTimesUp -= CountDown_onTimesUp;
        }

        void Update()
        {
            if (m_Start)
            {
                m_Start = false;
                Debug.Log("Start Introduction");
                StartCoroutine(ChangeIntroductionState("end_learningblock_A2", FirstIntroLetter));
                //StartCoroutine(ChangeIntroductionState("Intro_welcome", FirstIntroLetter));
            }

            if (m_End)
            {
                countDown.Update(Time.deltaTime);
            }
                      
        }

        public void FirstIntroLetter()
        {
            Debug.Log("Start Spawning");
            factory.StartSpawning = true;
            StartCoroutine(ChangeIntroductionState("end_learningblock_A2", SecondIntroLetter));
            //StartCoroutine(ChangeIntroductionState("Intro_Letters_1", SecondIntroLetter));
        }

        public void SecondIntroLetter()
        {
            Debug.Log("Second Intro Letter");
            StartCoroutine(ChangeIntroductionState("end_learningblock_A2", EnableAntura));
            //StartCoroutine(ChangeIntroductionState("Intro_Letters_2", EnableAntura));
        }

        public void EnableAntura()
        {
            factory.antura.SetAnturaTime(true);
            Debug.Log("Antura is enable");
            StartCoroutine(ChangeIntroductionState("end_learningblock_A2", EndIntroduction));
            //StartCoroutine(ChangeIntroductionState("Intro_Dog", EndIntroduction));
        } 

        public void EndIntroduction()
        {
            Debug.Log("EndIntroduction");
            StartCoroutine(ChangeIntroductionState("end_learningblock_A2", DisableAntura));
            //StartCoroutine(ChangeIntroductionState("Intro_Dog_Chase", DisableAntura));
        }

        public void DisableAntura()
        {
            factory.antura.SetAnturaTime(false);
            countDown.Start();
            countDown.onTimesUp += CountDown_onTimesUp;
            m_End = true;
        }


        IEnumerator ChangeIntroductionState(string audioI, System.Action nextState)
        {
            Debug.Log("Start Coroutine");
            yield return new WaitForSeconds(m_StateDelay);
            WidgetSubtitles.I.DisplaySentence(audioI, 2, true, nextState);
        }
    }    
}