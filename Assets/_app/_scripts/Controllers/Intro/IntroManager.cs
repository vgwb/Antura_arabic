using UnityEngine;

namespace EA4S
{
    public class IntroManager : MonoBehaviour
    {
        public IntroFactory factory;
        public float lettersTimer = 1;
        public float anturaEnterTimer = 1.5f;
        public float anturaExitTimer = 5;

        CountdownTimer countDown;

        bool m_Start = true;
        bool m_End = false;

        void Start() {
            GlobalUI.ShowPauseMenu(false);
            countDown = new CountdownTimer(2);
            //countDown.Start();
            //countDown.onTimesUp += CountDown_onTimesUp;
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
                WidgetSubtitles.I.DisplaySentence("Intro_welcome", 2, true, WelcomeEnd);
                //WidgetSubtitles.I.DisplaySentence("end_learningblock_A2", 2, true, WelcomeEnd);
            }

            if (m_End)
            {
                countDown.Update(Time.deltaTime);
            }
            

            //if (lettersTimer > 0)
            //{
            //    lettersTimer -= Time.deltaTime;

            //    if (lettersTimer <= 0)
            //    {
            //        crowd.AddLivingLetter(null);
            //        crowd.AddLivingLetter(null);
            //        crowd.AddLivingLetter(null);
            //        factory.StartSpawning = true;
            //    }
            //}

            //if (anturaEnterTimer > 0)
            //{
            //    anturaEnterTimer -= Time.deltaTime;

            //    if (anturaEnterTimer <= 0)
            //    {
            //        factory.antura.SetAnturaTime(true);
            //    }
            //}
            //else if (anturaExitTimer > 0)
            //{
            //    anturaExitTimer -= Time.deltaTime;

            //    if (anturaExitTimer <= 0)
            //    {
            //        factory.antura.SetAnturaTime(false);
            //    }
            //}
        }

        public void WelcomeEnd()
        {
            factory.StartSpawning = true;
            Debug.Log("Start Spawing");
            WidgetSubtitles.I.DisplaySentence("Intro_Letters_1", 2, true, IntroLetterEnd);
            //WidgetSubtitles.I.DisplaySentence("end_learningblock_A2", 2, true, IntroLetterEnd);
        }

        public void IntroLetterEnd()
        {
            Debug.Log("First Intro Letter is Concluded");
            WidgetSubtitles.I.DisplaySentence("Intro_Letters_2", 2, true, EnableAntura);
            //WidgetSubtitles.I.DisplaySentence("end_learningblock_A2", 2, true, EnableAntura);
        }

        public void EnableAntura()
        {
            factory.antura.SetAnturaTime(true);
            Debug.Log("Antara is enable");
            WidgetSubtitles.I.DisplaySentence("Intro_Dog", 2, true, AnturaEnd);
            //WidgetSubtitles.I.DisplaySentence("end_learningblock_A2", 2, true, AnturaEnd);
        } 

        public void AnturaEnd()
        {
            Debug.Log("EndIntroduction");
            WidgetSubtitles.I.DisplaySentence("Intro_Dog_Chase", 2, true);
            //WidgetSubtitles.I.DisplaySentence("end_learningblock_A2", 2, true, EndIntroduction);
        }

        public void EndIntroduction()
        {
            factory.antura.SetAnturaTime(false);
            countDown.Start();
            countDown.onTimesUp += CountDown_onTimesUp;
            m_End = true;
        }

    }    
}