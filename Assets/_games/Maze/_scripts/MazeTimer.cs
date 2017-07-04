using EA4S.UI;
using UnityEngine;
using TMPro;

namespace EA4S.Minigames.Maze
{
    public class MazeTimer : MonoBehaviour
    {

        [HideInInspector]
        public float time;
        public TextMeshProUGUI timerText;

        private bool isRunning;
        private bool playedSfx;
        private float timeRemaining;


        public void initTimer()
        {
            /*time = MazeGameManager.Instance.gameTime;
			timeRemaining = time;
			DisplayTime ();*/

            // this.StopAllCoroutines();
            if (!MazeGame.instance.isTutorialMode)
                MinigamesUI.Timer.Setup(MazeGame.instance.gameTime);


        }

        public void Update()
        {

            if (!MazeGame.instance.isTutorialMode &&
                MinigamesUI.Timer != null &&
                MinigamesUI.Timer.Duration == MinigamesUI.Timer.Elapsed)
            {
                StopTimer();
                MazeGame.instance.onTimeUp();
            }

            /*if (isRunning) {
				if (timeRemaining > 0f) {
					timeRemaining -= Time.deltaTime;
					DisplayTime();
				}
				if (!playedSfx && timeRemaining < 5f) {
					AudioManager.I.PlaySound(Sfx.DangerClockLong);
					playedSfx = true;
				}
				if (timeRemaining < 1f) {
					StopTimer();
					MazeGameManager.Instance.onTimeUp();
				}
			}*/

        }

        public void StartTimer()
        {
            //isRunning = true;
            //   this.StopAllCoroutines();
            if (!MazeGame.instance.isTutorialMode)
                MinigamesUI.Timer.Play();

        }

        public void StopTimer()
        {
            // this.StopAllCoroutines();
            if (!MazeGame.instance.isTutorialMode)
                MinigamesUI.Timer.Pause();
            /*isRunning = false;
			playedSfx = false;
			AudioManager.I.StopSfx(Sfx.DangerClockLong);*/
        }



        public void DisplayTime()
        {
            /*var text = Mathf.Floor(timeRemaining).ToString();
			timerText.text = text;*/
        }
    }
}