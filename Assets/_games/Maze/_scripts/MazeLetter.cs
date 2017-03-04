using UnityEngine;

namespace EA4S.Minigames.Maze
{
    public class MazeLetter : MonoBehaviour
    {
        public MazeCharacter mazeCharacter;
        public bool isDrawing;
        public float idleSeconds = 0;
        public float anturaSeconds;
        private bool didAnturaBark = false;

        void Start()
        {
            anturaSeconds = 0;
            isDrawing = false;
            mazeCharacter.toggleVisibility(false);
            //character.gameObject.SetActive (false);
        }

        void Update()
        {
            if (MazeGameManager.instance.gameEnded)
            {
                return;
            }

            if (mazeCharacter.characterIsMoving)
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

                    if (anturaSeconds >= 8f && !didAnturaBark)
                    {
                        MazeConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.DogBarking);
                        didAnturaBark = true;
                    }

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

                mazeCharacter.calculateMovementAndRotation();
            }
        }

        public void OnPointerDown()
        {
            if (mazeCharacter.characterIsMoving || !mazeCharacter.canMouseBeDown() || mazeCharacter.finishedRound)
            {
                return;
            }

            Debug.Log("started Drawing!");

            MazeGameManager.instance.drawingTool.SetActive(true);

            idleSeconds = 0;
            MazeGameManager.instance.currentTutorial.stopCurrentTutorial();
            anturaSeconds = 0;

            mazeCharacter.ChangeStartingFXHighlight();

            // Inform that we are inside the collision:
            isDrawing = true;
        }

        public void OnPointerUp()
        {
            if (CanLaunchRocket())
            {
                MazeGameManager.instance.drawingTool.SetActive(false);
                LaunchRocket();
            }
        }

        public void OnPointerOverTrackBounds(Vector3 pointOfImpact)
        {
            if (CanLaunchRocket())
            {
                mazeCharacter.loseState = MazeCharacter.LoseState.OutOfBounds;

                MazeGameManager.instance.ColorCurrentLinesAsIncorrect();

                Tutorial.TutorialUI.MarkNo(pointOfImpact);
                MazeConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.KO);

                if (!MazeGameManager.instance.isTutorialMode)
                {
                    MazeConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Lose);
                }

                LaunchRocket();
            }
        }

        private bool CanLaunchRocket()
        {
            return MazeGameManager.instance.tutorialForLetterisComplete() && isDrawing;
        }

        private void LaunchRocket()
        {
            isDrawing = false;
            mazeCharacter.toggleVisibility(true);
            mazeCharacter.initMovement();

            

            MazeGameManager.instance.timer.StopTimer();
        }

        public void NotifyFruitGotMouseOver(MazeArrow fruit)
        {
            if (isDrawing && fruit.gameObject != mazeCharacter._fruits[0])
            {
                fruit.HighlightAsReached();
            }
        }

        public void NotifyDrawnLetterWrongly()
        {
            if (CanLaunchRocket())
            {
                LaunchRocket();
            }
        }
    }
}