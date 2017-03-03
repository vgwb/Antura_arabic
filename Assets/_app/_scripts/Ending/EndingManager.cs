using System.Collections;
using EA4S.Animation;
using EA4S.Audio;
using EA4S.CameraEffects;
using EA4S.Core;
using EA4S.MinigamesCommon;
using EA4S.UI;
using UnityEngine;
using EA4S.LivingLetters;
using EA4S.Helpers;
using EA4S.MinigamesAPI;

namespace EA4S.Intro
{
    /// <summary>
    /// Manages the Intro scene, which shows a non-interactive introduction to the game.
    /// </summary>
    public class EndingManager : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        public LetterObjectView[] Letters;
        public Antura.AnturaAnimationController Antura;

        public float m_StateDelay = 1.0f;
        public float m_EndDelay = 2.0f;
        const float FadeTime = 5;

        bool m_Start = true;
        bool m_End = false;

        Vector3 m_CameraStartPosition;
        Vector3 m_CameraEndPosition;
        public Vector3 cameraOffset = new Vector3(0, 5.0f, -10.0f);
        public float m_CameraVelocity = 0.1f;

        public AnimationCurve cameraAnimationCurve;
        public AnimationCurve fadeOutCurve;
        public VignettingSimple vignetting;

        public GameObject environment;
        AutoMove[] autoMoveObjects;

        float time;
        float fadeOutTime;

        bool fadeIn = true;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            AudioManager.I.PlayMusic(SceneMusic);
            
            m_CameraEndPosition = Camera.main.transform.position;
            m_CameraStartPosition = m_CameraEndPosition + cameraOffset;
            autoMoveObjects = environment.GetComponentsInChildren<AutoMove>();

            var lettersData = AppManager.I.Teacher.GetAllTestLetterDataLL();
            foreach (var l in Letters)
            {
                l.Initialize(lettersData.GetRandom());
                l.State = LLAnimationStates.LL_dancing;
            }

            Antura.State = AnturaAnimationStates.dancing;
        }

        void OnEnable()
        {
            Debugging.DebugManager.OnSkipCurrentScene += SkipScene;
        }

        void OnDisable()
        {
            Debugging.DebugManager.OnSkipCurrentScene -= SkipScene;
        }

        void SkipScene()
        {
            StopCoroutine(DoEnding());
            KeeperManager.I.StopDialog();
            AppManager.I.NavigationManager.GoToNextScene();
        }

        void Update()
        {
            time += Time.deltaTime * m_CameraVelocity;
            float t = cameraAnimationCurve.Evaluate(time);

            if (fadeIn)
            {
                vignetting.fadeOut = Mathf.Pow((1 - t), 2);
            }
            else
            {
                fadeOutTime += Time.deltaTime;
                vignetting.fadeOut = Mathf.Lerp(0, 1, fadeOutTime/FadeTime);
            }

            for (int i = 0; i < autoMoveObjects.Length; ++i)
                autoMoveObjects[i].SetTime(t);

            if (m_Start)
            {
                m_Start = false;
                Debug.Log("Start Introduction");
               
                StartCoroutine(DoEnding());
            }
            else
            {
                if (m_End)
                {
                    AppManager.I.NavigationManager.GoToNextScene();
                    m_End = false;
                    return;
                }
            }

            Camera.main.transform.position = Vector3.Lerp(m_CameraStartPosition, m_CameraEndPosition, t);
        }


        IEnumerator DoEnding()
        {
            bool completed = false;
            System.Func<bool> CheckIfCompleted = () => {
                if (completed)
                {
                    // Reset it
                    completed = false;
                    AppManager.I.Player.SetFinalShowed();
                    return true;
                }
                return false;
            };

            System.Action OnCompleted = () => { completed = true; };

            yield return new WaitForSeconds(3);

            KeeperManager.I.PlayDialog(Database.LocalizationDataId.Map_EndGame, true, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(m_StateDelay);


            KeeperManager.I.PlayDialog(Database.LocalizationDataId.Assessment_Complete_2, true, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(3);

            fadeIn = false;
            yield return new WaitForSeconds(FadeTime);
            m_End = true;

        }
    }
}