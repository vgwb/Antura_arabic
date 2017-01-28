using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Minigames.Maze
{
    public class TrackBounds : MonoBehaviour
    {
        private MazeLetter mazeLetter;
        
        private void OnMouseOver()
        {
            mazeLetter.OnPointerOverTrackBounds();
        }

        public void SetMazeLetter(MazeLetter mazeLetter)
        {
            this.mazeLetter = mazeLetter;
        }
    }
}