using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S
{
    public class LivingLetterClickCollider : MonoBehaviour
    {
        public LivingLetter Controller;

        void OnMouseDown()
        {
            Controller.Clicked();
        }
    }
}