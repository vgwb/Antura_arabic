using UnityEngine;
using System.Collections;
using EA4S;

// refactor: remove?
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