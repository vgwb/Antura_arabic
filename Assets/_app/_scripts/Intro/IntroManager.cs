using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class IntroManager : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        public IntroFactory factory;

        CountdownTimer countDown;

        public float m_StateDelay = 1.0f;
        public float m_EndDelay = 2.0f;

        bool m_Start = true;
        bool m_End = false;
        bool m_TimedUp = false;

        Vector3 m_CameraStartPosition;
        Vector3 m_CameraEndPosition;
        public Vector3 cameraOffset = new Vector3(0, 5.0f, -10.0f);
        public float m_CameraVelocity = 0.1f;

        public IntroMazeCharacter[] m_MazeCharacters;
        public float m_MazeCharactesVelocity = 0.1f;
        public AnimationCurve cameraAnimationCurve;
        //public UnityStandardAssets.ImageEffects.ForegroundCameraEffect foregroundEffect;
        public VignettingSimple vignetting;

        public GameObject environment;
        AutoMove[] autoMoveObjects;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            AudioManager.I.PlayMusic(SceneMusic);

            countDown = new CountdownTimer(m_EndDelay);
            m_CameraEndPosition = Camera.main.transform.position;
            m_CameraStartPosition = m_CameraEndPosition + cameraOffset;
            autoMoveObjects = environment.GetComponentsInChildren<AutoMove>();

            foreach (var mazeCharacter in m_MazeCharacters) {
                mazeCharacter.transform.position += new Vector3(0, 10f, 0);
                mazeCharacter.m_Velocity = m_MazeCharactesVelocity;
            }
        }

        private void CountDown_onTimesUp()
        {
            NavigationManager.I.GoToScene(AppScene.Map);
        }

        void OnDisable()
        {
            if (countDown != null)
                countDown.onTimesUp -= CountDown_onTimesUp;
        }

        float time;
        void Update()
        {
            time += Time.deltaTime * m_CameraVelocity;
            float t = cameraAnimationCurve.Evaluate(time);

            vignetting.fadeOut = Mathf.Pow((1 - t), 2);

            for (int i = 0; i < autoMoveObjects.Length; ++i)
                autoMoveObjects[i].SetTime(t);

            if (m_Start) {
                m_Start = false;
                Debug.Log("Start Introduction");
                foreach (var mazeCharacter in m_MazeCharacters) {
                    mazeCharacter.SetDestination();
                }
                StartCoroutine(DoIntroduction());
            } else {
                if (m_End) {
                    countDown.Update(Time.deltaTime);
                }
            }

            Camera.main.transform.position = Vector3.Lerp(m_CameraStartPosition, m_CameraEndPosition, t);
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
            System.Func<bool> CheckIfCompleted = () => {
                if (completed) {
                    // Reset it
                    completed = false;
                    return true;
                }
                return false;
            };

            System.Action OnCompleted = () => { completed = true; };

            yield return new WaitForSeconds(m_StateDelay);

            KeeperManager.I.PlayDialog(Db.LocalizationDataId.Intro_welcome, true, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(m_StateDelay);

            Debug.Log("Start Spawning");
            factory.StartSpawning = true;

            KeeperManager.I.PlayDialog(Db.LocalizationDataId.Intro_Letters_1, true, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(m_StateDelay);

            Debug.Log("Second Intro Letter");
            KeeperManager.I.PlayDialog(Db.LocalizationDataId.Intro_Letters_2, true, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(m_StateDelay);

            factory.antura.SetAnturaTime(true);
            Debug.Log("Antura is enable");
            KeeperManager.I.PlayDialog(Db.LocalizationDataId.Intro_Dog, true, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(m_StateDelay);

            DisableAntura();

            KeeperManager.I.PlayDialog(Db.LocalizationDataId.Intro_Dog_Chase, true, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);

            m_End = true;

        }
    }
}