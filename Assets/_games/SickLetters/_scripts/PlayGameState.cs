using System;
using TMPro;
using UnityEngine;
using System.Collections;

namespace EA4S.SickLetters
{
    public class PlayGameState : IGameState
    {
        SickLettersGame game;
        Vector3 correctDotPos;

        float timer = 2, t = 0;
        public PlayGameState(SickLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {

            //game.Context.GetOverlayWidget().Initialize(true, true, false);
            
            game.peocessDifiiculties(SickLettersConfiguration.Instance.Difficulty);

            timer = game.gameDuration;
            //game.Context.GetOverlayWidget().SetClockDuration(game.gameDuration);

            SickLettersConfiguration.Instance.Context.GetAudioManager().MusicEnabled = true;
            if (game.roundsCount == 0)
                SickLettersConfiguration.Instance.Context.GetAudioManager().PlayMusic(Music.Relax);
            else
            {
                SickLettersConfiguration.Instance.Context.GetAudioManager().PlayMusic(Music.MainTheme);
                game.Context.GetOverlayWidget().Initialize(true, true, false);
                game.Context.GetOverlayWidget().SetClockDuration(game.gameDuration);
            }

            game.LLPrefab.jumpIn();

        }

        public void ExitState()
        {
            game.disableInput = true;
        }

        public void Update(float delta)
        {
            game.peocessDifiiculties(SickLettersConfiguration.Instance.Difficulty);


            if (game.roundsCount > 0)
            {
                timer -= delta;
                game.Context.GetOverlayWidget().SetClockTime(timer);
            }
            if (timer < 0 /*|| game.roundsCount == 6*/)
            {
                game.SetCurrentState(game.ResultState);
                
            }

             if (Input.GetKeyDown(KeyCode.A))
             {
                 t = 1;
                 game.LLPrefab.jumpOut();
                 //game.LLPrefab.jumpIn();
            }

            correctDotPos = game.LLPrefab.correctDot.transform.TransformPoint(Vector3.Lerp(game.LLPrefab.correctDot.mesh.vertices[0], game.LLPrefab.correctDot.mesh.vertices[2], 0.5f));
            //scatterDDs();

            if(game.LLPrefab.correctDotCollider.transform.childCount == 0)
                game.LLPrefab.correctDotCollider.transform.position = correctDotPos;

            Debug.DrawRay(correctDotPos, -Vector3.forward * 10, Color.red);
            Debug.DrawRay(correctDotPos, -Vector3.right * 10, Color.yellow);
            //Debug.Log(v);
        }

        

        public void UpdatePhysics(float delta)
        {
            
        }

            
        
    }
}
