using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

namespace EA4S.ThrowBalls
{
    public class UIController : MonoBehaviour
    {
        public static UIController instance;

        public GameObject letterHint;
        public TMP_Text letterHintText;

        private int numPokeballs;

        public GameObject[] cracks;
        private List<int> unusedCracks;

        void Awake()
        {
            instance = this;

            unusedCracks = new List<int>();
        }

        public void SetLetterHint(LL_LetterData _data)
        {
            letterHintText.text = _data.TextForLivingLetter;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void OnScreenCracked()
        {
            if (unusedCracks.Count != 0 && !ThrowBallsGameManager.Instance.IsTutorialLevel())
            {
                int randomCrackIndex = UnityEngine.Random.Range(0, unusedCracks.Count);
                cracks[unusedCracks[randomCrackIndex]].SetActive(true);
                unusedCracks.RemoveAt(randomCrackIndex);

                AudioManager.I.PlaySfx(Sfx.ScreenHit);
            }
        }

        public void Reset()
        {
            unusedCracks.Clear();

            for (int i = 0; i < cracks.Length; i++)
            {
                cracks[i].SetActive(false);
                unusedCracks.Add(i);
            }
        }
    }
}

