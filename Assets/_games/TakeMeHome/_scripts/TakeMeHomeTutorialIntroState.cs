namespace EA4S.TakeMeHome
{
    public class TakeMeHomeTutorialIntroState : IGameState
    {

        TakeMeHomeGame game;
        
        public TakeMeHomeTutorialIntroState(TakeMeHomeGame game)
        {
            this.game = game;
        }
       
        public void EnterState()
        {
            TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlayDialogue(Db.LocalizationDataId.TakeMeHome_Title, playedTitleSFX);
            
            
        }

        private void playedTitleSFX()
        {
            UnityEngine.Debug.Log("Played Title");
            TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlayDialogue(Db.LocalizationDataId.TakeMeHome_Intro, playedIntroSFX);
        }
       

        private void playedIntroSFX()
        {
            UnityEngine.Debug.Log("Played Intro");
            game.spawnLetteAtTube();
        }
        
        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            if (game.currentLetter != null && !game.currentLetter.isMoving)
            {
                game.currentLetter.isDraggable = true;
                game.SetCurrentState(game.TutorialPlayState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
