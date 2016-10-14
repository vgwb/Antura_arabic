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
            game.questionManager.StartNewQuestion();
            game.eggController.Reset();
            game.eggController.MoveNext(2f, OnEggEnterComplete);
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
            WordData questionWordData = game.questionManager.GetQuestionWordData();
            List<EggButton> eggButtons;

            if (questionWordData == null)
            {
                game.Context.GetAudioManager().PlayLetter(((LetterData)game.questionManager.GetlLetterDataSequence()[0]).Key);
                eggButtons = game.eggButtonBox.GetButtons(true);
            }
            else
            {
                eggButtons = game.eggButtonBox.GetButtons(false);
            }

            LightUpButtons(eggButtons);
        }

        int currentLightUpButtonIndex;
        List<EggButton> lightUpButtons;
        void LightUpButtons(List<EggButton> buttons)
        {
            currentLightUpButtonIndex = 0;
            lightUpButtons = buttons;

            lightUpButtons[currentLightUpButtonIndex].LightUp(true, 1f, 1f, OnLightUpButtonComplete);

            currentLightUpButtonIndex++;
        }

        void OnLightUpButtonComplete()
        {
            if(currentLightUpButtonIndex == lightUpButtons.Count - 1)
            {
                lightUpButtons[currentLightUpButtonIndex].LightUp(true, 1f, 0f, OnLightUpButtonsComplete);
            }
            else
            {
                lightUpButtons[currentLightUpButtonIndex].LightUp(true, 1f, 0f, OnLightUpButtonComplete);
            }
            
            currentLightUpButtonIndex++;
        }

        void OnLightUpButtonsComplete()
        {
            game.SetCurrentState(game.PlayState);
        }
    }
}