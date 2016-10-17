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

            game.questionManager.StartNewQuestion();
            game.eggController.Reset();

            game.Context.GetPopupWidget().Show(OnPopupCloseRequested, "Question Description", true);
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
            WordData questionWordData = game.questionManager.GetQuestionWordData();

            if (questionWordData == null)
            {
                game.eggController.eggLivingLetter.SetQuestionText(game.questionManager.GetlLetterDataSequence()[0]);
                game.Context.GetAudioManager().PlayLetter(((LetterData)game.questionManager.GetlLetterDataSequence()[0]).Key);
                game.eggController.StartTrembling();

                game.eggButtonBox.LightUpButtons(true, true, 1f, 2f, OnLightUpButtonsComplete);
            }
            else
            {
                game.eggController.eggLivingLetter.SetQuestionText(questionWordData);
                game.eggButtonBox.LightUpButtons(true, false, 1f, 1f, OnLightUpButtonsComplete);
            }
        }

        void OnLightUpButtonsComplete()
        {
            game.SetCurrentState(game.PlayState);
        }
    }
}