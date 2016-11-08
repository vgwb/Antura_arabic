using System;
using UnityEngine;

namespace EA4S.ReadingGame
{
    public class ReadingGameAnswerState : IGameState
    {
        ReadingGameGame game;

        public ReadingGameAnswerState(ReadingGameGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.isTimesUp = false;

            var inputManager = game.Context.GetInputManager();

            inputManager.onPointerDown += OnPointerDown;

            game.circleBox.SetActive(true);
        }


        public void ExitState()
        {
            var inputManager = game.Context.GetInputManager();

            inputManager.onPointerDown -= OnPointerDown;

            game.circleBox.SetActive(false);
        }

        public void Update(float delta)
        {
            
        }

        public void UpdatePhysics(float delta)
        {

        }

        void OnPointerDown()
        {
           
        }
    }
}