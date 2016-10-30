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

        void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SpawnLetters(List<LL_LetterData> lettersToSpawn)
        {
            spawnLettersCoroutine = SpawnLettersCoroutine(lettersToSpawn);
            StartCoroutine(spawnLettersCoroutine);
        }

        private IEnumerator SpawnLettersCoroutine(List<LL_LetterData> lettersToSpawn)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < lettersToSpawn.Count; i++)
            {
                indices.Add(i);
            }

            for (int i = 0; i < lettersToSpawn.Count; i++)
            {
                int randIndex = indices[Random.Range(0, indices.Count)];
                indices.Remove(randIndex);

                LL_LetterData letterToSpawn = lettersToSpawn[randIndex];

                SeparateLetterController separateLetterController = separateLetterControllers[i];
                separateLetterController.Enable();
                separateLetterController.SetPosition(transform.position);
                separateLetterController.SetLetter(letterToSpawn);
                separateLetterController.SetRotation(new Vector3(0, 0, Random.Range(0, 4) * 90));
                separateLetterController.SetIsKinematic(false);
                yield return new WaitForSeconds(0.8f);
            }
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

