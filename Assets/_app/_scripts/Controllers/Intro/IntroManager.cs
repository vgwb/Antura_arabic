using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class IntroManager : MonoBehaviour
    {
        public IntroFactory factory;

        CountdownTimer countDown;

        public float m_StateDelay = 1.0f;
        public float m_EndDelay = 2.0f;

        bool m_Start = true;
        bool m_End = false;

        Vector3 m_CameraStartPosition;
        Vector3 m_CameraPath;
        public float m_CameraHeightAtStart = 30.0f;
        public float m_CameraVelocity = 0.1f;

        public IntroMazeCharacter[] m_MazeCharacters;
        public float m_MazeCharactesVelocity = 0.1f;

        void Start() {
            GlobalUI.ShowPauseMenu(false);
            countDown = new CountdownTimer(m_EndDelay);
            m_CameraStartPosition = Camera.main.transform.position;
            Camera.main.transform.position += Vector3.up * m_CameraHeightAtStart;
            m_CameraPath = m_CameraStartPosition - Camera.main.transform.position;

            foreach (var mazeCharacter in m_MazeCharacters)
            {
                mazeCharacter.transform.position += new Vector3(0, m_CameraHeightAtStart - 10f, 0);
                mazeCharacter.m_Velocity = m_MazeCharactesVelocity;
            }          
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
                foreach (var mazeCharacter in m_MazeCharacters)
                {
                    mazeCharacter.SetDestination();
                }
                StartCoroutine(ChangeIntroductionState(Db.LocalizationDataId.Intro_welcome, FirstIntroLetter));
            }
            else
            {
                if (m_End)
                {
                    countDown.Update(Time.deltaTime);
                }
            }

            if (Camera.main.transform.position.y > m_CameraStartPosition.y)
            {
                Camera.main.transform.position += m_CameraPath * Time.deltaTime * m_CameraVelocity;
            }
                     
        }

        public void FirstIntroLetter()
        {
            Debug.Log("Start Spawning");
            factory.StartSpawning = true;
            StartCoroutine(ChangeIntroductionState(Db.LocalizationDataId.Intro_Letters_1, SecondIntroLetter));
        }

        public void SecondIntroLetter()
        {
            Debug.Log("Second Intro Letter");
            StartCoroutine(ChangeIntroductionState(Db.LocalizationDataId.Intro_Letters_2, EnableAntura));
        }

        public void EnableAntura()
        {
            factory.antura.SetAnturaTime(true);
            Debug.Log("Antura is enable");
            StartCoroutine(ChangeIntroductionState(Db.LocalizationDataId.Intro_Dog, EndIntroduction));
        } 

        public void EndIntroduction()
        {
            Debug.Log("EndIntroduction");
            StartCoroutine(ChangeIntroductionState(Db.LocalizationDataId.Map_End_LB_2, DisableAntura));
            StartCoroutine(ChangeIntroductionState(Db.LocalizationDataId.Intro_Dog_Chase, DisableAntura));
        }

        public void DisableAntura()
        {
            factory.antura.SetAnturaTime(false);
            countDown.Start();
            countDown.onTimesUp += CountDown_onTimesUp;
            m_End = true;
        }


        IEnumerator ChangeIntroductionState(Db.LocalizationDataId audioI, System.Action nextState)
        {
            Debug.Log("Start Coroutine");
            yield return new WaitForSeconds(m_StateDelay);
            WidgetSubtitles.I.DisplaySentence(audioI, 2, true, nextState);
        }
    }    
}