using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Random = UnityEngine.Random;

namespace EA4S.MixedLetters
{
    public class SeparateLettersSpawnerController : MonoBehaviour
    {
        public static SeparateLettersSpawnerController instance;

        // The delay to start dropping the letters, in seconds:
        private const float LOSE_ANIMATION_DROP_DELAY = 0.5f;

        // The time offset between each letter drop, in seconds:
        private const float LOSE_ANIMATION_DROP_OFFSET = 0.1f;

        // The delay to start vanishing the letters, in seconds:
        private const float LOSE_ANIMATION_POOF_DELAY = 1f;

        // The time offset between each letter vanish, in seconds:
        private const float LOSE_ANIMATION_POOF_OFFSET = 0.1f;

        // The delay to announce the end of the animation, in seconds:
        private const float LOSE_ANIMATION_END_DELAY = 1.5f;

        // The delay to start vanishing the letters (for the win animation), in seconds:
        private const float WIN_ANIMATION_POOF_DELAY = 1f;

        // The time offset between each letter vanish, in seconds:
        private const float WIN_ANIMATION_POOF_OFFSET = 0.1f;

        // The delay for the big LL (with the whole word) to appear, in seconds:
        private const float WIN_ANIMATION_BIG_LL_DELAY = 0.5f;

        // The delay to announce the end of the animation, in seconds:
        private const float WIN_ANIMATION_END_DELAY = 2f;

        public SeparateLetterController[] separateLetterControllers;

        public AudioSource audioSource;

        private IEnumerator spawnLettersCoroutine;

        void Awake()
        {
            instance = this;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SpawnLetters(List<ILivingLetterData> lettersToSpawn, Action spawnOverCallback)
        {
            spawnLettersCoroutine = SpawnLettersCoroutine(lettersToSpawn, spawnOverCallback);
            StartCoroutine(spawnLettersCoroutine);
        }

        private IEnumerator SpawnLettersCoroutine(List<ILivingLetterData> lettersToSpawn, Action spawnOverCallback)
        {
            PlayCartoonFightSfx();

            yield return new WaitForSeconds(1);

            List<int> indices = new List<int>();
            for (int i = 0; i < lettersToSpawn.Count; i++)
            {
                indices.Add(i);
            }

            bool throwLetterToTheRight = Random.Range(1, 40) % 2 == 0;

            for (int i = 0; i < lettersToSpawn.Count; i++)
            {
                int randIndex = indices[Random.Range(0, indices.Count)];
                indices.Remove(randIndex);

                LL_LetterData letterToSpawn = (LL_LetterData)lettersToSpawn[randIndex];

                SeparateLetterController separateLetterController = separateLetterControllers[i];
                separateLetterController.Enable();
                separateLetterController.SetPosition(transform.position, false);
                separateLetterController.SetLetter(letterToSpawn);
                separateLetterController.SetRotation(new Vector3(0, 0, Random.Range(0, 4) * 90));
                separateLetterController.SetIsKinematic(false);
                separateLetterController.SetCorrectDropZone(MixedLettersGame.instance.dropZoneControllers[randIndex]);
                MixedLettersGame.instance.dropZoneControllers[randIndex].correctLetter = separateLetterController;
                separateLetterController.SetIsSubjectOfTutorial(MixedLettersGame.instance.roundNumber == 0 && randIndex == 0);
                separateLetterController.AddForce(new Vector3(throwLetterToTheRight ? Random.Range(2f, 6f) : Random.Range(-6f, -2f), Constants.GRAVITY.y * -0.45f), ForceMode.VelocityChange);

                throwLetterToTheRight = !throwLetterToTheRight;

                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.ThrowObj);
                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlayLetterData(letterToSpawn);

                yield return new WaitForSeconds(0.75f);
            }

            yield return new WaitForSeconds(1);
            audioSource.Stop();

            spawnOverCallback.Invoke();
        }

        private void PlayCartoonFightSfx()
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.Play();
        }

        public void ShowLoseAnimation(Action OnAnimationEnded)
        {
            StartCoroutine(LoseAnimationCoroutine(OnAnimationEnded));
        }

