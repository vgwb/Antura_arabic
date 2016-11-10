using DG.DeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EA4S.ReadingGame
{
    public class ReadingGameAnswerState : IGameState
    {
        ReadingGameGame game;

        ILivingLetterData correct;

        public float ReadTime;

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
            var box = game.circleBox.GetComponent<CircleButtonBox>();
            box.Clear();
            box.ImageMode = true;

            correct = game.CurrentQuestion.GetCorrectAnswers().First();
            var wrongs = game.CurrentQuestion.GetWrongAnswers();

            var choices = new List<ILivingLetterData>();
            choices.Add(correct);
            choices.AddRange(wrongs);
            choices.Shuffle();

            foreach (var c in choices)
                box.AddButton(c, OnAnswered);
        }


        public void ExitState()
        {
            var inputManager = game.Context.GetInputManager();

            inputManager.onPointerDown -= OnPointerDown;

            game.circleBox.GetComponent<CircleButtonBox>().Clear();
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

        void OnAnswered(CircleButton button)
        {
            game.Context.GetAudioManager().PlaySound(button.Answer == correct ? Sfx.OK : Sfx.KO);

            if (button.Answer == correct)
            {
                // Assign score
                game.AddScore((int)(ReadTime) + 1);
                game.SetCurrentState(game.ReadState);
                game.Context.GetCheckmarkWidget().Show(true);

            }
            else
            {
                game.Context.GetCheckmarkWidget().Show(false);

                if (!game.RemoveLife())
                    game.SetCurrentState(game.ReadState);
            }
        }
    }
}