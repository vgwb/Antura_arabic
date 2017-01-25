using UnityEngine;
using System.Collections.Generic;

namespace EA4S.Map
{
    // refactor: can be a non-MonoBehaviour class
    public class Rope : MonoBehaviour
    {
        public int learningBlockRope;
        public List<GameObject> dots = new List<GameObject>();
    }
}
