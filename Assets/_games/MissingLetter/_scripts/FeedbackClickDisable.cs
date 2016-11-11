using UnityEngine;
using System.Collections;

namespace EA4S.MissingLetter
{
    public class FeedbackClickDisable : MonoBehaviour
    {
        bool mIsPlaying = false;

        void OnMouseDown()
        {
            if(!mIsPlaying)
            {
                mIsPlaying = true;
                AudioManager.I.PlaySfx(Sfx.Splat);
                StartCoroutine(Utils.LaunchDelay(0.5f, setPlaying, false));
            }
        }

        void setPlaying(bool _isPlaying)
        {
            mIsPlaying = _isPlaying;
        }
    }
}
