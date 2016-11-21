using System;
using UnityEngine;

namespace EA4S.ReadingGame
{
    public class ReadingGameReadState : IGameState
    {
        CountdownTimer gameTime = new CountdownTimer(90.0f);
        ReadingGameGame game;
        IAudioSource timesUpAudioSource;

        bool hurryUpSfx;

        bool completedDragging = false;
        ReadingBar dragging;
        Vector2 draggingOffset;

        // song related
        float timeFarFromTarget = 0;
        float scoreAccumulator = 0;

        public ReadingGameReadState(ReadingGameGame game)
        {
            this.game = game;

            gameTime.onTimesUp += OnTimesUp;
        }

        public void EnterState()
        {
            game.antura.AllowSitting = true;
            game.isTimesUp = false;

            if (game.CurrentQuestionNumber >= ReadingGameGame.MAX_QUESTIONS)
            {
                game.EndGame(game.CurrentStars, game.CurrentScore);
                return;
            }

            ++game.CurrentQuestionNumber;

            // Reset game timer
            gameTime.Reset(ReadingGameGame.TIME_TO_ANSWER);

            if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.ReadAndAnswer)
            {
                gameTime.Start();

                game.Context.GetOverlayWidget().SetClockDuration(gameTime.Duration);
                game.Context.GetOverlayWidget().SetClockTime(gameTime.Time);
            }

            hurryUpSfx = false;

            var inputManager = game.Context.GetInputManager();

            inputManager.onPointerDown += OnPointerDown;
            inputManager.onPointerUp += OnPointerUp;

            game.blurredText.SetActive(true);
            //game.circleBox.SetActive(false);

            if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.ReadAndAnswer)
            {
                // Pick a question
                var pack = ReadingGameConfiguration.Instance.Questions.GetNextQuestion();
                game.CurrentQuestion = pack;
                if (pack != null)
                    game.barSet.SetData(pack.GetQuestion());
                else
                    game.EndGame(game.CurrentStars, game.CurrentScore);
            }
            else
            {
                game.barSet.SetShowTargets(ReadingGameConfiguration.Instance.Difficulty < 0.3f);
                game.barSet.SetShowArrows(ReadingGameConfiguration.Instance.Difficulty < 0.6f);

                game.barSet.SetData(game.alphabetSong);
                game.barSet.PlaySong(game.Context.GetAudioManager().PlayMusic(game.alphabetSongAudio), OnSongEnded);
            }

            game.barSet.active = true;
            completedDragging = false;
        }


        public void ExitState()
        {
            var inputManager = game.Context.GetInputManager();

            inputManager.onPointerDown -= OnPointerDown;
            inputManager.onPointerUp -= OnPointerUp;

            if (timesUpAudioSource != null)
                timesUpAudioSource.Stop();

            gameTime.Stop();

            game.barSet.active = false;
            game.barSet.Clear();
            game.blurredText.SetActive(false);
        }

        public void Update(float delta)
        {
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

            if (dragging != null)
            {
                var inputManager = game.Context.GetInputManager();
                completedDragging = dragging.SetGlassScreenPosition(inputManager.LastPointerPosition + draggingOffset,
                    ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.ReadAndAnswer);
            }
            else
            {
                if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.ReadAndAnswer)
                {

                    if (completedDragging)
                    {
                        var completedAllBars = game.barSet.SwitchToNextBar();

                        if (completedAllBars)
                        {
                            // go to Buttons State
                            game.AnswerState.ReadTime = gameTime.Time;
                            game.AnswerState.MaxTime = gameTime.Duration;
                            game.SetCurrentState(game.AnswerState);
                            return;
                        }
                    }

                    completedDragging = false;
                }
            }


            if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.ReadAndAnswer)
            {
                float perc = gameTime.Time / gameTime.Duration;

                if (perc < 0.05f)
                    game.antura.Mood = ReadingGameAntura.AnturaMood.SAD;
                else if (perc < 0.5f)
                    game.antura.Mood = ReadingGameAntura.AnturaMood.ANGRY;
                else
                    game.antura.Mood = ReadingGameAntura.AnturaMood.HAPPY;
            }
            else // Alphabet Song
            {
                float distance;
                if (game.barSet.GetFollowingDistance(out distance))
                {
                    if (distance > 100)
                    {
                        timeFarFromTarget += delta;
                    }
                    else
                    {
                        timeFarFromTarget = 0;
                        if (distance < 50)
                            scoreAccumulator += 1.15f * delta;
                        else
                            scoreAccumulator += 1 * delta;

                        if (scoreAccumulator >= 1)
                        {
                            game.AddScore((int)scoreAccumulator);
                            scoreAccumulator = scoreAccumulator - (int)scoreAccumulator;
                        }
                    }

                    if (timeFarFromTarget > 1.0f)
                        game.antura.Mood = ReadingGameAntura.AnturaMood.ANGRY;
                    else
                        game.antura.Mood = ReadingGameAntura.AnturaMood.HAPPY;
                }
            }
        }

        public void UpdatePhysics(float delta)
        {

        }

        void OnTimesUp()
        {
            // Time's up!
            game.barSet.active = false;
            game.isTimesUp = true;
            game.Context.GetOverlayWidget().OnClockCompleted();

            if (game.RemoveLife())
                return;

            // show time's up and back
            game.Context.GetPopupWidget().ShowTimeUp(
                () =>
                {
                    game.SetCurrentState(this);
                    game.Context.GetPopupWidget().Hide();
                });
        }

        void OnPointerDown()
        {
            if (dragging)
                return;

            var inputManager = game.Context.GetInputManager();
            dragging = game.barSet.PickGlass(Camera.main, inputManager.LastPointerPosition);

            if (dragging != null)
                draggingOffset = dragging.GetGlassScreenPosition() - inputManager.LastPointerPosition;
        }

        void OnPointerUp()
        {
            dragging = null;
        }

        void OnSongEnded()
        {
            game.EndGame(game.CurrentStars, game.CurrentScore);
        }
    }
}