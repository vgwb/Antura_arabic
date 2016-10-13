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

        List<FastCrowdLivingLetter> letters = new List<FastCrowdLivingLetter>();

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
            // TODO: Poof!

            foreach (var l in letters)
                Destroy(l.gameObject);
            letters.Clear();
        }
    }
}