        private IEnumerator LoseAnimationCoroutine(Action OnAnimationEnded)
        {
            int numLettersToDrop = 0;

            foreach (DropZoneController dropZoneController in MixedLettersGame.instance.dropZoneControllers)
            {
                if (dropZoneController.isActiveAndEnabled && dropZoneController.droppedLetter != null)
                {
                    numLettersToDrop++;
                }
            }

            MixedLettersGame.instance.HideDropZones();

            if (numLettersToDrop != 0)
            {
                yield return new WaitForSeconds(LOSE_ANIMATION_DROP_DELAY);

                foreach (DropZoneController dropZoneController in MixedLettersGame.instance.dropZoneControllers)
                {
                    if (dropZoneController.droppedLetter != null)
                    {
                        dropZoneController.droppedLetter.SetIsKinematic(false);
                        numLettersToDrop--;

                        if (numLettersToDrop == 0)
                        {
                            break;
                        }

                        else
                        {
                            yield return new WaitForSeconds(LOSE_ANIMATION_DROP_OFFSET);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(LOSE_ANIMATION_POOF_DELAY);

            List<int> letterIndicesList = new List<int>();
            List<float> letterAbscissasList = new List<float>();

            for (int i = 0; i < separateLetterControllers.Length; i++)
            {
                SeparateLetterController letterController = separateLetterControllers[i];

                if (letterController.isActiveAndEnabled)
                {
                    letterIndicesList.Add(i);
                    letterAbscissasList.Add(letterController.transform.position.x);
                }
            }

            int[] letterIndicesArray = letterIndicesList.ToArray();
            float[] letterAbscissasArray = letterAbscissasList.ToArray();

            Array.Sort(letterAbscissasArray, letterIndicesArray);

            for (int i = letterIndicesArray.Length - 1; i >= 0; i--)
            {
                separateLetterControllers[letterIndicesArray[i]].Vanish();

                if (i != 0)
                {
                    yield return new WaitForSeconds(LOSE_ANIMATION_POOF_OFFSET);
                }
            }

            yield return new WaitForSeconds(LOSE_ANIMATION_END_DELAY);

            OnAnimationEnded();
        }

        public void ShowWinAnimation(Action OnAnimationEnded)
        {
            StartCoroutine(WinAnimationCoroutine(OnAnimationEnded));
        }

        private IEnumerator WinAnimationCoroutine(Action OnAnimationEnded)
        {
            yield return new WaitForSeconds(WIN_ANIMATION_POOF_DELAY);

            for (int i = 0; i < MixedLettersGame.instance.lettersInOrder.Count; i++)
            {
                if (i != 0)
                {
                    yield return new WaitForSeconds(WIN_ANIMATION_POOF_OFFSET);
                }

                MixedLettersGame.instance.dropZoneControllers[i].droppedLetter.Vanish();
            }

            MixedLettersGame.instance.HideDropZones();

            yield return new WaitForSeconds(WIN_ANIMATION_BIG_LL_DELAY);

            VictimLLController.instance.Enable();
            VictimLLController.instance.Reset();
            VictimLLController.instance.DoHooray();
            VictimLLController.instance.ShowVictoryRays();

            if (MixedLettersConfiguration.Instance.Variation == MixedLettersConfiguration.MixedLettersVariation.Spelling)
            {
                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlayLetterData(VictimLLController.instance.letterObjectView.Data);
            }

            yield return new WaitForSeconds(WIN_ANIMATION_END_DELAY);

            OnAnimationEnded();
        }

        public void SetLettersDraggable()
        {
            foreach (SeparateLetterController separateLetterController in separateLetterControllers)
            {
                separateLetterController.SetDraggable();
            }
        }

        public void SetLettersNonInteractive()
        {
            foreach (SeparateLetterController separateLetterController in separateLetterControllers)
            {
                separateLetterController.SetNonInteractive();
            }
        }

        public void ResetLetters()
        {
            foreach (SeparateLetterController separateLetterController in separateLetterControllers)
            {
                separateLetterController.Reset();
            }
        }

        public void DisableLetters()
        {
            foreach (SeparateLetterController separateLetterController in separateLetterControllers)
            {
                separateLetterController.Disable();
            }
        }
    }
}

