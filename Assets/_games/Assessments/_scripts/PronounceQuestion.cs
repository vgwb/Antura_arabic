using DG.Tweening;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class PronounceQuestion : MonoBehaviour, IQuestionAnswered
    {
        public void Trigger( IAudioManager audioManager)
        {
            audioManager.PlayLetterData(
            GetComponent< LetterObjectView>().Data);

            transform.DOShakeScale( 1,1,10,10);
        }
    }
}
