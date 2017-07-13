using UnityEngine;
using System.Collections.Generic;

namespace Antura.Map
{
    public class MapRope : MonoBehaviour
    {
        public int assignedLearningBlock;
        public List<MapDot> dots = new List<MapDot>();

        public MapDot DotForPS(int ps)
        {
            return dots[ps - 1];
        }
    }
}