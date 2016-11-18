using DG.DeExtensions;
using System.Collections;
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
        CircleButton correctButton;

        public ReadingGameAnswerState(ReadingGameGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.antura.AllowSitting = false;
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
                var button = box.AddButton(c, OnAnswered, delay);
                delay += 0.2f;

                if (c == correct)
                {
                    correctButton = button;
                }
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

                game.StartCoroutine(DoEndAnimation(true, correctButton));

                game.antura.Mood = ReadingGameAntura.AnturaMood.HAPPY;
            }
            else
            {
                game.radialWidget.PoofAndHide();

                game.StartCoroutine(DoEndAnimation(false, correctButton));

                game.antura.animator.DoShout(() => { ReadingGameConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.DogBarking); });
            }
        }

        IEnumerator DoEndAnimation(bool correct, CircleButton correctButton)
        {
            if (correct)
                correctButton.SetColor(UnityEngine.Color.green);
            else
                correctButton.SetColor(UnityEngine.Color.red);

            yield return new UnityEngine.WaitForSeconds(1.0f);

            if (!correct && game.RemoveLife())
                yield break;

            game.circleBox.GetComponent<CircleButtonBox>().Clear(() =>
            {
                game.SetCurrentState(game.ReadState);
            }, 0.5f);
        }
    }
}