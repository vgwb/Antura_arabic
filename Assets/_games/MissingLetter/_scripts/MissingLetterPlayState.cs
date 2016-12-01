using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace EA4S.MissingLetter
{
    public class MissingLetterPlayState : IGameState
    {
        
        public MissingLetterPlayState(MissingLetterGame _game)
        {
            this.m_oGame = _game;
            M_oGameTime = new CountdownTimer(_game.m_fGameTime);
            M_oGameTime.onTimesUp += OnTimesUp;
        }

        public void EnterState()
        {
            m_oGame.m_oRoundManager.onAnswered += OnRoundResult;
            m_oGame.m_bIsTimesUp = false;
            m_oGame.ResetScore();

            m_bHurryUpSfx = false;

            M_oGameTime.Reset();
            M_oGameTime.Start();

            m_oGame.Context.GetOverlayWidget().Initialize(true, true, false);

            m_oGame.Context.GetOverlayWidget().SetStarsThresholds(m_oGame.STARS_1_THRESHOLD, m_oGame.STARS_2_THRESHOLD, m_oGame.STARS_3_THRESHOLD);
            m_oGame.Context.GetOverlayWidget().SetClockDuration(M_oGameTime.Duration);
            m_oGame.Context.GetOverlayWidget().SetClockTime(M_oGameTime.Time);

            m_oGame.m_oRoundManager.NewRound();
        }

        public void ExitState()
        {
            AudioManager.I.StopMusic();

            //AudioManager.I.StopSfx(Sfx.DangerClockLong);
            if (timesUpAudioSource != null)
                timesUpAudioSource.Stop();

            M_oGameTime.Stop();
        }

        public void Update(float _delta)
        {

            if(m_oGame.m_iAnturaTriggersIndex < m_oGame.m_afAnturaEnterTriggers.Length && M_oGameTime.Time <= m_oGame.m_afAnturaEnterTriggers[m_oGame.m_iAnturaTriggersIndex]) {
                if (m_oGame.IsInIdle()) {
                    ++m_oGame.m_iAnturaTriggersIndex;
                    m_oGame.m_oAntura.GetComponent<AnturaBehaviour>().EnterScene(m_oGame.m_fAnturaAnimDuration);
                    m_oGame.StartCoroutine(Utils.LaunchDelay(m_oGame.m_fAnturaAnimDuration / 6, m_oGame.m_oRoundManager.ShuffleLetters, m_oGame.m_fAnturaAnimDuration / 2));
                } else {
                    m_oGame.m_afAnturaEnterTriggers[m_oGame.m_iAnturaTriggersIndex] -= 3.0f;
                }
            }

            m_oGame.Context.GetOverlayWidget().SetClockTime(M_oGameTime.Time);


            if (!m_bHurryUpSfx)
            {
                if (M_oGameTime.Time < 4f)
                {
                    m_bHurryUpSfx = true;
                    //AudioManager.I.PlaySfx(Sfx.DangerClockLong);
                    timesUpAudioSource = m_oGame.Context.GetAudioManager().PlaySound(Sfx.DangerClockLong);
                }
            }

            M_oGameTime.Update(_delta);
        }

        public void UpdatePhysics(float delta)
        {
        }



        void OnTimesUp()
        {
            // Time's up!
            m_oGame.m_bIsTimesUp = true;
            m_oGame.Context.GetOverlayWidget().OnClockCompleted();
            m_oGame.SetCurrentState(m_oGame.ResultState);
        }


        void OnRoundResult(bool _result) {
            m_oGame.OnResult(_result);
            m_oGame.m_oRoundManager.NewRound();
        }


        #region VARS

        CountdownTimer M_oGameTime;
        MissingLetterGame m_oGame;
        IAudioSource timesUpAudioSource;
        bool m_bHurryUpSfx;

        #endregion
    }
}
