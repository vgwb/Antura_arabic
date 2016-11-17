using DG.Tweening;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class PronounceAndFlipQuestion : MonoBehaviour, IQuestionAnswered
    {
        bool triggered = false;
        public void Trigger( IAudioManager audioManager)
        {
            if (triggered)
                return;

            triggered = true;
            audioManager
                .PlayLetterData(
                    GetComponent< LetterObjectView>().Data);

            transform.DORotate( new Vector3( 0, 180, 0), 1);
        }

        void Awake()
        {
            transform.rotation = Quaternion.Euler( Vector3.zero);
        }
    }
}
