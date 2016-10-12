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

        List<GameObject> letters = new List<GameObject>();

        public void AddLivingLetter(ILivingLetterData letter)
        {
            LetterObjectView letterObjectView = Instantiate(livingLetterPrefab);
            letterObjectView.transform.SetParent(transform, true);
            Vector3 newPosition = Vector3.zero;
            GameplayHelper.RandomPointInWalkableArea(transform.position, 20f, out newPosition);
            letterObjectView.transform.position = newPosition;
            letterObjectView.Init(letter);
        }

        public void Clean()
        {
            foreach (var l in letters)
                Destroy(l);
            letters.Clear();
        }
    }
}
