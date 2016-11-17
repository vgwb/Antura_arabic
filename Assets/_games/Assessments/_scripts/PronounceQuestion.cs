using DG.Tweening;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class PronounceQuestion : MonoBehaviour, IQuestionDecoration
    {
        public void TriggerOnAnswered()
        {
            AssessmentConfiguration.Instance.Context.GetAudioManager()
                .PlayLetterData( GetComponent< LetterObjectView>().Data);

            transform.DOShakeScale( 1,1,10,10);
        }

        public void TriggerOnSpawned()
        {
            
        }
    }
}
