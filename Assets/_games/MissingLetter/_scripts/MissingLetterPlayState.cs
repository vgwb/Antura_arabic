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
            gameTime = new CountdownTimer(game.m_fGameTime);
            gameTime.onTimesUp += OnTimesUp;
        }

        public void EnterState()
        {
            game.m_RoundManager.onAnswered += OnRoundResult;
            game.mIsTimesUp = false;
            game.ResetScore();

            hurryUpSfx = false;

            gameTime.Reset();
            gameTime.Start();

            game.Context.GetOverlayWidget().Initialize(true, true, false);

            game.Context.GetOverlayWidget().SetStarsThresholds(game.STARS_1_THRESHOLD, game.STARS_2_THRESHOLD, game.STARS_3_THRESHOLD);
            game.Context.GetOverlayWidget().SetClockDuration(gameTime.Duration);
            game.Context.GetOverlayWidget().SetClockTime(gameTime.Time);

            game.m_RoundManager.NewRound();
        }

        public void ExitState()
        {
            if (timesUpAudioSource != null)
                timesUpAudioSource.Stop();

            gameTime.Stop();
        }

        public void Update(float delta)
        {

            if(game.miAnturaTriggersIndex < game.mafAnturaEnterTriggers.Length && gameTime.Time <= game.mafAnturaEnterTriggers[game.miAnturaTriggersIndex]) {
                if (game.IsInIdle()) {

                    ++game.miAnturaTriggersIndex;
                    game.mAnturaRef.GetComponent<AnturaBehaviour>().EnterScene(game.mfAnturaAnimDuration);
                    game.SetInIdle(false);
                    game.StartCoroutine(Utils.LaunchDelay(game.mfAnturaAnimDuration / 6, game.m_RoundManager.ShuffleLetters, game.mfAnturaAnimDuration / 2));
                    game.StartCoroutine(Utils.LaunchDelay(game.mfAnturaAnimDuration, game.SetInIdle, true));
                } else {
                    game.mafAnturaEnterTriggers[game.miAnturaTriggersIndex] -= 3.0f;
                }
            }

            game.Context.GetOverlayWidget().SetClockTime(gameTime.Time);


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
            game.Context.GetOverlayWidget().OnClockCompleted();
            game.SetCurrentState(game.ResultState);
        }


        void OnRoundResult(bool _result) {
            game.OnResult(_result);
            game.m_RoundManager.NewRound();
        }


        #region VARS

        CountdownTimer gameTime;
        MissingLetterGame game;
        IAudioSource timesUpAudioSource;
        bool hurryUpSfx;

        #endregion
    }
}
