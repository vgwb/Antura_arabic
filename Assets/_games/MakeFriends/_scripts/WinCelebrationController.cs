using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S.MakeFriends
{
    public class WinCelebrationController : MonoBehaviour
    {
        public ParticleSystem vfx;

        public void Show()
        {
            vfx.gameObject.SetActive(true);
            vfx.Play();
        }

        public void Hide()
        {
            vfx.Stop();
        }
    }
}