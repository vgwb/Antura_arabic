using System.Collections.Generic;
using UnityEngine;

namespace EA4S.FastCrowd
{
    public class FastCrowdTutorialState : IGameState
    {
        FastCrowdGame game;

        float tutorialStartTimer;
        int answerCounter;

        public FastCrowdTutorialState(FastCrowdGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            answerCounter = 2;
            game.QuestionManager.OnCompleted += OnQuestionCompleted;
            game.QuestionManager.OnDropped += OnAnswerDropped;

            if (game.CurrentChallenge != null)
                game.QuestionManager.StartQuestion(game.CurrentChallenge, game.NoiseData);
            else
                game.QuestionManager.Clean();

            StartTutorial();

            tutorialStartTimer = 3f;
        }

        public void ExitState()
        {
            game.QuestionManager.OnCompleted -= OnQuestionCompleted;
            game.QuestionManager.OnDropped -= OnAnswerDropped;
            game.QuestionManager.Clean();

            game.showTutorial = false;
        }

        void OnQuestionCompleted()
        {
            game.SetCurrentState(game.QuestionState);
        }

        void OnAnswerDropped(bool result)
        {
            if (result)
            {
                --answerCounter;

                if (answerCounter <= 0 &&
                    (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Alphabet ||
                    FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Counting ||
                    FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Words)
                    )
                {
                    game.SetCurrentState(game.QuestionState);
                    return;
                }
            }

            tutorialStartTimer = 3f;
            game.Context.GetCheckmarkWidget().Show(result);
            game.Context.GetAudioManager().PlaySound(result ? Sfx.OK : Sfx.KO);
        }

        public void Update(float delta)
        {
            tutorialStartTimer += -delta;

            if (tutorialStartTimer <= 0f)
            {
                tutorialStartTimer = 3f;

                StartTutorial();
            }
        }

        void StartTutorial()
        {
            if (game.QuestionManager.crowd.GetLetter(game.QuestionManager.dropContainer.GetActiveData()) == null)
                return;

            StrollingLivingLetter tutorialLetter = game.QuestionManager.crowd.GetLetter(game.QuestionManager.dropContainer.GetActiveData());

            Vector3 startLine = tutorialLetter.gameObject.GetComponent<LetterObjectView>().contentTransform.position;
            Vector3 endLine = game.QuestionManager.dropContainer.transform.position;

            List<StrollingLivingLetter> nearLetters = new List<StrollingLivingLetter>();

            game.QuestionManager.crowd.GetNearLetters(nearLetters, startLine, 10f);

            for (int i = 0; i < nearLetters.Count; i++)
            {
                if (nearLetters[i] != tutorialLetter)
                {
                    nearLetters[i].Scare(startLine, 3f);
                }
            }

            tutorialLetter.Tutorial();

            TutorialUI.DrawLine(startLine, endLine, TutorialUI.DrawLineMode.Finger);
        }

        public void UpdatePhysics(float delta) { }
    }
}