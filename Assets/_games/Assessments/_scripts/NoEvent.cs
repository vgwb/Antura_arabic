using UnityEngine;

namespace EA4S.Assessment
{
    internal class NoEvent : MonoBehaviour, IQuestionDecoration
    {
        public void TriggerOnAnswered()
        {

        }

        public void TriggerOnSpawned()
        {
            
        }

        public float TimeToWait()
        {
            return 0.05f;
        }
    }
}
