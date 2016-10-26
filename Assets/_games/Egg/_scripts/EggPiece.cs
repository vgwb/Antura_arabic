using System;
using UnityEngine;

namespace EA4S.Egg
{
    public class EggPiece : MonoBehaviour
    {
        public Action onPooEnd;

        bool poofed = false;

        public void Reset()
        {
            gameObject.SetActive(true);

            poofed = false;
        }

        public void Poof()
        {
            if(!poofed)
            {
                poofed = true;

                gameObject.SetActive(false);

                OnPoofEnd();
            }
        }

        void OnPoofEnd()
        {
            if (onPooEnd != null)
            {
                onPooEnd();
            }
        }
    }
}
