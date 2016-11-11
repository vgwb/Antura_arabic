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
            game.SetInIdle(true);
            game.m_RoundManager.onAnswered += OnRoundResult;
        }


        public void ExitState() {
            TutorialUI.Clear(true);
            game.m_RoundManager.GetCorrectLLObject().GetComponent<LetterBehaviour>().StopSuggest();
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
            }
        }

        public void Update(float delta) {
            delayTime -= delta;
            if(delayTime < 0 && !suggested)
            {
                game.m_RoundManager.GetCorrectLLObject().GetComponent<LetterBehaviour>().SuggestLetter();
                Vector3 pos = game.m_RoundManager.GetCorrectLLObject().transform.position + Vector3.back * 0.8f + Vector3.up * 3;
                TutorialUI.ClickRepeat(pos, 90, 1.5f);
                suggested = true;
            }
        }

        public void UpdatePhysics(float delta) {

        }
    }
}
