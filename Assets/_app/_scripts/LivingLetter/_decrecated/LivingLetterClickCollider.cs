using UnityEngine;

// refactor: remove?
namespace EA4S.LivingLetters
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