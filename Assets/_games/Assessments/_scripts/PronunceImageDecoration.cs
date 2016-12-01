using System;
using UnityEngine;

namespace EA4S.Assessment
{
    public class PronunceImageDecoration : MonoBehaviour, IQuestionDecoration
    {
        public void TriggerOnAnswered()
        {
            PlaySound();
        }

        private void PlaySound()
        {
            AssessmentConfiguration.Instance.Context.GetAudioManager()
                .PlayLetterData( GetComponent< LetterObjectView>().Data);
        }

        public void TriggerOnSpawned()
        {
            PlaySound();
        }

        public float TimeToWait()
        {
            return 0.05f;
        }
    }
}
