using EA4S.Assessment;
using System;
using System.Collections;
using UnityEngine;

namespace EA4S.IdentifyLetter
{
    public class PlayGameState : IGameState
    {
        IdentifyLetterGame game;

        public PlayGameState(IdentifyLetterGame game)
        {
            this.game = game;
        }


        public void EnterState()
        {
            var cam = Camera.main;
            float h = cam.orthographicSize * 2;
            float w = h * cam.aspect;

            game.Context.GetAudioManager().PlayMusic(Music.MainTheme);
            game.questionController.audioManager = game.Context.GetAudioManager();
            game.questionController.positions = new DefaultPositionsProvider(h, w, Vector3.zero); // Inject here
            game.questionController.dragManager = new DragManager(game.score);
            game.questionController.dragManager.SetAudioManager(game.questionController.audioManager);
            game.questionController.dragManager.SetDragType(DragType.Anywhere);
            game.questionController.Reset();
            game.StartCoroutine(PlayCoroutine());

        }

        IEnumerator PlayCoroutine()
        {
            int questionsNumber = LetterShapeConfiguration.Instance.SimultaneosQuestions;
            int rounds = LetterShapeConfiguration.Instance.Rounds;
            IQuestionProvider provider = LetterShapeConfiguration.Instance.Questions;

            for (int i = 0; i < rounds; i++) {
                for (int j = 0; j < questionsNumber; j++) {
                    game.questionController.AddQuestion(provider.GetNextQuestion());
                }

                game.questionController.SpawnAllLivingLetters(game.score);

                while (game.questionController.IsAnimating())
                    yield return null;

                while (game.score.AnsweredAll() == false)
                    yield return null;

                game.questionController.Cleanup();
                while (game.questionController.IsAnimating())
                    yield return null;

                yield return null;
            }

            game.SetCurrentState(game.ResultState);
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            TimeEngine.Instance.Update(delta);
            game.questionController.dragManager.Update(delta);
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
