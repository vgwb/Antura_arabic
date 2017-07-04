using UnityEngine;
using EA4S.LivingLetters;

namespace EA4S.Minigames.Maze
{
    public class MazeLL : MonoBehaviour
    {
        public LivingLetterController letter;
        // Use this for initialization
        void Start()
        {
            letter = GetComponent<LivingLetterController>();
            letter.SetState(LLAnimationStates.LL_rocketing);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}