namespace EA4S.MissingLetter {
    public class MissingLetterTutorialState : IGameState {
        MissingLetterGame game;

        public MissingLetterTutorialState(MissingLetterGame game) {
            this.game = game;
        }

        public void EnterState() {
            game.Context.GetAudioManager().PlayMusic(Music.MainTheme);
            game.m_RoundManager.SetTutorial(true);
            game.m_RoundManager.NewRound();
            game.m_RoundManager.onAnswered += OnRoundResult;
        }


        public void ExitState() {
            game.m_RoundManager.SetTutorial(false);
            game.m_RoundManager.onAnswered -= OnRoundResult;
        }

        void OnRoundResult(bool _result) {
            if (_result) {
                //TODO: tutorial finito ... mostrare qualcosa a livello di UI ?
                game.SetCurrentState(game.PlayState);
            }
            else
            {
                var _LL = game.m_RoundManager.GetCorrectLLObject();
                _LL.GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_dancing);
            }
        }

        public void Update(float delta) {
            //TODO: dopo TOT tempo mostrare un aiuto ? Tipo un dito che indica la LL giusta ??
        }

        public void UpdatePhysics(float delta) {

        }
    }
}
