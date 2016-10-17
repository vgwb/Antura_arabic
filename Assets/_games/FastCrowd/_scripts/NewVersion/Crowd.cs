using UnityEngine;
using System.Collections.Generic;

namespace EA4S.FastCrowd
{
    /// <summary>
    /// This class manages the letters crowd
    /// </summary>
    public class Crowd : MonoBehaviour
    {
        public LetterObjectView livingLetterPrefab;
        public GameObject puffPrefab;

        List<FastCrowdLivingLetter> letters = new List<FastCrowdLivingLetter>();

        Queue<FastCrowdLivingLetter> toDestroy = new Queue<FastCrowdLivingLetter>();
        float destroyTimer = 0;

        public void AddLivingLetter(ILivingLetterData letter)
        {
            LetterObjectView letterObjectView = Instantiate(livingLetterPrefab);
            letterObjectView.transform.SetParent(transform, true);
            Vector3 newPosition = Vector3.zero;
            GameplayHelper.RandomPointInWalkableArea(transform.position, 20f, out newPosition);
            letterObjectView.transform.position = newPosition;
            letterObjectView.Init(letter);

            var livingLetter = letterObjectView.gameObject.AddComponent<FastCrowdLivingLetter>();

            letters.Add(livingLetter);
        }

        public void Clean()
        {
            foreach (var l in letters)
                toDestroy.Enqueue(l);

            letters.Clear();
        }

        void Update()
        {
            if (toDestroy.Count > 0)
            {
                destroyTimer -= Time.deltaTime;

                if (destroyTimer <= 0)
                {
                    destroyTimer = 0.1f;

                    var puffGo = GameObject.Instantiate(puffPrefab);
                    puffGo.AddComponent<AutoDestroy>().duration = 2;
                    puffGo.SetActive(true);

                    var t = toDestroy.Dequeue();
                    puffGo.transform.position = t.transform.position;
                    Destroy(t.gameObject);
                }
            }
        }
    }
}
