using System;
using UnityEngine;

namespace Antura.Rewards.Collectible
{
    
    public class CollectibleBone : MonoBehaviour
    {
        private bool _collected;
        public Action OnPickUp;

        public void Initialise(float duration)
        {
            _collected = false;

            Invoke("DestroyObject", duration);
        }

        public void OnMouseDown()
        {
            if (_collected) return;

            _collected = true;

            if (OnPickUp != null) OnPickUp.Invoke();

            DestroyObject();
        }

        void DestroyObject()
        {
            Destroy(gameObject);
        }
    }

}
