using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using EA4S;
using ModularFramework.Core;
using ModularFramework.Modules;
using ModularFramework.Helpers;
using Google2u;

namespace EA4S.DontWakeUp
{
    public enum How2Die {
        Null,
        TouchedDog,
        TouchedAlarm,
        TooFast,
        TooSlow,
        Fall
    }

    public class DontWakeUpManager : MiniGameBase
    {
        [Header("Scene Setup")]
        public Music SceneMusic;
 
        [Header("Test / Debug")]
        public int StartingLevel;

        [Header("References")]
        public GameObject[] Levels;
        public DangerMeter dangering;
        public GameObject myLetter;
        public GameObject StarSystems;
        public LivesContainer LivesController;
        public Antura Antura;
        public GameObject CameraWakeUpAntura;

        [Header("Images")]
        public Sprite FailTouchedDog;
        public Sprite FailTouchedAlarm;
        public Sprite FailFall;

        [HideInInspector]
        public WordData currentWord;

        [HideInInspector]
        public MinigameState currentState;

        [Header("Game Vars")]   
        public int currentRound;

        int LivesLeft;
        int RoundsTotal;
        int currentLevel;
        LevelController currentLevelController;
        float dangeringSpeed = 1.0f;
        bool inDanger;
        float dangerIntensity;
        How2Die dangerCause;
        int TutorialIndex;
        bool dangerAlertShown;

        [Header("Gameplay Info and Config section")]
        #region Overrides
        new public GameDontWakeUpGameplayInfo GameplayInfo;

        new public static DontWakeUpManager Instance
        {
            get { return SubGame.Instance as DontWakeUpManager; }
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            currentState = MinigameState.Initializing;
            RoundsTotal = Levels.Length;
            currentRound = StartingLevel;
            currentLevel = currentRound;
            LivesLeft = 4;
            AppManager.Instance.InitDataAI();
            AppManager.Instance.CurrentGameManagerGO = gameObject;

            LoggerEA4S.Log("minigame", "dontwakeup", "start", "");
            LoggerEA4S.Save();

            ResetAntura();

            AudioManager.I.PlayMusic(SceneMusic);
            AudioManager.I.PlaySfx(Sfx.DogSnoring);

            SceneTransitioner.Close();

            //StartCurrentRound();
            currentState = MinigameState.GameIntro;
            Invoke("GameIntro", 2);
        }

