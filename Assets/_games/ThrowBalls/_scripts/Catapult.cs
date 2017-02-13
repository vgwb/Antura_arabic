using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Minigames.ThrowBalls
{
    public class Catapult : MonoBehaviour
    {
        private void OnMouseDown()
        {
            BallController.instance.OnBallTugged();
        }
    }
}