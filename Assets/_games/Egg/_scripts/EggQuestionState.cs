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

            game.Context.GetPopupWidget().Show(OnPopupCloseRequested, game.questionManager.GetQuestionDescription(), true);
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
        }

        public void UpdatePhysics(float delta)
        {
        }

        void OnPopupCloseRequested()
        {
            game.Context.GetPopupWidget().Hide();

            game.eggController.MoveNext(2f, OnEggEnterComplete);
        }

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
            bool noColor = (game.gameDifficulty >= 0.25f && game.gameDifficulty < 0.5f) || game.gameDifficulty >= 0.75f;

            bool isSequence = game.questionManager.IsSequence();
            
            if(isSequence)
            {
                game.eggController.eggLivingLetter.SetLetter(game.questionManager.GetlLetterDataSequence()[0]);

                game.eggButtonBox.LightUpButtons(noColor, true, false, 1f, 1f, OnLightUpButtonsComplete);
            }
            else
            {
                game.eggController.eggLivingLetter.SetLetter(game.questionManager.GetlLetterDataSequence()[0]);

                game.Context.GetAudioManager().PlayLetter(((LL_LetterData)game.questionManager.GetlLetterDataSequence()[0]));
                game.eggController.StartTrembling();
                
                if(noColor)
                {
                    OnLightUpButtonsComplete();
                }
                else
                {
                    game.eggButtonBox.LightUpButtons(false, true, true, 1f, 2f, OnLightUpButtonsComplete);
                }
            }
        }

        void OnLightUpButtonsComplete()
        {
            game.SetCurrentState(game.PlayState);
        }
    }
}