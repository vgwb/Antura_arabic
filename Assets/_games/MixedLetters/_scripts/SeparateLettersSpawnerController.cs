using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.MixedLetters
{
    public class SeparateLettersSpawnerController : MonoBehaviour
    {
        public static SeparateLettersSpawnerController instance;

        public SeparateLetterController[] separateLetterControllers;

        private IEnumerator spawnLettersCoroutine;

        public AudioSource audioSource;

        void Awake()
        {
            instance = this;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SpawnLetters(List<LL_LetterData> lettersToSpawn, System.Action spawnOverCallback)
        {
            spawnLettersCoroutine = SpawnLettersCoroutine(lettersToSpawn, spawnOverCallback);
            StartCoroutine(spawnLettersCoroutine);
        }

        private IEnumerator SpawnLettersCoroutine(List<LL_LetterData> lettersToSpawn, System.Action spawnOverCallback)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.Play();
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

                LL_LetterData letterToSpawn = lettersToSpawn[randIndex];

                SeparateLetterController separateLetterController = separateLetterControllers[i];
                separateLetterController.Enable();
                separateLetterController.SetPosition(transform.position, false);
                separateLetterController.SetLetter(letterToSpawn);
                separateLetterController.SetRotation(new Vector3(0, 0, Random.Range(0, 4) * 90));
                separateLetterController.SetIsKinematic(false);
                separateLetterController.AddForce(new Vector3(throwLetterToTheRight ? Random.Range(2f, 6f) : Random.Range(-6f, -2f), Constants.GRAVITY.y * -0.45f), ForceMode.VelocityChange);

                throwLetterToTheRight = !throwLetterToTheRight;

                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.ThrowObj);
                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlayLetter(letterToSpawn);

                yield return new WaitForSeconds(0.75f);
            }

            yield return new WaitForSeconds(1);
            audioSource.Stop();

            spawnOverCallback.Invoke();
        }

        public void SetLettersDraggable(bool isDraggable)
        {
            foreach (SeparateLetterController separateLetterController in separateLetterControllers)
            {
                separateLetterController.SetDraggable(isDraggable);
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

