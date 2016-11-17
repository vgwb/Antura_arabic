using System;
using DG.Tweening;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class PronounceAndFlipQuestion : MonoBehaviour, IQuestionDecoration
    {
        bool triggered = false;
        public void TriggerOnAnswered()
        {
            if (triggered)
                return;

            triggered = true;
            PlaySound();

            transform.DORotate( new Vector3( 0, 180, 0), 1);
        }

        private void PlaySound()
        {
            AssessmentConfiguration.Instance.Context.GetAudioManager()
                .PlayLetterData(GetComponent<LetterObjectView>().Data);
        }

        public void TriggerOnSpawned()
        {
            PlaySound();
        }

        void Awake()
        {
            transform.rotation = Quaternion.Euler( Vector3.zero);
        }

        public float TimeToWait()
        {
            return 1.0f;
        }
    }
}
