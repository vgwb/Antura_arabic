using UnityEngine;

namespace EA4S.Minigames.Maze
{
    public class MazeLetter : MonoBehaviour
    {
        public MazeCharacter LLOnRocket;
        public bool isDrawing;
        public float idleSeconds = 0;
        public float anturaSeconds;
        
        void Start()
        {
            anturaSeconds = 0;
            isDrawing = false;
            LLOnRocket.toggleVisibility(false);
            //character.gameObject.SetActive (false);
        }
        
        void Update()
        {

            if (LLOnRocket.characterIsMoving)
            {
                anturaSeconds = 0;
                return;
            }

            //should we replay tutorial?
            if (!isDrawing)
            {

                if (!MazeGameManager.instance.currentCharacter || MazeGameManager.instance.currentCharacter.isFleeing || MazeGameManager.instance.currentCharacter.isAppearing)
                    return;

                if (!MazeGameManager.instance.isTutorialMode && MazeGameManager.instance.currentTutorial && MazeGameManager.instance.currentTutorial.isShownOnce && MazeGameManager.instance.isShowingAntura == false)
                {
                    anturaSeconds += Time.deltaTime;

                    if (anturaSeconds >= 10)
                    {
                        anturaSeconds = 0;
                        MazeGameManager.instance.onIdleTime();
                    }

                }


                if (MazeGameManager.instance.currentTutorial != null &&
                    MazeGameManager.instance.currentTutorial.isStopped == false &&
                    MazeGameManager.instance.currentTutorial.isShownOnce == true)
                {

                    idleSeconds += Time.deltaTime;

                    if (idleSeconds >= 5)
                    {
                        idleSeconds = 0;
                        MazeGameManager.instance.currentTutorial.showCurrentTutorial();
                    }
                }
            }


            if (isDrawing)
            {
                anturaSeconds = 0;

                LLOnRocket.calculateMovementAndRotation();
            }
        }

        public void OnPointerDown()
        {
            if (LLOnRocket.characterIsMoving || !LLOnRocket.canMouseBeDown())
            {
                return;
            }

            Debug.Log("started Drawing!");

            idleSeconds = 0;
            MazeGameManager.instance.currentTutorial.stopCurrentTutorial();
            anturaSeconds = 0;

            // Inform that we are inside the collision:
            isDrawing = true;
        }

        public void OnPointerUp()
        {
            if (CanLaunchRocket())
            {
                LaunchRocket();
            }
        }

        public void OnPointerOverTrackBounds()
        {
            if (CanLaunchRocket())
            {
                LaunchRocket();
                MazeGameManager.instance.ColorCurrentLinesAsIncorrect();
            }
        }

        private bool CanLaunchRocket()
        {
            return MazeGameManager.instance.tutorialForLetterisComplete() && isDrawing;
        }

        private void LaunchRocket()
        {
            isDrawing = false;
            LLOnRocket.toggleVisibility(true);
            LLOnRocket.initMovement();

            MazeGameManager.instance.timer.StopTimer();
        }

        public void NotifyFruitGotMouseOver(MazeArrow fruit)
        {
            if (isDrawing)
            {
                fruit.Highlight();
            }
        }
    }
}