using EA4S.Audio;
using UnityEngine;

namespace EA4S.Antura
{
    public class TouchToBark : MonoBehaviour
    {
        public void OnMouseDown()
        {
            GetComponent<AnturaAnimationController>().DoShout(() => { AudioManager.I.PlaySound(Sfx.DogBarking); });
        }
    }
}