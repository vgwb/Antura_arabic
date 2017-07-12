using System.Collections.Generic;
using Antura.LivingLetters;
using Antura.Tutorial;
using Antura.MinigamesCommon;
using UnityEngine;

namespace Antura.Minigames.FastCrowd
{
    public class FastCrowdTutorialState : IState
    {
        FastCrowdGame game;

        float tutorialStartTimer;
        int answerCounter;

        bool tutorialStarted;

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

            tutorialStarted = false;

            if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Alphabet)
            {
                game.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.FastCrowd_alphabet_Tuto, () => { StartTutorial(); });
            }
            else if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Counting)
            {
                game.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.FastCrowd_counting_Tuto, () => { StartTutorial(); });
            }
            else if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Letter)
            {
                game.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.FastCrowd_letter_Tuto, () => { StartTutorial(); });
            }
            else if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Spelling)
            {
                game.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.FastCrowd_spelling_Tuto, () => { StartTutorial(); });
            }
            else if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Words)
            {
                game.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.FastCrowd_words_Tuto, () => { StartTutorial(); });
            }
            else
            {
                StartTutorial();
            }
        }

        public void ExitState()
        {
            game.QuestionManager.OnCompleted -= OnQuestionCompleted;
            game.QuestionManager.OnDropped -= OnAnswerDropped;
            game.QuestionManager.Clean();

            game.showTutorial = false;
        }

        void StartTutorial()
        {
            DrawTutorial();

            tutorialStartTimer = 3f;

            tutorialStarted = true;
        }

        void OnQuestionCompleted()
        {
            game.SetCurrentState(game.QuestionState);
        }

        void OnAnswerDropped(ILivingLetterData data, bool result)
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
            if(tutorialStarted)
            {
                tutorialStartTimer += -delta;

                if (tutorialStartTimer <= 0f)
                {
                    tutorialStartTimer = 3f;

                    DrawTutorial();
                }
            }
        }

        void DrawTutorial()
        {
            if (game.QuestionManager.crowd.GetLetter(game.QuestionManager.dropContainer.GetActiveData()) == null)
                return;

            StrollingLivingLetter tutorialLetter = game.QuestionManager.crowd.GetLetter(game.QuestionManager.dropContainer.GetActiveData());

            Vector3 startLine = tutorialLetter.gameObject.GetComponent<LivingLetterController>().contentTransform.position;
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