        public void DoPause(bool status)
        {
            Debug.Log("GameDontWakeUp DoPause() " + status);
            if (currentState == MinigameState.Playing) {
                currentState = MinigameState.Paused;
            } else if (currentState == MinigameState.Paused) {
                currentState = MinigameState.Playing;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopSceneSounds();
        }

        public void CloseScene()
        {
            StopSceneSounds();
        }

        void StopSceneSounds()
        {
            AudioManager.I.StopSfx(Sfx.DangerClock);
            AudioManager.I.StopSfx(Sfx.DogSnoring);
        }

        void GameIntro()
        {
            StartCurrentRound();
        }

        //        public void GameIntroFinished()
        //        {
        //            WidgetSubtitles.I.Close();
        //            //WidgetPopupWindow.Show(false);
        //            StartCurrentRound();
        //        }
        //
        //        private void ShowTutorialLine()
        //        {
        //            switch (TutorialIndex) {
        //                case 3:
        //                    WidgetSubtitles.I.DisplaySentence("game_dontwake_intro1");
        //                    break;
        //                case 2:
        //                    WidgetSubtitles.I.DisplaySentence("game_dontwake_intro2");
        //                    break;
        //                case 1:
        //                    WidgetSubtitles.I.DisplaySentence("game_dontwake_intro3");
        //                    break;
        //            }
        //        }

        public void ClickedNext()
        {
            //Debug.Log("ClickedNext()");
            switch (currentState) {
//                case MinigameState.GameIntro:
//                    if (TutorialIndex > 1) {
//                        TutorialIndex = TutorialIndex - 1;
//                        ShowTutorialLine();
//                    } else {
//                        GameIntroFinished();
//                    }
//                    break;
                case MinigameState.RoundIntro:
                    WidgetSubtitles.I.Close();
                    WidgetPopupWindow.Show(false);
                    currentState = MinigameState.Playing;
                    break;
            }
        }

        public void InitRound()
        {
            currentState = MinigameState.RoundIntro;

            UpdateLivesContainer();
            SetupLevel();

            WidgetPopupWindow.I.ShowStringAndWord(ClickedNext, currentRound.ToString(), currentWord);
            SpeakCurrentLetter();
        }

        public void SpeakCurrentLetter()
        {
            AudioManager.I.PlayWord(currentWord.Key);
        }

        void SetupLevel()
        {
            currentLevelController = Levels[currentLevel - 1].GetComponent<LevelController>();
            currentWord = AppManager.Instance.Teacher.GimmeAGoodWordData();

            myLetter.SetActive(true);
            myLetter.GetComponent<MyLetter>().Init(currentWord.Key);
            myLetter.transform.position = currentLevelController.GetStartPosition().position;
            myLetter.transform.eulerAngles = new Vector3(0, currentLevelController.GetStartPosition().rotation.eulerAngles.y, 0);

            DangerDog.I.Hide();

            LoggerEA4S.Log("minigame", "dontwakeup", "newWord", currentWord.Word);

            currentLevelController.SetWord();
            ChangeCamera(false);
        }

        public void RoundLost(How2Die how)
        {
            if (currentState != MinigameState.RoundEnd) {
                currentState = MinigameState.RoundEnd;
                resetDanger();
                myLetter.SetActive(false);

                AudioManager.I.StopSfx(Sfx.DangerClock);
                AudioManager.I.PlaySfx(Sfx.Lose);

                if (LivesLeft > 0) {
                    LivesLeft = LivesLeft - 1;
                    UpdateLivesContainer();
                }

                switch (how) {
                    case How2Die.TouchedAlarm:
                        currentLevelController.DoAlarmEverything();
                        WidgetPopupWindow.I.ShowSentenceWithMark(RoundLostEnded, "game_dontwake_fail_alarms", false, FailTouchedAlarm);
                        break;
                    case How2Die.TouchedDog:
                        AnturaHasWokenUp();
                        break;
                    case How2Die.TooFast:
                        WidgetPopupWindow.I.ShowSentenceWithMark(RoundLostEnded, "game_dontwake_fail_toofast", false, null);
                        break;
                    case How2Die.Fall:
                        WidgetPopupWindow.I.ShowSentenceWithMark(RoundLostEnded, "game_dontwake_fail_fall", false, FailFall);
                        break;
                }
            }
        }

        void ResetAntura()
        {
            Antura.SetAnimation(AnturaAnim.DontWakeSleeping);
            Antura.IsBarking = false;
        }

        void AnturaHasWokenUp()
        {
            currentLevelController.StopSnoozing();
            Antura.SetAnimation(AnturaAnim.DontWakeWakesUp);
            Antura.IsBarking = true;
            CameraGameplayController.I.MoveToPosition(CameraWakeUpAntura.transform.position, CameraWakeUpAntura.transform.rotation);
            Invoke("AnturaHasWokenUpFinished", 4);
        }

        void AnturaHasWokenUpFinished()
        {
            WidgetPopupWindow.I.ShowSentenceWithMark(RoundLostEnded, "game_dontwake_fail_antura", false, FailTouchedDog);
        }


        void RoundLostEnded()
        {
            if (LivesLeft > 0) {
                ResetAntura();
                currentLevelController.DoAlarmOff();
                WidgetSubtitles.I.DisplaySentence("");
                InitRound();

            } else {
                GameLost();
            }
        }

        public void RoundWon()
        {
            currentState = MinigameState.Paused;
            myLetter.SetActive(false);
            LoggerEA4S.Log("minigame", "dontwakeup", "wordFinished", "");
            if (currentRound < RoundsTotal) {
                GoToNextRound();
            } else {
                GameWon();
            }
        }

        public void GameWon()
        {
            currentState = MinigameState.GameEnd;
            StopSceneSounds();
            AudioManager.I.PlaySfx(Sfx.Win);

            if (LivesLeft >= 3) {
                LivesLeft = 3;
            }

            StartCoroutine(EndGame_ShowResults(LivesLeft));
        }

        public void GameLost()
        {
            WidgetPopupWindow.Close();
            currentState = MinigameState.GameEnd;
            StopSceneSounds();

            StartCoroutine(EndGame_ShowResults(0));
        }


        IEnumerator EndGame_ShowResults(int score)
        {
            yield return new WaitForSeconds(1f);

            if (score <= 0) {
                WidgetSubtitles.I.DisplaySentence("game_result_retry");
            } else if (score == 1) {
                WidgetSubtitles.I.DisplaySentence("game_result_fair");
            } else if (score == 2) {
                WidgetSubtitles.I.DisplaySentence("game_result_good");
            } else {
                WidgetSubtitles.I.DisplaySentence("game_result_great");
            }

            if (score > 0) {
                LoggerEA4S.Log("minigame", "dontwakeup", "Won", score.ToString());
            } else {
                LoggerEA4S.Log("minigame", "dontwakeup", "Lost", "");
            }
            LoggerEA4S.Save();

            StarSystems.SetActive(true);
            StarSystems.GetComponent<StarFlowers>().Show(score);
        }

        void StartCurrentRound()
        {
            Debug.Log("StartCurrentRound " + currentRound);
            currentLevel = currentRound;
            Levels[currentLevel - 1].SetActive(true);
            currentLevelController = Levels[currentLevel - 1].GetComponent<LevelController>();
            currentLevelController.Init();
            ChangeCamera(true);
        }

        void GoToNextRound()
        {
            if (currentLevel > 0)
                Levels[currentLevel - 1].SetActive(false);
            currentRound = currentRound + 1;
            StartCurrentRound();
        }
            
        // called by callback in camera
        public void CameraReady()
        {
            if (currentState != MinigameState.RoundEnd) {
                InitRound();
            }
        }

        public void ChangeCamera(bool animated)
        {
            if (animated) {
                CameraGameplayController.I.MoveToPosition(currentLevelController.LevelCamera.transform.position, currentLevelController.LevelCamera.transform.rotation);
            } else {
                CameraGameplayController.I.SetToPosition(currentLevelController.LevelCamera.transform.position, currentLevelController.LevelCamera.transform.rotation);
            }
        }

        void UpdateLivesContainer()
        {
            LivesController.SetLives(LivesLeft);
        }


        void Update()
        {
            if (DontWakeUpManager.Instance.currentState == MinigameState.Playing) {
                if (inDanger) {
                    if (dangerCause == How2Die.TooFast) {
                        // toofast danger speed in faster!
                        dangerIntensity = dangerIntensity + dangeringSpeed * 1 * Time.deltaTime;
                    } else {
                        dangerIntensity = dangerIntensity + dangeringSpeed * Time.deltaTime;
                    }

                    if (dangerIntensity < 0.3f) {
                        dangerAlertShown = false;
                    }

                    if (dangerIntensity > 0.4f && !dangerAlertShown) {
                        dangerAlertShown = true;
                        switch (dangerCause) {
                            case How2Die.TooFast:
                                WidgetSubtitles.I.DisplaySentence("game_dontwake_fail_toofast", 1, true, CloseDisplayAlert);
                                break;
                            case How2Die.TouchedAlarm:
                                WidgetSubtitles.I.DisplaySentence("game_dontwake_fail_alarms", 1, true, CloseDisplayAlert);
                                break;
                            case How2Die.TouchedDog:
                                WidgetSubtitles.I.DisplaySentence("game_dontwake_fail_antura", 1, true, CloseDisplayAlert);
                                break;
                        }
                    }

                    if (dangerIntensity > 1f) {
                        dangerIntensity = 1f;
                        RoundLost(dangerCause);
                    }

                } else {
                    dangerIntensity = dangerIntensity - dangeringSpeed * Time.deltaTime;
                    if (dangerIntensity < 0)
                        dangerIntensity = 0;
                }

                dangering.SetIntensity(dangerIntensity);
            }
        }

        public void CloseDisplayAlert()
        {
            WidgetSubtitles.I.Close();
        }

        public void InDanger(bool status, How2Die cause)
        {
            inDanger = status;
            dangerCause = cause;
            if (inDanger) {
                AudioManager.I.PlaySfx(Sfx.DangerClock);
            } else {
                AudioManager.I.StopSfx(Sfx.DangerClock);
            }
        }

        void resetDanger()
        {
            DangerDog.I.Hide();
            InDanger(false, How2Die.Null);
            dangerIntensity = 0;
        }

    }

    [Serializable]
    public class GameDontWakeUpGameplayInfo : AnturaGameplayInfo
    {
        [Tooltip("Play session duration in seconds.")]
        public float PlayTime = 10;
    }


}