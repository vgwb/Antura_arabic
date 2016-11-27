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
        bool m_TimedUp = false;

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
            AppManager.I.Modules.SceneModule.LoadSceneWithTransition("app_Map");
        }

        void OnDisable()
        {
            if (countDown != null)
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
                StartCoroutine(DoIntroduction());
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

        public void DisableAntura()
        {
            factory.antura.SetAnturaTime(false);
            countDown.Start();
            countDown.onTimesUp += CountDown_onTimesUp;
        }
        

        IEnumerator DoIntroduction()
        {
            bool completed = false;
            System.Func<bool> CheckIfCompleted = () => 
            {
                if (completed)
                {
                    // Reset it
                    completed = false;
                    return true;
                }
                return false;
            };

            System.Action OnCompleted = () => { completed = true; };

            yield return new WaitForSeconds(m_StateDelay);

            KeeperManager.I.PlayDialog(Db.LocalizationDataId.Intro_welcome, true, OnCompleted);
            
            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(m_StateDelay);

            Debug.Log("Start Spawning");
            factory.StartSpawning = true;
            
            KeeperManager.I.PlayDialog(Db.LocalizationDataId.Intro_Letters_1, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(m_StateDelay);

            Debug.Log("Second Intro Letter");
            KeeperManager.I.PlayDialog(Db.LocalizationDataId.Intro_Letters_2, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(m_StateDelay);

            factory.antura.SetAnturaTime(true);
            Debug.Log("Antura is enable");
            KeeperManager.I.PlayDialog(Db.LocalizationDataId.Intro_Dog, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(m_StateDelay);

            DisableAntura();

            KeeperManager.I.PlayDialog(Db.LocalizationDataId.Intro_Dog_Chase, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);

            m_End = true;

        }
    }    
}