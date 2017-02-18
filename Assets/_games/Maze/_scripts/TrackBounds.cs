using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Minigames.Maze
{
    public class TrackBounds : MonoBehaviour
    {
        public static TrackBounds instance;

        private MazeLetter mazeLetter;

        private void Awake()
        {
            instance = this;
        }
    }
}