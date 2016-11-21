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
            
            game.Context.GetAudioManager().PlaySound(Sfx.TickAndWin);
            game.eggController.MoveNext(2f, OnEggEnterComplete);
        }

        public void ExitState()
        {
            game.eggButtonBox.SetOnPressedCallback(null);
        }

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
            game.eggButtonBox.SetOnPressedCallback(OnEggButtonPressed);

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
                game.eggButtonBox.PlayButtonsAudio(lightUpButtons, false, 0f, OnQuestionAudioComplete);
            }
            else
            {
                game.eggController.SetQuestion(game.questionManager.GetlLetterDataSequence()[0]);

                if (lightUpButtons)
                {
                    game.eggController.PlayAudioQuestion(delegate ()
                       {
                           EnableEggButtonsInput();
                           game.eggButtonBox.PlayButtonsAudio(true, true, 0.5f, OnQuestionAudioComplete);
                       });
                }
                else
                {
                    game.eggController.PlayAudioQuestion(OnQuestionAudioComplete);
                }
            }
        }

        void OnEggButtonPressed(ILivingLetterData letterData)
        {
            if(!game.questionManager.IsSequence())
            {
                game.eggButtonBox.StopButtonsAudio();
            }

            game.PlayState.OnEggButtonPressed(letterData);
        }

        void OnQuestionAudioComplete()
        {
            DisableEggButtonsInput();

            game.eggController.EmoticonClose();

            game.SetCurrentState(game.PlayState);
        }

        void EnableEggButtonsInput()
        {
            game.eggButtonBox.EnableButtonsInput();
        }

        void DisableEggButtonsInput()
        {
            game.eggButtonBox.DisableButtonsInput();
        }
    }
}