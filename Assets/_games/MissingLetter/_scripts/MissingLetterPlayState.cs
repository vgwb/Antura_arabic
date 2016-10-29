using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace EA4S.MissingLetter
{
    public class MissingLetterPlayState : IGameState
    {
        

        public MissingLetterPlayState(MissingLetterGame game)
        {
            this.game = game;

            gameTime.onTimesUp += OnTimesUp;

            game.m_RoundManager.onAnswered += OnRoundResult;
        }

        public void EnterState()
        {
            game.mIsTimesUp = false;
            game.ResetScore();

            // Reset game timer
            gameTime.Reset();
            gameTime.Start();

            game.timerText.gameObject.SetActive(true);
            game.timerText.text = "";

            hurryUpSfx = false;

            game.Context.GetAudioManager().PlayMusic(Music.MainTheme);

            game.m_RoundManager.NewRound();
        }

        public void ExitState()
        {
            if (timesUpAudioSource != null)
                timesUpAudioSource.Stop();

            game.timerText.gameObject.SetActive(false);
            gameTime.Stop();
        }

        public void Update(float delta)
        {
            if (Input.GetButtonDown("Jump"))
            {
                float duration = 7;
                game.mAnturaRef.GetComponent<AnturaBehaviour>().EnterScene(duration);
                game.StartCoroutine(Utils.LaunchDelay(duration / 6, game.m_RoundManager.ShuffleLetters, duration / 2));
            }

            game.timerText.text = String.Format("{0:0}", gameTime.Time);

            if (!hurryUpSfx)
            {
                if (gameTime.Time < 4f)
                {
                    hurryUpSfx = true;

                    timesUpAudioSource = game.Context.GetAudioManager().PlaySound(Sfx.DangerClockLong);
                }
            }

            gameTime.Update(delta);
        }

        public void UpdatePhysics(float delta)
        {
        }



        void OnTimesUp()
        {
            // Time's up!
            game.mIsTimesUp = true;
            game.SetCurrentState(game.ResultState);
        }


        void OnRoundResult(bool _result) {
            game.m_RoundManager.NewRound();
            game.OnResult(_result);
        }

        #region VARS

        CountdownTimer gameTime = new CountdownTimer(60.0f);
        MissingLetterGame game;
        IAudioSource timesUpAudioSource;
        bool hurryUpSfx;

        #endregion
    }
}
