using UnityEngine;

namespace EA4S
{
    // Sample Intro Manager
    public class IntroManager : MonoBehaviour
    {
        public LetterCrowd crowd;
        float lettersTimer = 2;
        float anturaEnterTimer = 5;
        float anturaExitTimer = 10;
        CountdownTimer countDown = new CountdownTimer(5);

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            countDown.Start();
            countDown.onTimesUp += CountDown_onTimesUp;
        }

        private void CountDown_onTimesUp() {
            AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_AnturaSpace");
        }

        void OnDisable() {
            countDown.onTimesUp -= CountDown_onTimesUp;
        }

        void Update()
        {
            countDown.Update(Time.deltaTime);
            if (lettersTimer > 0)
            {
                lettersTimer -= Time.deltaTime;

                if (lettersTimer <= 0)
                {
                    crowd.AddLivingLetter(null);
                    crowd.AddLivingLetter(null);
                    crowd.AddLivingLetter(null);
                }
            }

            if (anturaEnterTimer > 0)
            {
                anturaEnterTimer -= Time.deltaTime;

                if (anturaEnterTimer <= 0)
                {
                    crowd.antura.SetAnturaTime(true);
                }
            }
            else if (anturaExitTimer > 0)
            {
                anturaExitTimer -= Time.deltaTime;

                if (anturaExitTimer <= 0)
                {
                    crowd.antura.SetAnturaTime(false);
                }
            }
        }
    }
}