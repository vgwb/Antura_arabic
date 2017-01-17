using UnityEngine;
using System.Collections;
using EA4S.LivingLetters;

namespace EA4S.Maze
{
    public class MazeLL : MonoBehaviour
    {
        public LetterObjectView letter;
        // Use this for initialization
        void Start()
        {
            letter = GetComponent<LetterObjectView>();
            letter.SetState(LLAnimationStates.LL_rocketing);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}