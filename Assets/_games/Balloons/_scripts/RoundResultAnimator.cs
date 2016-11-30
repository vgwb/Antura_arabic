using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S.Balloons
{
    public class RoundResultAnimator : MonoBehaviour
    {
        public LetterObjectView LLPrefab;
        public ParticleSystem vfx;
        public Vector3 wrongMarkPosition1;
        public Vector3 wrongMarkPosition2;
        public Vector3 onscreenPosition;
        public Vector3 offscreenPosition;
        public float moveDuration;

        public void ShowWin(ILivingLetterData livingLetterData)
        {
            this.transform.position = offscreenPosition;
            LLPrefab.gameObject.SetActive(true);
            vfx.gameObject.SetActive(true);
            if (livingLetterData != null)
            {
                LLPrefab.Init(livingLetterData);
            }
            LLPrefab.DoHorray();
            vfx.Play();

            StartCoroutine(Move_Coroutine(offscreenPosition, onscreenPosition, moveDuration));
        }

        public void ShowLose(ILivingLetterData livingLetterData)
        {
            this.transform.position = offscreenPosition;
            LLPrefab.gameObject.SetActive(true);
            vfx.gameObject.SetActive(true);
            if (livingLetterData != null)
            {
                LLPrefab.Init(livingLetterData);
            }
            LLPrefab.DoAngry();
            TutorialUI.MarkNo(wrongMarkPosition1, TutorialUI.MarkSize.Huge);
            TutorialUI.MarkNo(wrongMarkPosition2, TutorialUI.MarkSize.Huge);

            StartCoroutine(Move_Coroutine(offscreenPosition, onscreenPosition, moveDuration));
        }

        public void Hide()
        {
            StartCoroutine(Move_Coroutine(onscreenPosition, offscreenPosition, moveDuration, true));
        }

        private IEnumerator Move_Coroutine(Vector3 from, Vector3 to, float duration, bool hide = false)
        {
            var interpolant = 0f;
            var lerpProgress = 0f;
            var lerpLength = duration;

            while (lerpProgress < lerpLength)
            {
                transform.position = Vector3.Lerp(from, to, interpolant);
                lerpProgress += Time.deltaTime;
                interpolant = lerpProgress / lerpLength;
                interpolant = Mathf.Sin(interpolant * Mathf.PI * 0.5f);
                yield return new WaitForFixedUpdate();
            }

            if (hide)
            {
                LLPrefab.Label.text = "";
                LLPrefab.gameObject.SetActive(false);
                vfx.gameObject.SetActive(false);
            }
        }
    }
}