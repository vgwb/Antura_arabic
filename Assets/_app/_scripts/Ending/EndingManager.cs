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
        bool showText = false;
        float lastAlpha = 0;

        public TextRender text;
        public CanvasRenderer panel;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            AudioManager.I.PlayMusic(SceneMusic);
            
            m_CameraEndPosition = Camera.main.transform.position;
            m_CameraStartPosition = m_CameraEndPosition + cameraOffset;
            autoMoveObjects = environment.GetComponentsInChildren<AutoMove>();

            var lettersData = (AppManager.Instance as AppManager).Teacher.GetAllTestLetterDataLL();
            foreach (var l in Letters)
            {
                l.Initialize(lettersData.GetRandom());
                l.State = LLAnimationStates.LL_dancing;
            }

            Antura.State = AnturaAnimationStates.dancing;
            
            text.SetSentence(Database.LocalizationDataId.End_Scene_2);
            text.Alpha = 0;
            panel.SetAlpha(0);
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
            (AppManager.Instance as AppManager).NavigationManager.GoToNextScene();
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
                Debug.Log("Start Ending");
               
                StartCoroutine(DoEnding());
            }
            else
            {
                if (m_End)
                {
                    (AppManager.Instance as AppManager).NavigationManager.GoToNextScene();
                    m_End = false;
                    return;
                }
            }

            Camera.main.transform.position = Vector3.Lerp(m_CameraStartPosition, m_CameraEndPosition, t);

            var newAlpha = Mathf.Lerp(lastAlpha, showText ? 1 : 0, Time.deltaTime);

            if (lastAlpha != newAlpha)
            {
                text.Alpha = newAlpha;
                panel.SetAlpha(newAlpha);
                lastAlpha = newAlpha;
            }
        }


        IEnumerator DoEnding()
        {
            bool completed = false;
            System.Func<bool> CheckIfCompleted = () => {
                if (completed)
                {
                    // Reset it
                    completed = false;
                    return true;
                }
                return false;
            };

            System.Action OnCompleted = () => { completed = true; };

            yield return new WaitForSeconds(3);

            KeeperManager.I.PlayDialog(Database.LocalizationDataId.Map_EndGame, true, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(m_StateDelay);
            
            KeeperManager.I.PlayDialog(Database.LocalizationDataId.End_Scene_1_1, false, true, OnCompleted);
            yield return new WaitUntil(CheckIfCompleted);
            KeeperManager.I.PlayDialog(Database.LocalizationDataId.End_Scene_1_2, false, true, OnCompleted);

            yield return new WaitForSeconds(m_StateDelay);
            yield return new WaitUntil(CheckIfCompleted);

            // Show Text
            showText = true;

            yield return new WaitForSeconds(5);

            showText = false;

            KeeperManager.I.PlayDialog(Database.LocalizationDataId.End_Scene_3_1, false, true, OnCompleted);
            yield return new WaitUntil(CheckIfCompleted);
            KeeperManager.I.PlayDialog(Database.LocalizationDataId.End_Scene_3_2, false, true, OnCompleted);
            yield return new WaitUntil(CheckIfCompleted);
            KeeperManager.I.PlayDialog(Database.LocalizationDataId.End_Scene_3_3, false, true, OnCompleted);

            yield return new WaitUntil(CheckIfCompleted);
            yield return new WaitForSeconds(1);

            fadeIn = false;
            yield return new WaitForSeconds(FadeTime);
            m_End = true;

            (AppManager.Instance as AppManager).Player.SetFinalShown();
        }
    }
}