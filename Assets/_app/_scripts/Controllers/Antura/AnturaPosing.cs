using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S
{
    public class AnturaPosing : MonoBehaviour
    {
        public GameObject PropPirateHat;
        public GameObject PropPirateHook;


        void OnMouseDown()
        {
            AudioManager.I.PlaySfx(Sfx.DogBarking);
        }
    }
}