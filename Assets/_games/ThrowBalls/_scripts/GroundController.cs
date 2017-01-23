using UnityEngine;

namespace EA4S.Minigames.ThrowBalls
{
    public class GroundController : MonoBehaviour {

        public static GroundController instance;

        void Awake()
        {
            instance = this;
        }
    }
}