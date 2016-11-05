using UnityEngine;

namespace EA4S.MissingLetter {

    public class MissingLetterTutorialState : IGameState {
        MissingLetterGame game;
        float delayTime = 2f;
        bool suggested = false;

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

        void OnRoundResult(bool _result)
        {
            if (_result)
            {
                //TODO: tutorial finito ... mostrare qualcosa a livello di UI ?
                game.SetCurrentState(game.PlayState);
            }
            else
            {
                var _LL = game.m_RoundManager.GetCorrectLLObject();
                _LL.GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_dancing);
                //Utils.LaunchDelay<bool>(1.0f, delegate (bool b){ _LL.GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_idle); }, true);  
            }
        }

        public void Update(float delta) {
            delayTime -= delta;
            if(delayTime < 0 && !suggested)
            {
                game.m_RoundManager.GetCorrectLLObject().GetComponent<LetterBehaviour>().SuggestLetter();
                game.mFinger.GetComponent<FingerSuggestion>().DoSuggestion(game.m_RoundManager.GetCorrectLLObject());
                suggested = true;
            }
        }

        public void UpdatePhysics(float delta) {

        }
    }
}
