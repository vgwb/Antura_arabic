using UnityEngine;

namespace EA4S.Map
{
    /// <summary>
    /// Represents a single dot in the Map scene, from which a new play session can be accessed.
    /// </summary>
    // refactor: can be a non-MonoBehaviour class
    public class Dot : MonoBehaviour
    {
        public int learningBlockActual;
        public int playSessionActual;
        public int pos;
    }
}
