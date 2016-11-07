using UnityEngine;
using System.Collections.Generic;

namespace EA4S.Egg
{
    public class EggQuestionState : IGameState
    {
        EggGame game;

        public EggQuestionState(EggGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.eggButtonBox.RemoveButtons();

            bool onlyLetter = Random.Range(0, 2) == 0;

            game.questionManager.StartNewQuestion(game.gameDifficulty, onlyLetter);
            game.eggController.Reset();

            game.eggController.MoveNext(2f, OnEggEnterComplete);
        }

        public void ExitState() { }
        public void Update(float delta) { }        
        public void UpdatePhysics(float delta) { }

        void OnEggEnterComplete()
        {
            SetAndShowEggButtons();
        }

        void SetAndShowEggButtons()
        {
            List<ILivingLetterData> lLetterDataSequence = game.questionManager.GetlLetterDataSequence();

            for (int i = 0; i < lLetterDataSequence.Count; i++)
            {
                game.eggButtonBox.AddButton(lLetterDataSequence[i]);
            }

            game.eggButtonBox.SetButtonsOnPosition();
            game.eggButtonBox.ShowButtons();

            ShowQuestionSequence();
        }

        void ShowQuestionSequence()
        {
            bool lightUpButtons = game.gameDifficulty < 0.25f || (game.gameDifficulty >= 0.5f && game.gameDifficulty < 0.75f);

            bool isSequence = game.questionManager.IsSequence();

            game.eggController.EmoticonInterrogative();

            if (isSequence)
            {
                game.eggController.SetQuestion(game.questionManager.GetlLetterDataSequence());

                game.eggController.QuestionParticleEnabled();
                game.eggButtonBox.PlayButtonsAudio(lightUpButtons, false, 0f, OnQuestionAudioComplete);
            }
            else
            {
                game.eggController.SetQuestion(game.questionManager.GetlLetterDataSequence()[0]);

                if (lightUpButtons)
                {
                    game.eggController.PlayAudioQuestion(delegate ()
                       {
                           game.eggController.QuestionParticleEnabled();

                           game.eggButtonBox.PlayButtonsAudio(true, true, 0.5f, OnQuestionAudioComplete);
                       });

                    game.eggController.StartTrembling();
                }
                else
                {
                    game.eggController.PlayAudioQuestion(OnQuestionAudioComplete);
                }
            }
        }

        void OnQuestionAudioComplete()
        {
            game.eggController.EmoticonClose();

            game.eggController.QuestionParticleDisabled();
            game.SetCurrentState(game.PlayState);
        }
    }
}