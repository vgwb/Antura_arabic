using EA4S.MinigamesCommon;

namespace EA4S.Minigames.TakeMeHome
{
    public class TakeMeHomeTutorialIntroState : IState
    {

        TakeMeHomeGame game;
        
        public TakeMeHomeTutorialIntroState(TakeMeHomeGame game)
        {
            this.game = game;
        }
       
        public void EnterState()
        {
            TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.TakeMeHome_Title, playedTitleSFX);
            
            
        }

        private void playedTitleSFX()
        {
            UnityEngine.Debug.Log("Played Title");
            TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.TakeMeHome_Intro, playedIntroSFX);
        }
       

        private void playedIntroSFX()
        {
            if (!game.PerformTutorial)
            {
                game.SetCurrentState(game.IntroductionState);
                return;
            }

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
