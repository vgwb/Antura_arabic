namespace EA4S.MinigamesCommon
{
    /// <summary>
    /// The game state reached when the minigame ends.
    /// This state is present in all minigames and is always accessed last.
    /// </summary>
    public class OutcomeGameState : IGameState
    {
        MiniGame game;

        public OutcomeGameState(MiniGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            int starsScore = game.StarsScore;
            if (starsScore > 3)
                starsScore = 3;

            game.Context.GetStarsWidget().Show(starsScore);

            Db.LocalizationDataId text;

            if (starsScore < 1)
                text = (Db.LocalizationDataId.Keeper_Bad_2);
            else if (starsScore < 2)
                text = (Db.LocalizationDataId.Keeper_Good_5);
            else if (starsScore < 3)
                text = (Db.LocalizationDataId.Keeper_Good_2);
            else
                text = (Db.LocalizationDataId.Keeper_Good_1);
            
            //game.Context.GetSubtitleWidget().DisplaySentence(text);
            game.Context.GetAudioManager().PlayDialogue(text);
        }

        public void ExitState()
        {
            game.Context.GetStarsWidget().Hide();
        }

        public void Update(float delta)
        {
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
