using UnityEngine;
using System.Collections;
using System;

using Random = UnityEngine.Random;

namespace EA4S.MixedLetters
{
    public class TutorialGameState : IGameState
    {
        MixedLettersGame game;

        private IAudioManager audioManager;

        private bool isSpelling;

        public TutorialGameState(MixedLettersGame game)
        {
            this.game = game;
            audioManager = game.Context.GetAudioManager();

            isSpelling = MixedLettersConfiguration.Instance.Variation == MixedLettersConfiguration.MixedLettersVariation.Spelling;
        }

        public void EnterState()
        {
            game.GenerateNewWord();

            VictimLLController.instance.HideVictoryRays();
            VictimLLController.instance.Reset();
            VictimLLController.instance.Enable();

            Vector3 victimLLPosition = VictimLLController.instance.transform.position;
            victimLLPosition.x = Random.Range(0, 40) % 2 == 0 ? 0.5f : -0.5f;
            VictimLLController.instance.SetPosition(victimLLPosition);

            audioManager.PlayDialogue(isSpelling ? Db.LocalizationDataId.MixedLetters_spelling_Title : Db.LocalizationDataId.MixedLetters_alphabet_Title, OnTitleVoiceOverDone);
        }

        private void OnTitleVoiceOverDone()
        {
            game.StartCoroutine(OnTitleVoiceOverDoneCoroutine());
        }

        private void OnQuestionOver()
        {
            game.StartCoroutine(OnQuestionOverCoroutine());
        }

        private IEnumerator OnQuestionOverCoroutine()
        {
            yield return new WaitForSeconds(MixedLettersConfiguration.Instance.Variation == MixedLettersConfiguration.MixedLettersVariation.Alphabet ? 1.5f : 3f);

            MixedLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.DogBarking);
            VictimLLController.instance.LookTowardsAntura();

            yield return new WaitForSeconds(0.25f);

            AnturaController.instance.Enable();
            AnturaController.instance.EnterScene(OnFightBegan, OnAnturaExitedScene);
        }

        private IEnumerator OnTitleVoiceOverDoneCoroutine()
        {
            yield return new WaitForSeconds(0.75f);

            game.SayQuestion(OnQuestionOver);
        }

        private void OnFightBegan()
        {
            AnturaController.instance.SetPosition(VictimLLController.instance.transform.position);
            AnturaController.instance.Disable();
            VictimLLController.instance.Disable();
            ParticleSystemController.instance.Enable();
            ParticleSystemController.instance.SetPosition(VictimLLController.instance.transform.position);
            SeparateLettersSpawnerController.instance.SetPosition(VictimLLController.instance.transform.position);
            SeparateLettersSpawnerController.instance.SpawnLetters(game.lettersInOrder, OnFightEnded);
        }

        private void OnFightEnded()
        {
            AnturaController.instance.Enable();
            AnturaController.instance.SetPositionWithOffset(VictimLLController.instance.transform.position, new Vector3(0, 0, 1f));
            ParticleSystemController.instance.Disable();
        }

        private void OnAnturaExitedScene()
        {
            audioManager.PlayDialogue(isSpelling ? Db.LocalizationDataId.MixedLetters_spelling_Intro : Db.LocalizationDataId.MixedLetters_alphabet_Intro, OnIntroVoiceOverDone);
        }

        private void OnIntroVoiceOverDone()
        {
            MixedLettersGame.instance.OnRoundStarted();
            audioManager.PlayDialogue(isSpelling ? Db.LocalizationDataId.MixedLetters_spelling_Tuto : Db.LocalizationDataId.MixedLetters_alphabet_Tuto);
        }

        public void ExitState()
        {

        }

        public void Update(float delta)
        {
            if (game.lastRoundWon)
            {
                game.SetCurrentState(game.ResultState);
            }
        }

        public void UpdatePhysics(float delta)
        {

        }
    }
}
