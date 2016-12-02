using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S.MakeFriends
{
    public class RoundResultAnimator : MonoBehaviour
    {
        public ParticleSystem vfx;
        public Vector3 wrongMarkPosition;

        public void ShowWin()
        {
            vfx.gameObject.SetActive(true);
            vfx.Play();
        }

        public void ShowLose()
        {
            TutorialUI.MarkNo(wrongMarkPosition, TutorialUI.MarkSize.Huge);
        }

        public void Hide()
        {
            vfx.Stop();
        }
    }
}