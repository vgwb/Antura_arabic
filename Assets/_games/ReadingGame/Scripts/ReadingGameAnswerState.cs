using DG.DeExtensions;
using System.Collections.Generic;
using System.Linq;

namespace EA4S.ReadingGame
{
    public class ReadingGameAnswerState : IGameState
    {
        ReadingGameGame game;

        ILivingLetterData correct;

        public float ReadTime;
        public float MaxTime;

        public ReadingGameAnswerState(ReadingGameGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.isTimesUp = false;

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

            float delay = 0;
            foreach (var c in choices)
            {
                box.AddButton(c, OnAnswered, delay);
                delay += 0.2f;
            }

            game.radialWidget.Show();
            game.radialWidget.Reset(ReadTime / MaxTime);
            game.radialWidget.inFront = true;
            game.radialWidget.pulsing = true;
        }


        public void ExitState()
        {
            var inputManager = game.Context.GetInputManager();

            game.circleBox.GetComponent<CircleButtonBox>().Clear();
            
            game.radialWidget.inFront = false;
        }

        public void Update(float delta)
        {

        }

        public void UpdatePhysics(float delta)
        {

        }

        void OnAnswered(CircleButton button)
        {
            game.Context.GetAudioManager().PlaySound(button.Answer == correct ? Sfx.OK : Sfx.KO);

            if (button.Answer == correct)
            {
                // Assign score
                game.AddScore((int)(ReadTime) + 1);
                game.radialWidget.percentage = 0;
                game.radialWidget.pulsing = false;
                game.Context.GetCheckmarkWidget().Show(true);

            }
            else
            {
                game.radialWidget.PoofAndHide();

                game.Context.GetCheckmarkWidget().Show(false);

                if (game.RemoveLife())
                    return;
            }

            game.circleBox.GetComponent<CircleButtonBox>().Clear(()
                =>
            {
                game.SetCurrentState(game.ReadState);
            }, 0.5f);
        }
    }
}