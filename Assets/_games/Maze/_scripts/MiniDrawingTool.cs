using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Minigames.Maze
{
    public class MiniDrawingTool : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.name == "MazeLetter")
            {
                MazeGameManager.instance.OnDrawnLetterWrongly();
            }
        }
    }
}